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

        [HttpHandler("/visits")]
        private static string HandleVisits(HttpServer server, HttpListenerRequest request, Dictionary<string, string> parameters)
        {
            return server.GetVisits().ToString();
        }

        [HttpHandler("/portals/24h.html")]
        private static string Handle24h(HttpServer server, HttpListenerRequest request, Dictionary<string, string> parameters)
        {
            try
            {
                var articles = "";
                Program._24h.ForEach(a =>
                    {
                        var article = _24h.ArticleListHtml.Replace("@portal@", "https://www.24sata.hr/").Replace("@title@", a.Title).Replace("@lead@", a.Lead).Replace("@link@", a.Link).Replace("@article@", a.ID);
                        articles += article;
                    });
                return _24h.Html.Replace("@articles@", articles);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "Greška";
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
                    var article = Index.ArticleListHtml.Replace("@portal@", "https://www.index.hr/").Replace("@title@", a.Title).Replace("@lead@", a.Lead).Replace("@link@", a.Link).Replace("@article@", a.ID);
                    articles += article;
                });
                return Index.Html.Replace("@articles@", articles);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "Greška";
            }
        }

        [HttpHandler("/portals/jutarnji.html")]
        private static string HandleJutarnji(HttpServer server, HttpListenerRequest request, Dictionary<string, string> parameters)
        {
            return Jutarnji.Html;
        }

        [HttpHandler("/portals/vecernji.html")]
        private static string HandleVecernji(HttpServer server, HttpListenerRequest request, Dictionary<string, string> parameters)
        {
            return Vecernji.Html;
        }

        [HttpHandler("/portals/dnevnik.html")]
        private static string HandleDnevnik(HttpServer server, HttpListenerRequest request, Dictionary<string, string> parameters)
        {
            return Dnevnik.Html;
        }

        [HttpHandler("/portals/net.html")]
        private static string HandleNet(HttpServer server, HttpListenerRequest request, Dictionary<string, string> parameters)
        {
            return Net.Html;
        }

        [HttpHandler("/articles/24h")]
        private static string Handle24hArticle(HttpServer server, HttpListenerRequest request, Dictionary<string, string> parameters)
        {
            try
            {
                if (!parameters.ContainsKey("id"))
                    return "Neispravan zahtjev";

                var id = parameters["id"];
                var html = File.ReadAllText($"html/articles/24h/{id}.html");
                return html;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "Greška";
            }
        }

        [HttpHandler("/articles/index")]
        private static string HandleIndexArticle(HttpServer server, HttpListenerRequest request, Dictionary<string, string> parameters)
        {
            try
            {
                if (!parameters.ContainsKey("id"))
                    return "Neispravan zahtjev";

                var id = parameters["id"];
                var html = File.ReadAllText($"html/articles/index/{id}.html");
                return html;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "Greška";
            }
        }

        [HttpHandler("/articles/jutarnji")]
        private static string HandleJutarnjiArticle(HttpServer server, HttpListenerRequest request, Dictionary<string, string> parameters)
        {
            try
            {
                if (!parameters.ContainsKey("id"))
                    return "Neispravan zahtjev";

                var id = parameters["id"];
                var html = File.ReadAllText($"html/articles/jutarnji/{id}.html");
                return html;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "Greška";
            }
        }

        [HttpHandler("/articles/vecernji")]
        private static string HandleVecernjiArticle(HttpServer server, HttpListenerRequest request, Dictionary<string, string> parameters)
        {
            try
            {
                if (!parameters.ContainsKey("id"))
                    return "Neispravan zahtjev";

                var id = parameters["id"];
                var html = File.ReadAllText($"html/articles/vecernji/{id}.html");
                return html;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "Greška";
            }
        }

        [HttpHandler("/articles/dnevnik")]
        private static string HandleDnevnikArticle(HttpServer server, HttpListenerRequest request, Dictionary<string, string> parameters)
        {
            try
            {
                if (!parameters.ContainsKey("id"))
                    return "Neispravan zahtjev";

                var id = parameters["id"];
                var html = File.ReadAllText($"html/articles/dnevnik/{id}.html");
                return html;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "Greška";
            }
        }

        [HttpHandler("/articles/net")]
        private static string HandleNetArticle(HttpServer server, HttpListenerRequest request, Dictionary<string, string> parameters)
        {
            try
            {
                if (!parameters.ContainsKey("id"))
                    return "Neispravan zahtjev";

                var id = parameters["id"];
                var html = File.ReadAllText($"html/articles/net/{id}.html");
                return html;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "Greška";
            }
        }
    }
}