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

using System.IO;

namespace Portals
{
    public static class _24h
    {
        public static readonly string[] URLs = new string[3]
        {
            "https://www.24sata.hr/feeds/aktualno.xml",
            "https://www.24sata.hr/feeds/najnovije.xml",
            "https://www.24sata.hr/feeds/news.xml"
        };

        public static readonly string
                BaseUrl1 = "https://www.24sata.hr/",
                BaseUrl2 = "https://www.24sata.hr/news",
                TitleHtml = "<h1 class=\"article__title\">",
                LeadHtml = "<h2 class=\"article__lead\">",
                AuthorHtml = "<span class=\"article__author \">",
                TimeHtml = "datetime=\"",
                ContentHtml = "<div class=\"article__text\">",
                ContentEndHtml = "<footer class=\"article__footer cf\">",
                ArticleListHtml = "<div class=\"row\"><div class=\"ui segment\"><!--<a target=\"_blank\" href=\"@portal@\"><img class=\"ui left floated image\" src=\"@image@\" style=\"height: 128px; width: 128px;\"/></a>--><h1>@title@</h1><p>@lead@</p><a target=\"_blank\" href=\"@link@\"><button class=\"ui right floated primary button\">Otvori originalni članak</button></a><a href=\"/articles/24h&id=@article@\"><button class=\"ui right floated secondary button\">Pretpregledaj članak</button></a><br><br><br><br></div></div><br>",
                ArticleHtml = File.ReadAllText("html/templates/24h-article.html"),
                Html = File.ReadAllText("html/portals/24h.html");
    }

    public static class Index
    {
        public static readonly string 
                Html = File.ReadAllText("html/portals/index.html").Replace("@articles@", "<i>Trenutno nedostupno</i>");
    }

    public static class Jutarnji
    {
        public static readonly string 
                Html = File.ReadAllText("html/portals/jutarnji.html").Replace("@articles@", "<i>Trenutno nedostupno</i>");
    }

    public static class Vecernji
    {
        public static readonly string 
                Html = File.ReadAllText("html/portals/vecernji.html").Replace("@articles@", "<i>Trenutno nedostupno</i>");
    }
}