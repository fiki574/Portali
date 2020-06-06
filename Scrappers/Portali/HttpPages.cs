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

using System.IO;
using System.Net;
using System.Collections.Generic;

namespace Portals
{
    public partial class HttpServer
    {
        [HttpHandler("/")]
        private static string HandleBlank(HttpServer server, HttpListenerRequest request, Dictionary<string, string> parameters)
        {
            return HandleHome(server, request, parameters);
        }

        [HttpHandler("/index.html")]
        private static string HandleHome(HttpServer server, HttpListenerRequest request, Dictionary<string, string> parameters)
        {
            return Constants.Homepage;
        }

        [HttpHandler("/portals/24h.html")]
        private static string Handle24h(HttpServer server, HttpListenerRequest request, Dictionary<string, string> parameters)
        {
            try
            {
                var articles = "";
                Program.H24.ForEach(a =>
                {
                    var article = H24.ArticleListHtml.Replace("@title@", a.Title).Replace("@lead@", a.Lead).Replace("@link@", a.Link).Replace("@article@", a.ID);
                    articles += article;
                });
                if (Program.H24.Count(a => true) == 0)
                    articles = "Nema članaka za prikaz.";
                return H24.Html.Replace("@articles@", articles);
            }
            catch
            {
                return Constants.Redirect.Replace("@redurl@", "https://portali.bzg.com.hr");
            }
        }

        [HttpHandler("/portals/index.html")]
        private static string HandleIndex(HttpServer server, HttpListenerRequest request, Dictionary<string, string> parameters)
        {
            try
            {
                var articles = "";
                Program.Index.ForEach(a =>
                {
                    var article = Index.ArticleListHtml.Replace("@title@", a.Title).Replace("@lead@", a.Lead).Replace("@link@", a.Link).Replace("@article@", a.ID);
                    articles += article;
                });
                if (Program.Index.Count(a => true) == 0)
                    articles = "Nema članaka za prikaz.";
                return Index.Html.Replace("@articles@", articles);
            }
            catch
            {
                return Constants.Redirect.Replace("@redurl@", "https://portali.bzg.com.hr");
            }
        }

        [HttpHandler("/portals/jutarnji.html")]
        private static string HandleJutarnji(HttpServer server, HttpListenerRequest request, Dictionary<string, string> parameters)
        {
            try
            {
                var articles = "";
                Program.Jutarnji.ForEach(a =>
                {
                    var article = Jutarnji.ArticleListHtml.Replace("@title@", a.Title).Replace("@lead@", a.Lead).Replace("@link@", a.Link).Replace("@article@", a.ID);
                    articles += article;
                });
                if (Program.Jutarnji.Count(a => true) == 0)
                    articles = "Nema članaka za prikaz.";
                return Jutarnji.Html.Replace("@articles@", articles);
            }
            catch
            {
                return Constants.Redirect.Replace("@redurl@", "https://portali.bzg.com.hr");
            }
        }

        [HttpHandler("/portals/vecernji.html")]
        private static string HandleVecernji(HttpServer server, HttpListenerRequest request, Dictionary<string, string> parameters)
        {
            try
            {
                var articles = "";
                Program.Vecernji.ForEach(a =>
                {
                    var article = Vecernji.ArticleListHtml.Replace("@title@", a.Title).Replace("@lead@", a.Lead).Replace("@link@", a.Link).Replace("@article@", a.ID);
                    articles += article;
                });
                if (Program.Vecernji.Count(a => true) == 0)
                    articles = "Nema članaka za prikaz.";
                return Vecernji.Html.Replace("@articles@", articles);
            }
            catch
            {
                return Constants.Redirect.Replace("@redurl@", "https://portali.bzg.com.hr");
            }
        }

        [HttpHandler("/portals/net.html")]
        private static string HandleNet(HttpServer server, HttpListenerRequest request, Dictionary<string, string> parameters)
        {
            try
            {
                var articles = "";
                Program.Net.ForEach(a =>
                {
                    var article = Net.ArticleListHtml.Replace("@title@", a.Title).Replace("@lead@", a.Lead).Replace("@link@", a.Link).Replace("@article@", a.ID);
                    articles += article;
                });
                if (Program.Net.Count(a => true) == 0)
                    articles = "Nema članaka za prikaz.";
                return Net.Html.Replace("@articles@", articles);
            }
            catch
            {
                return Constants.Redirect.Replace("@redurl@", "https://portali.bzg.com.hr");
            }
        }

        [HttpHandler("/articles/24h")]
        private static string Handle24hArticle(HttpServer server, HttpListenerRequest request, Dictionary<string, string> parameters)
        {
            var id = "";
            try
            {
                if (!parameters.ContainsKey("id"))
                    return "Neispravan zahtjev";

                id = parameters["id"];
                var html = File.ReadAllText($"html/articles/24h/{id}.html");
                return html;
            }
            catch
            {
                return Constants.Redirect.Replace("@redurl@", "https://portali.bzg.com.hr/portals/24h.html");
            }
        }

        [HttpHandler("/articles/index")]
        private static string HandleIndexArticle(HttpServer server, HttpListenerRequest request, Dictionary<string, string> parameters)
        {
            var id = "";
            try
            {
                if (!parameters.ContainsKey("id"))
                    return "Neispravan zahtjev";

                id = parameters["id"];
                var html = File.ReadAllText($"html/articles/index/{id}.html");
                return html;
            }
            catch
            {
                return Constants.Redirect.Replace("@redurl@", "https://portali.bzg.com.hr/portals/index.html");
            }
        }

        [HttpHandler("/articles/jutarnji")]
        private static string HandleJutarnjiArticle(HttpServer server, HttpListenerRequest request, Dictionary<string, string> parameters)
        {
            var id = "";
            try
            {
                if (!parameters.ContainsKey("id"))
                    return "Neispravan zahtjev";

                id = parameters["id"];
                var html = File.ReadAllText($"html/articles/jutarnji/{id}.html");
                return html;
            }
            catch
            {
                return Constants.Redirect.Replace("@redurl@", "https://portali.bzg.com.hr/portals/jutarnji.html");
            }
        }

        [HttpHandler("/articles/vecernji")]
        private static string HandleVecernjiArticle(HttpServer server, HttpListenerRequest request, Dictionary<string, string> parameters)
        {
            var id = "";
            try
            {
                if (!parameters.ContainsKey("id"))
                    return "Neispravan zahtjev";

                id = parameters["id"];
                var html = File.ReadAllText($"html/articles/vecernji/{id}.html");
                return html;
            }
            catch
            {
                return Constants.Redirect.Replace("@redurl@", "https://portali.bzg.com.hr/portals/vecernji.html");
            }
        }

        [HttpHandler("/articles/net")]
        private static string HandleNetArticle(HttpServer server, HttpListenerRequest request, Dictionary<string, string> parameters)
        {
            var id = "";
            try
            {
                if (!parameters.ContainsKey("id"))
                    return "Neispravan zahtjev";

                id = parameters["id"];
                var html = File.ReadAllText($"html/articles/net/{id}.html");
                return html;
            }
            catch
            {
                return Constants.Redirect.Replace("@redurl@", "https://portali.bzg.com.hr/portals/jutarnji.html");
            }
        }
    }
}