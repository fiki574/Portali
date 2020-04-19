/*
    Live feed of Croatian public news portals
    Copyright (C) 2020 Bruno Fištrek

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Portals
{
    public static class Utilities
    {
        public static string GetBase64(string path)
        {
            try
            {
                return Convert.ToBase64String(File.ReadAllBytes(path));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        public static void ClearDirectory(string path)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(path);
                foreach (var file in di.GetFiles())
                    file.Delete();

                foreach (var dir in di.GetDirectories())
                    dir.Delete(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static void UpdateList(ThreadSafeList<Article> articles, PortalType type)
        {
            ThreadSafeList<string> remove = new ThreadSafeList<string>();
            articles.ForEach(a =>
            {
                a.ReplaceInvalidText();
                if (!a.ShouldBeDisplayed(type))
                    remove.Add(a.ID);
            });
            remove.ForEach(s => articles.Remove(a => a.ID == s));
            Console.WriteLine($"Filtering out {remove.Count(s => true)} articles that shouldn't be displayed");
        }

        public static ThreadSafeList<Article> Scrap24h()
        {
            string[] datas = new string[3] { "", "", "" };
            List<string> links = new List<string>();
            ThreadSafeList<Article> articles = new ThreadSafeList<Article>();
            try
            {
                using (var wc = new WebClient())
                {
                    int count = 0;
                    foreach (var url in H24.URLs)
                        datas[count++] = wc.DownloadString(url);
                }

                foreach (var data in datas)
                    foreach (var lnk in data.Split(new[] { "<link>" }, StringSplitOptions.None))
                        foreach (var sublink in lnk.Split(new[] { "</link>" }, StringSplitOptions.None))
                            if (sublink.StartsWith("http") && !sublink.Equals(H24.BaseUrl1) && !sublink.Equals(H24.BaseUrl2))
                                links.Add(sublink);

                links.ForEach(link =>
                {
                    if (!link.Equals(H24.BaseUrl1) && !link.Equals(H24.BaseUrl2) && link.Contains(H24.BaseUrl1))
                    {
                        using (var wc = new WebClient())
                        {
                            var d = "";
                            var article = new Article();
                            try
                            {
                                article.Link = link;
                                var i = link.Substring(link.LastIndexOf("/") + 1);
                                article.ID = i;
                                d = wc.DownloadString(link);
                            }
                            catch
                            {
                                Console.WriteLine("Couldn't download data from: " + link);
                                return;
                            }

                            try
                            {
                                var t = d.Substring(d.IndexOf(H24.TitleHtml) + H24.TitleHtml.Length).Split("</h1>")[0].Replace("&#39;", "'").Replace("&quot;", "\"").Trim();
                                article.Title = t;
                            }
                            catch
                            {
                                article.Title = "exception";
                            }

                            try
                            {
                                var l = d.Substring(d.IndexOf(H24.LeadHtml) + H24.LeadHtml.Length).Split("</h2>")[0].Replace("&#39;", "'").Replace("&quot;", "\"").Trim();
                                article.Lead = l;
                            }
                            catch
                            {
                                article.Lead = "exception";
                            }

                            try
                            {
                                var a = d.Substring(d.IndexOf(H24.AuthorHtml)).Split("</span>")[1].Replace("<span>", "").Trim();
                                article.Author = a;
                            }
                            catch
                            {
                                article.Author = "exception";
                            }

                            try
                            {
                                var t = d.Substring(d.IndexOf(H24.TimeHtml) + H24.TimeHtml.Length).Split("\"")[0].Trim();
                                DateTime dt = DateTime.Parse(t);
                                article.Time = dt.ToString();
                            }
                            catch
                            {
                                article.Time = "exception";
                            }

                            try
                            {
                                var c = d.Substring(d.IndexOf(H24.ContentHtml) + H24.ContentHtml.Length).Split(H24.ContentEndHtml)[0].Trim();
                                foreach (var regex in H24.ContentRegex)
                                {
                                    MatchCollection matches = Regex.Matches(c, regex);
                                    var tmp = "";
                                    if (matches.Count > 0)
                                        foreach (Match m in matches)
                                        {
                                            var s = m.Groups[1].ToString().Trim();
                                            if (!s.Contains("Tema: <a") && !s.Contains("SERIJAL '"))
                                                tmp += s + "<br><br>";
                                        }

                                    article.Content += tmp.Trim().Replace("<strong>POGLEDAJTE VIDEO:</strong>", "").Replace("<strong>POGLEDAJTE VIDEO</strong>", "").Replace("POGLEDAJTE VIDEO:", "").Replace("POGLEDAJTE VIDEO", "").Trim();
                                }
                            }
                            catch
                            {
                                article.Content = "exception";
                            }

                            if (!articles.Contains(a => a.ID == article.ID))
                                articles.Add(article);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return articles;
        }
    }
}