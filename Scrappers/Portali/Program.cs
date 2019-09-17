/*
    Live feed of Croatian public news portals
    Copyright (C) 2019 Bruno Fištrek

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
using System.Net;
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Portals
{
    public class Program
    {
        static void Main(string[] args)
        {
            List<Article> _24h = Scrap24h();
            var valid = _24h.Count(a => { return a.IsValidArticle(); });
            var invalid = _24h.Count(a => { return !a.IsValidArticle(); });
            Console.WriteLine($"Total articles: {_24h.Count} | Valid articles: {valid} | Invalid articles: {invalid}");
            Console.ReadKey();
        }

        public static List<Article> Scrap24h()
        {
            string[] urls = new string[3]
            {
                "https://www.24sata.hr/feeds/aktualno.xml",
                "https://www.24sata.hr/feeds/najnovije.xml",
                "https://www.24sata.hr/feeds/news.xml"
            };

            string
                baseurl1 = "https://www.24sata.hr/",
                baseurl2 = "https://www.24sata.hr/news",
                //komentari = "/komentari",
                title = "<h1 class=\"article__title\">",
                lead = "<h2 class=\"article__lead\">",
                author = "<span class=\"article__author \">",
                time = "datetime=\"",
                content = "<div class=\"article__text\">",
                contentend = "<footer class=\"article__footer cf\">";

            string[] datas = new string[3] { "", "", "" };
            List<string> links = new List<string>();
            List<Article> articles = new List<Article>();
            try
            {
                using (var wc = new WebClient())
                {
                    int count = 0;
                    foreach (var url in urls)
                        datas[count++] = wc.DownloadString(url);
                }

                foreach (var data in datas)
                    foreach (var link in data.Split(new[] { "<link>" }, StringSplitOptions.None))
                        foreach (var sublink in link.Split(new[] { "</link>" }, StringSplitOptions.None))
                            if (sublink.StartsWith("http") && !sublink.Equals(baseurl1) && !sublink.Equals(baseurl2))
                                links.Add(sublink);

                links.ForEach(link =>
                {
                    if (!link.Equals(baseurl1) && !link.Equals(baseurl2) && link.Contains(baseurl1))
                    {
                        using (var wc = new WebClient())
                        {
                            var article = new Article();
                            article.Link = link;

                            var i = link.Substring(link.LastIndexOf("/") + 1);
                            article.ID = i;

                            var d = wc.DownloadString(link);

                            try
                            {
                                var t = d.Substring(d.IndexOf(title) + title.Length).Split("</h1>")[0].Replace("&#39;", "'").Replace("&quot;", "\"").Trim();
                                article.Title = title;
                            }
                            catch
                            {
                                article.Title = "exception";
                            }

                            try
                            {
                                var l = d.Substring(d.IndexOf(lead) + lead.Length).Split("</h2>")[0].Replace("&#39;", "'").Replace("&quot;", "\"").Trim();
                                article.Lead = l;
                            }
                            catch
                            {
                                article.Lead = "exception";
                            }

                            try
                            {
                                var a = d.Substring(d.IndexOf(author)).Split("</span>")[1].Replace("<span>", "").Trim();
                                article.Author = a;
                            }
                            catch
                            {
                                article.Author = "exception";
                            }

                            try
                            {
                                var t = d.Substring(d.IndexOf(time) + time.Length).Split("\"")[0].Trim();
                                article.Time = t;
                            }
                            catch
                            {
                                article.Time = "exception";
                            }

                            try
                            {
                                var c = d.Substring(d.IndexOf(content) + content.Length).Split(contentend)[0].Trim();
                                MatchCollection matches = Regex.Matches(c, "<p>(.*?)</p>");
                                c = "";
                                if (matches.Count > 0)
                                    foreach (Match m in matches)
                                    {
                                        var s = m.Groups[1].ToString().Trim();
                                        if (!s.Contains("Tema: <a"))
                                            c += s + "\n";
                                    }

                                article.Content = c.Trim().Replace("<strong>POGLEDAJTE VIDEO:</strong>", "").Replace("<strong>POGLEDAJTE VIDEO</strong>", "").Trim();
                            }
                            catch
                            {
                                article.Content = "exception";
                            }

                            try
                            {
                                //komentari
                            }
                            catch
                            {
                                article.Comments = null;
                            }

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