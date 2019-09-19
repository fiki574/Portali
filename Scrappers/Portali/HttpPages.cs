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
        private static string HandleHome1(HttpServer server, HttpListenerRequest request, Dictionary<string, string> parameters)
        {
            return HandleHome2(server, request, parameters);
        }

        [HttpHandler("/index.html")]
        private static string HandleHome2(HttpServer server, HttpListenerRequest request, Dictionary<string, string> parameters)
        {
            return Constants.Homepage;
        }

        [HttpHandler("/portals/24h.html")]
        private static string Handle24h(HttpServer server, HttpListenerRequest request, Dictionary<string, string> parameters)
        {
            try
            {
                var articles = "";
                if (!Program.IsScrapping24h)
                    Program._24h.ForEach(a =>
                    {
                        var article = _24h.ArticleListHtml.Replace("@portal@", "https://www.24sata.hr/").Replace("@image@", Constants.ImageData.Replace("@base64@", Utilities.GetBase64ForImage("html/images/24h.png"))).Replace("@title@", a.Title).Replace("@lead@", a.Lead).Replace("@link@", a.Link).Replace("@article@", a.ID);
                        articles += article;
                    });
                else
                    return "Trenutno traje dohvaćanje članaka sa 24sata, ubrzo će biti dostupni. Osvježite ovu stranicu kro maksimalno jednu minutu.";

                return _24h.Html.Replace("@articles@", articles);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return "Greška: " + ex.ToString();
            }
        }

        [HttpHandler("/portals/index.html")]
        private static string HandleIndex(HttpServer server, HttpListenerRequest request, Dictionary<string, string> parameters)
        {
            return Index.Html;
        }

        [HttpHandler("/portals/jutarnji.html")]
        private static string HandleJutarnji(HttpServer server, HttpListenerRequest request, Dictionary<string, string> parameters)
        {
            return Jutarnji.Html;
        }

        [HttpHandler("/portals/vecernji.html")]
        private static string HandlePregled(HttpServer server, HttpListenerRequest request, Dictionary<string, string> parameters)
        {
            return Vecernji.Html;
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
                return "Greška: " + ex.ToString();
            }
        }
    }
}