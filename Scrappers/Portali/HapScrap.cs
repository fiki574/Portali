/*
    Live feed of Croatian public news portals
    Copyright (C) 2020/2021 Bruno Fištrek

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
using System.Net;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace Portals
{
    public static class HapScrap
    {
        public static ThreadSafeList<Article> ScrapPortal(PortalType type)
        {
            var web = new HtmlWeb();
            var articles = new ThreadSafeList<Article>();
            if (type == PortalType.H24)
            {
                string[] datas = new string[3] { "", "", "" };
                List<string> links = new List<string>();
                using (var wc = new WebClient())
                {
                    int count = 0;
                    foreach (var url in H24.URLs)
                    {
                        try
                        {
                            datas[count] = wc.DownloadString(url);
                        }
                        catch
                        {
                            datas[count] = "";
                        }
                        count++;
                    }
                }

                foreach (var data in datas)
                    foreach (var lnk in data.Split(new[] { "<link>" }, StringSplitOptions.None))
                        foreach (var sublink in lnk.Split(new[] { "</link>" }, StringSplitOptions.None))
                            if (sublink.StartsWith("http") && !sublink.Equals(H24.BaseUrl1) && !sublink.Equals(H24.BaseUrl2))
                                links.Add(sublink);

                foreach (var link in links)
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
                                continue;
                            }

                            var document = new HtmlDocument();
                            HtmlNode body = null;
                            try
                            {
                                document.LoadHtml(d);
                                body = document.DocumentNode.SelectSingleNode("//body");
                            }
                            catch
                            {
                                continue;
                            }

                            if (body == null)
                                continue;

                            try
                            {
                                var article_titles = body.SelectNodes("//h1");
                                foreach (var title in article_titles)
                                {
                                    article.Title = title.InnerHtml;
                                    break;
                                }
                            }
                            catch
                            {
                                article.Title = "exception";
                            }

                            try
                            {
                                var article_leads = body.SelectNodes("//h2");
                                foreach (var lead in article_leads)
                                {
                                    article.Lead = lead.InnerHtml;
                                    break;
                                }
                            }
                            catch
                            {
                                article.Lead = "exception";
                            }

                            try
                            {
                                var article_ul = body.SelectNodes("//ul")[1];
                                article.Author = article_ul.InnerHtml.Trim().Replace("<li>", "").Replace("</li>", ",").Trim();
                                article.Author = article.Author.TrimEnd(',').Trim();
                                article.Author = article.Author.Replace("<a href=\"https://www.24sata.hr/tagovi/", "").Trim().Replace("<a href=\"https://www.24sata.hr/autori/", "").Trim().Replace("\"></a>", "").Replace(" ", "").Replace("\n", "");
                            }
                            catch
                            {
                                article.Author = "exception";
                            }

                            try
                            {
                                string regex = "\"datePublished\": \"(.*?)\"";
                                MatchCollection matches = Regex.Matches(body.InnerHtml, regex);
                                if (matches.Count > 0)
                                    foreach (Match match in matches)
                                    {
                                        DateTime dt = DateTime.Parse(match.Groups[1].ToString().Trim());
                                        article.Time = dt.ToString();
                                        break;
                                    }
                            }
                            catch
                            {
                                article.Time = "exception";
                            }

                            try
                            {
                                var idx = body.InnerHtml.IndexOf("<div class=\"article__text\">");
                                var content = body.InnerHtml.Substring(idx).Replace("<div class=\"article__text\">", "");
                                idx = content.IndexOf("</div>");
                                content = content.Substring(0, idx).Trim();
                                content = content.Replace("&gt;", ">").Replace("&lt;", "<").Replace("POGLEDAJTE VIDEO VIJESTI:", "").Replace("POGLEDAJTE VIDEO:", "").Replace("POGLEDAJTE VIDEO VIJESTI", "").Replace("POGLEDAJTE VIDEO", "");
                                content = content.Replace("<iframe", "");
                                article.Content = content;
                            }
                            catch
                            {
                                article.Content = "exception";
                                continue;
                            }

                            if (!articles.Contains(a => a.ID == article.ID))
                                articles.Add(article);
                        }
                    }
                }
                return articles;
            }
            else if (type == PortalType.Index)
            {
                List<string> links = new List<string>();
                List<string> titles = new List<string>();
                List<string> leads = new List<string>();

                var document = new HtmlDocument();
                HtmlNode body = null;
                try
                {
                    document = web.Load(Index.ScrapUrl);
                    body = document.DocumentNode.SelectSingleNode("//body");
                }
                catch
                {
                    return articles;
                }

                if (body == null)
                    return articles;

                var article_links = body.SelectNodes("//a[@href]");
                if (article_links != null)
                    foreach (var link in article_links)
                    {
                        string hrefValue = link.GetAttributeValue("href", string.Empty);
                        if (link.GetClasses().Contains("vijesti-text-hover") && link.GetClasses().Contains("scale-img-hover") && link.GetClasses().Contains("flex"))
                            links.Add(hrefValue);
                    }

                var article_titles = body.SelectNodes("//h3");
                if (article_titles != null)
                    foreach (var title in article_titles)
                        if (title.GetClasses().Contains("title"))
                            titles.Add(title.InnerText);

                var article_spans = body.SelectNodes("//span");
                if (article_spans != null)
                    foreach (var summary in article_spans)
                        if (summary.GetClasses().Contains("summary"))
                            leads.Add(summary.InnerText);

                if (links.Count == leads.Count && leads.Count == titles.Count)
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

                for (int i = 0; i < articles.Count(ar => true); i++)
                {
                    var article = articles[i];
                    HtmlDocument article_document = null;
                    try
                    {
                        article_document = web.Load(article.Link);
                    }
                    catch
                    {
                        continue;
                    }

                    var article_body = article_document.DocumentNode.SelectSingleNode("//body");
                    if (article_body == null)
                        continue;

                    HtmlNodeCollection article_span = null;
                    try
                    {
                        article_span = article_body.SelectNodes("//span");
                        foreach (var author in article_span)
                            if (author.GetClasses().Contains("author"))
                            {
                                article.Author = author.InnerText;
                                break;
                            }
                    }
                    catch
                    {
                        article.Author = "exception";
                    }

                    if (article_span == null)
                        continue;

                    try
                    {
                        foreach (var time in article_span)
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
                    catch
                    {
                        article.Time = "exception";
                    }

                    try
                    {
                        var article_divs = article_body.SelectNodes("//div");
                        foreach (var div in article_divs)
                            if (div.GetClasses().Contains("text"))
                            {
                                string content = "";
                                var article_ps = div.SelectNodes("//p");
                                foreach (var p in article_ps)
                                {
                                    var s = p.InnerText;
                                    if (!s.Contains("iframe") && !s.ToLowerInvariant().Contains("index.me"))
                                        content += p.InnerText + "<br><br>";
                                }
                                article.Content = content;
                                break;
                            }
                    }
                    catch
                    {
                        article.Content = "exception";
                    }
                }
            }
            else if (type == PortalType.Jutarnji)
            {
                List<string> links = new List<string>();
                List<string> titles = new List<string>();
                List<string> times = new List<string>();

                List<string> base64encoded_html = new List<string>();
                try
                {
                    using (var wc = new WebClient())
                    {
                        var data = wc.DownloadString(Jutarnji.ScrapUrl);
                        for (int i = 1; i <= 30; i++)
                        {
                            string item = "htmlItemsPart" + i;
                            if (data.Contains(item))
                            {
                                var regex = item + "=\"(.*?)\"";
                                MatchCollection matches = Regex.Matches(data, regex);
                                if (matches.Count > 0)
                                    base64encoded_html.Add(matches[0].Groups[1].ToString());
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }

                HtmlDocument[] documents = new HtmlDocument[base64encoded_html.Count];
                for (int i = 0; i < base64encoded_html.Count; i++)
                    try
                    {
                        string html = Regex.Unescape(Encoding.UTF8.GetString(Convert.FromBase64String(base64encoded_html[i]))).Trim(new char[] { ' ', '"' });
                        documents[i] = new HtmlDocument();
                        documents[i].LoadHtml(html);
                    }
                    catch
                    {
                        documents[i] = null;
                    }

                foreach (var document in documents)
                {
                    if (document == null)
                        continue;

                    var bodies = document.DocumentNode.SelectNodes("//h3");
                    if (bodies == null)
                        continue;

                    foreach (var body in bodies)
                    {
                        if (!body.GetClasses().Contains("card__info"))
                            continue;

                        var spans = body.SelectNodes("//span");
                        if (spans != null)
                            foreach (var span in spans)
                            {
                                if (span.GetClasses().Contains("card__title") && !titles.Contains(span.InnerText))
                                    titles.Add(span.InnerText);
                                if (span.GetClasses().Contains("card__published") && !times.Contains(span.InnerText))
                                    times.Add(span.InnerText);
                            }

                        var articles_as = body.SelectNodes("//a[@href]");
                        if (articles_as != null)
                            foreach (var a in articles_as)
                            {
                                if (a.GetClasses().Contains("card__article-link"))
                                {
                                    string hrefValue = a.GetAttributeValue("href", string.Empty);
                                    if (!links.Contains(hrefValue))
                                        links.Add(hrefValue);
                                }
                            }
                    }
                }

                if (links.Count == titles.Count && titles.Count == times.Count)
                    for (int i = 0; i < links.Count; i++)
                    {
                        var article = new Article
                        {
                            ID = links[i].Replace(Jutarnji.BaseUrl + "/", "").Replace('/', '-').TrimEnd('-').TrimStart('-'),
                            Link = Jutarnji.BaseUrl + links[i],
                            Title = titles[i],
                            Time = times[i]
                        };

                        if (article.Link.Contains("/vijesti/hrvatska"))
                            articles.Add(article);
                    }

                for (int i = 0; i < articles.Length; i++)
                {
                    var article = articles[i];
                    HtmlDocument document = null;
                    try
                    {
                        document = web.Load(article.Link);
                    }
                    catch
                    {
                        continue;
                    }

                    if (document == null)
                        continue;

                    var body = document.DocumentNode.SelectSingleNode("//body");
                    if (body == null)
                        continue;

                    var divs = body.SelectNodes("//div");
                    if (divs != null)
                        foreach (var div in divs)
                        {
                            if (div.GetClasses().Contains("item__supertitle"))
                            {
                                article.Lead = div.InnerText;
                                break;
                            }
                        }

                    var spans = body.SelectNodes("//span");
                    if (spans != null)
                        foreach (var span in spans)
                        {
                            if (span.GetClasses().Contains("item__author-name"))
                            {
                                article.Author = span.InnerText;
                                break;
                            }
                        }

                    if (divs != null)
                        foreach (var div in divs)
                        {
                            if (div.GetClasses().Contains("itemFullText"))
                            {
                                var ps = div.SelectNodes("//p");
                                foreach (var p in ps)
                                    article.Content += p.InnerText + "<br><br>";
                            }
                        }
                }
            }
            else if (type == PortalType.Vecernji)
            {
                List<string> all_links = new List<string>();
                List<string> all_titles = new List<string>();
                List<string> all_leads = new List<string>();

                var doc = web.Load(Vecernji.ScrapUrl + "1");
                var bod = doc.DocumentNode.SelectSingleNode("//body");
                int articles_count = 0;

                var article_as = bod.SelectNodes("//a");
                foreach (var a in article_as)
                    if (a.GetClasses().Contains("header__daily"))
                    {
                        var inner = a.InnerHtml;
                        if (inner.Contains("Najnovije vijesti"))
                        {
                            var regex = "<strong>(.*?)</strong>";
                            MatchCollection matches = Regex.Matches(inner, regex);
                            if (matches.Count > 0)
                                foreach (Match match in matches)
                                {
                                    articles_count = Convert.ToInt32(match.Groups[1].ToString().Trim());
                                    break;
                                }
                        }
                    }

                int pages = articles_count / 10;
                if (articles_count % 10 != 0)
                    pages += 1;

                HtmlDocument[] documents = new HtmlDocument[pages];
                documents[0] = doc;
                for (int i = 1; i < pages; i++)
                {
                    try
                    {
                        documents[i] = web.Load(Vecernji.ScrapUrl + (i + 1).ToString());
                    }
                    catch
                    {
                        documents[i] = null;
                    }
                }

                foreach (var document in documents)
                {
                    if (document == null)
                        continue;

                    var body = document.DocumentNode.SelectSingleNode("//body");

                    if (body == null)
                        continue;

                    var article_titles = body.SelectNodes("//h2");
                    if (article_titles != null)
                        foreach (var title in article_titles)
                            if (title.GetClasses().Contains("card__title"))
                                all_titles.Add(title.InnerText);

                    var article_leads = body.SelectNodes("//span");
                    if (article_leads != null)
                        foreach (var span in article_leads)
                            if (span.GetClasses().Contains("card__label") && span.GetClasses().Contains("card__label--article"))
                                all_leads.Add(span.InnerText.Trim());

                    var article_links = body.SelectNodes("//a[@href]");
                    if (article_links != null)
                        foreach (var link in article_links)
                            if (link.GetClasses().Contains("card__link"))
                            {
                                string hrefValue = link.GetAttributeValue("href", string.Empty);
                                all_links.Add(hrefValue);
                            }
                }

                List<string> links = new List<string>();
                List<string> titles = new List<string>();
                List<string> leads = new List<string>();

                for (int i = 0; i < all_titles.Count; i++)
                {
                    if (!all_links[i].Contains("24sata") && !links.Contains(all_links[i]))
                    {
                        try
                        {
                            titles.Add(all_titles[i]);
                        }
                        catch
                        {
                            titles.Add("exception");
                        }

                        try
                        {
                            leads.Add(all_leads[i]);
                        }
                        catch
                        {
                            leads.Add("exception");
                        }

                        try
                        {
                            links.Add(all_links[i]);
                        }
                        catch
                        {
                            links.Add(Vecernji.BaseUrl);
                        }
                    }
                }

                if (links.Count == leads.Count && leads.Count == titles.Count)
                    for (int i = 0; i < links.Count; i++)
                    {
                        var article = new Article
                        {
                            ID = links[i].Replace('/', '-').TrimStart(new char[] { '-' }),
                            Link = Vecernji.BaseUrl + links[i],
                            Title = titles[i],
                            Lead = leads[i]
                        };
                        articles.Add(article);
                    }

                for (int i = 0; i < articles.Length; i++)
                {
                    var article = articles[i];
                    HtmlDocument article_document = null;
                    try
                    {
                        article_document = web.Load(article.Link);
                    }
                    catch
                    {
                        continue;
                    }

                    var article_body = article_document.DocumentNode.SelectSingleNode("//body");
                    if (article_body == null)
                        continue;

                    HtmlNodeCollection article_spans = null;
                    try
                    {
                        article_spans = article_body.SelectNodes("//span");
                        foreach (var span in article_spans)
                            if (span.GetClasses().Contains("article__lead"))
                            {
                                article.Lead = span.InnerText;
                                break;
                            }
                    }
                    catch
                    {
                        article.Lead = "exception";
                    }

                    if (article_spans == null)
                        continue;

                    HtmlNodeCollection article_divs = null;
                    try
                    {
                        article_divs = article_body.SelectNodes("//div");
                        foreach (var div in article_divs)
                            if (div.GetClasses().Contains("article__author") && div.InnerHtml.Contains("Autor"))
                            {
                                var articles_as = div.SelectNodes("//a");
                                foreach (var a in articles_as)
                                    if (a.GetClasses().Contains("article__author--link"))
                                        article.Author = a.InnerText;
                            }
                    }
                    catch
                    {
                        article.Author = "exception";
                    }

                    if (article_divs == null)
                        continue;

                    try
                    {
                        foreach (var span in article_spans)
                            if (span.GetClasses().Contains("article__header_date"))
                            {
                                var str = "<i class=\"icon icon-clock3 icon--linear\"></i>";
                                var date = span.InnerText.Replace(str, "").Trim();
                                article.Time = span.InnerText;
                                break;
                            }
                    }
                    catch
                    {
                        article.Time = "exception";
                    }

                    try
                    {
                        var content = "";
                        foreach (var div in article_divs)
                        {
                            if (div.GetClasses().Contains("article__body--main_content"))
                            {
                                var article_ps = div.SelectNodes("//p");
                                foreach (var p in article_ps)
                                {
                                    var s = p.InnerText;
                                    if (!p.GetClasses().Contains("commbox__content") && !p.GetClasses().Contains("js_commboxContent") && !s.Contains("VIDEO") && !s.Contains("Vaš preglednik ne omogućava pregled ovog sadržaja.") && !s.Contains("Za komentiranje je potrebna prijava/registracija.") && !s.Contains("iframe"))
                                        content += s + "<br><br>";
                                }
                                article.Content = content;
                                break;
                            }
                        }
                    }
                    catch
                    {
                        article.Content = "exception";
                    }
                }
            }
            else if (type == PortalType.Net)
            {
                List<string> links = new List<string>();
                List<string> titles = new List<string>();
                List<string> leads = new List<string>();

                HtmlDocument[] documents = new HtmlDocument[3];
                using (var wc = new WebClient())
                {
                    try
                    {
                        documents[0] = new HtmlDocument();
                        documents[0].LoadHtml(wc.DownloadString(Net.ScrapUrl1));
                    }
                    catch
                    {
                        documents[0].LoadHtml(Constants.Empty);
                    }

                    try
                    {
                        documents[1] = new HtmlDocument();
                        documents[1].LoadHtml(wc.DownloadString(Net.ScrapUrl2));
                    }
                    catch
                    {
                        documents[1].LoadHtml(Constants.Empty);
                    }

                    try
                    {
                        documents[2] = new HtmlDocument();
                        documents[2].LoadHtml(wc.DownloadString(Net.ScrapUrl3));
                    }
                    catch
                    {
                        documents[2].LoadHtml(Constants.Empty);
                    }
                }

                foreach (var document in documents)
                {
                    if (document == null)
                        continue;

                    var body = document.DocumentNode.SelectSingleNode("//body");
                    if (body == null)
                        continue;

                    var article_articles = body.SelectNodes("//article");
                    if (article_articles != null)
                        foreach (var article in article_articles)
                            if (article.GetClasses().Contains("article-feed"))
                            {
                                var str = article.InnerHtml.Trim();
                                var split = str.Split("<h2 class=\"overtitle danas\">");
                                if (split.Length > 1)
                                {
                                    var index = split[1].IndexOf("</h2>");
                                    var lead = split[1].Substring(0, index - 1).Trim();
                                    leads.Add(lead);

                                    var regex = "<h1 class=\"title\">(.*?)</h1>";
                                    MatchCollection matches = Regex.Matches(str, regex);
                                    if (matches.Count > 0)
                                        foreach (Match match in matches)
                                        {
                                            var title = match.Groups[1].ToString().Trim();
                                            titles.Add(title);
                                            break;
                                        }

                                    regex = "<a href=\"(.*?)\">";
                                    matches = Regex.Matches(str, regex);
                                    if (matches.Count > 1)
                                    {
                                        var link = matches[1].Groups[1].ToString().Trim();
                                        links.Add(link);
                                    }
                                }
                            }
                }

                if (links.Count == leads.Count && leads.Count == titles.Count)
                    for (int i = 0; i < links.Count; i++)
                    {
                        var article = new Article
                        {
                            ID = links[i].Replace("https://net.hr/danas/hrvatska/", "").TrimEnd('/'),
                            Link = links[i],
                            Title = titles[i],
                            Lead = leads[i]
                        };
                        articles.Add(article);
                    }

                for (int i = 0; i < articles.Length; i++)
                {
                    var article = articles[i];
                    HtmlDocument article_document = null;
                    using (var wc = new WebClient())
                    {
                        try
                        {
                            article_document = new HtmlDocument();
                            article_document.LoadHtml(wc.DownloadString(article.Link));
                        }
                        catch
                        {
                            article_document.LoadHtml(Constants.Empty);
                        }
                    }

                    if (article_document == null)
                        continue;

                    var article_body = article_document.DocumentNode.SelectSingleNode("//body");
                    if (article_body == null)
                        continue;

                    var article_divs = article_body.SelectNodes("//div");
                    if (article_divs != null)
                        foreach (var div in article_divs)
                        {
                            if (div.GetClasses().Contains("metabox"))
                            {
                                string regex = "", str = "";
                                MatchCollection matches = null;
                                try
                                {
                                    str = div.InnerHtml.Trim();
                                    regex = "<span>Autor:(.*?)</span>";
                                    matches = Regex.Matches(str, regex);
                                    if (matches.Count > 0)
                                        foreach (Match match in matches)
                                        {
                                            var author = match.Groups[1].ToString().Trim();
                                            article.Author = author;
                                            break;
                                        }
                                }
                                catch
                                {
                                    article.Author = "exception";
                                }

                                try
                                {
                                    regex = "<span><i class=\"fa fa-clock-o\"></i>(.*?)</span>";
                                    matches = Regex.Matches(str, regex);
                                    if (matches.Count > 0)
                                        foreach (Match match in matches)
                                        {
                                            var time = match.Groups[1].ToString().Trim();
                                            article.Time = time;
                                            break;
                                        }
                                }
                                catch
                                {
                                    article.Time = "exception";
                                }
                            }
                            else if (div.GetClasses().Contains("article-content"))
                            {
                                try
                                {
                                    var str = div.InnerHtml;
                                    var regex = "<h4>(.*?)</h4>";
                                    MatchCollection matches = Regex.Matches(str, regex);
                                    if (matches.Count > 0)
                                        foreach (Match match in matches)
                                        {
                                            var lead = match.Groups[1].ToString().Trim();
                                            article.Lead = lead;
                                            break;
                                        }
                                }
                                catch
                                {
                                    article.Lead = "exception";
                                }

                                try
                                {
                                    var content = "";
                                    var article_ps = div.SelectNodes("//p");
                                    foreach (var p in article_ps)
                                        if (!p.GetClasses().Contains("description") && !p.GetClasses().Contains("undertitle"))
                                        {
                                            var s = p.InnerText;
                                            if (!s.Contains("iframe"))
                                                content += s + "<br><br>";
                                        }

                                    article.Content = content;
                                }
                                catch
                                {
                                    article.Content = "exception";
                                }
                            }
                        }
                }
            }
            return articles;
        }
    }
}