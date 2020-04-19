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
using System.Linq;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace Portals
{
    public enum PortalType : int
    {
        H24 = 0,
        Index = 1,
        Jutarnji = 2,
        Vecernji = 3,
        Dnevnik = 4,
        Net = 5
    }

    public static class HapScrap
    {
        public static ThreadSafeList<Article> ScrapPortal(PortalType type)
        {
            var web = new HtmlWeb();
            var articles = new ThreadSafeList<Article>();
            if (type == PortalType.H24)
            {
                //TODO: new implementation with HAP
                return Utilities.Scrap24h();
            }
            else if (type == PortalType.Index)
            {
                List<string> links = new List<string>();
                List<string> titles = new List<string>();
                List<string> leads = new List<string>();

                var document = web.Load(Index.ScrapUrl);
                var body = document.DocumentNode.SelectSingleNode("//body");

                var article_links = body.SelectNodes("//a[@href]");
                foreach (var link in article_links)
                {
                    string hrefValue = link.GetAttributeValue("href", string.Empty);
                    if (link.GetClasses().Contains("vijesti-text-hover") && link.GetClasses().Contains("scale-img-hover") && link.GetClasses().Contains("flex"))
                        links.Add(hrefValue);
                }

                var article_titles = body.SelectNodes("//h3");
                foreach (var title in article_titles)
                    if (title.GetClasses().Contains("title"))
                        titles.Add(title.InnerText);

                var article_spans = body.SelectNodes("//span");
                foreach (var summary in article_spans)
                    if (summary.GetClasses().Contains("summary"))
                        leads.Add(summary.InnerText);

                if (links.Count == leads.Count && leads.Count == titles.Count)
                {
                    for (int i = 0; i < links.Count; i++)
                    {
                        var article = new Article
                        {
                            ID = links[i].Replace('/', '-').Replace("-vijesti-clanak-", "").Replace(".aspx", ""),
                            Link = Index.BaseUrl + links[i],
                            Title = titles[i],
                            Lead = leads[i]
                        };
                        articles.Add(article);
                    }
                }

                for (int i = 0; i < articles.Count(ar => true); i++)
                {
                    var article = articles[i];
                    var article_document = web.Load(article.Link);
                    var article_body = article_document.DocumentNode.SelectSingleNode("//body");

                    var article_span = article_body.SelectNodes("//span");
                    foreach (var author in article_span)
                    {
                        if (author.GetClasses().Contains("author"))
                        {
                            article.Author = author.InnerText;
                            break;
                        }
                    }

                    foreach (var time in article_span)
                    {
                        if (time.GetClasses().Contains("time"))
                        {
                            var str = time.InnerHtml;
                            var regex = "datetime=\"(.*?)\"";
                            MatchCollection matches = Regex.Matches(str, regex);
                            if (matches.Count > 0)
                                foreach (Match match in matches)
                                {
                                    DateTime dt = DateTime.Parse(match.Groups[1].ToString().Trim());
                                    article.Time = dt.ToString();
                                }
                        }
                    }

                    var article_divs = article_body.SelectNodes("//div");
                    foreach (var div in article_divs)
                    {
                        if (div.GetClasses().Contains("text"))
                        {
                            string content = "";
                            var article_ps = div.SelectNodes("//p");
                            foreach (var p in article_ps)
                                content += p.InnerText + "<br><br>";
                            article.Content = content;
                            break;
                        }
                    }
                }
            }
            else if (type == PortalType.Jutarnji)
            {
                List<string> links = new List<string>();
                List<string> titles = new List<string>();
                List<string> leads = new List<string>();

                HtmlDocument[] documents = new HtmlDocument[3]
                {
                    web.Load(Jutarnji.ScrapUrl1),
                    web.Load(Jutarnji.ScrapUrl2),
                    web.Load(Jutarnji.ScrapUrl3)
                };

                foreach (var document in documents)
                {
                    var body = document.DocumentNode.SelectSingleNode("//body");

                    var article_titles = body.SelectNodes("//h4");
                    foreach (var title in article_titles)
                        if (title.GetClasses().Contains("title"))
                        {
                            titles.Add(title.InnerText);
                            var str = title.InnerHtml;
                            var regex = "<a href=\"(.*?)\"";
                            MatchCollection matches = Regex.Matches(str, regex);
                            if (matches.Count > 0)
                                foreach (Match match in matches)
                                {
                                    var link = match.Groups[1].ToString().Trim();
                                    links.Add(link);
                                }
                        }

                    var article_ps = body.SelectNodes("//p");
                    foreach (var p in article_ps)
                    {
                        if (p.GetClasses().Contains("overline"))
                        {
                            if (p.InnerHtml.Contains("<a href"))
                            {
                                var lead = p.InnerHtml.Split("/\">")[1].Split("</a>")[0];
                                leads.Add(lead);
                            }
                        }
                    }
                }

                if (links.Count == leads.Count && leads.Count == titles.Count)
                {
                    for (int i = 0; i < links.Count; i++)
                    {
                        var article = new Article
                        {
                            ID = links[i].Replace(Jutarnji.BaseUrl + "/", "").Replace('/', '-').TrimEnd(new char[] { '-' }),
                            Link = links[i],
                            Title = titles[i],
                            Lead = leads[i]
                        };
                        articles.Add(article);
                    }
                }

                for (int i = 0; i < articles.Length; i++)
                {
                    var article = articles[i];
                    var article_document = web.Load(article.Link);
                    var article_body = article_document.DocumentNode.SelectSingleNode("//body");

                    var article_ps = article_body.SelectNodes("//p");
                    foreach (var p in article_ps)
                    {
                        if (p.GetClasses().Contains("published-date"))
                        {
                            article.Time = p.InnerText;
                            break;
                        }
                    }

                    var article_uls = article_body.SelectNodes("//ul");
                    foreach (var ul in article_uls)
                    {
                        if (ul.InnerText.Contains("AUTOR"))
                        {
                            var autor = ul.InnerText.Trim().Replace("\n", "").Replace("  ", " ").Replace("AUTOR: ", "");
                            autor = autor.Split("OBJAVLJENO:")[0];
                            article.Author = autor;
                        }
                    }

                    string content = "";
                    var article_sections = article_body.SelectNodes("//section");
                    foreach (var section in article_sections)
                    {
                        if (section.Id == "CImaincontent" && section.GetClasses().Contains("ci_body"))
                        {
                            var section_ps = section.SelectNodes("//p");
                            foreach (var p in section_ps)
                            {
                                var text = p.InnerText;
                                if (!string.IsNullOrEmpty(text.Trim()) && !text.Contains("VIDEO") && !text.Contains("PROMO") && !text.Contains(article.Lead) && !text.Contains(article.Time) && !text.Contains("Copyright © HANZA MEDIA d.o.o. Sva prava pridržana."))
                                    content += text + "<br><br>";
                            }
                        }
                    }
                    article.Content = content;
                }
            }
            else if (type == PortalType.Vecernji)
            {
            }
            else if (type == PortalType.Dnevnik)
            {
            }
            else if (type == PortalType.Net)
            {
            }
            return articles;
        }
    }
}