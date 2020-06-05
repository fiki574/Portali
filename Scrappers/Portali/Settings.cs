/*
    Live feed of Croatian public news portals
    Copyright (C) 2020/2021/2021 Bruno Fištrek

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
    public static class H24
    {
        public static readonly string[] URLs = new string[3]
        {
            "https://www.24sata.hr/feeds/aktualno.xml",
            "https://www.24sata.hr/feeds/najnovije.xml",
            "https://www.24sata.hr/feeds/news.xml"
        };

        public static readonly string[] ContentRegex = new string[4]
        {
            "<p>(.*?)</p>",
            "<p style=\"margin:0cm 0cm 10pt; text-align:start; -webkit-text-stroke-width:0px\">(.*?)</p>",
            "<p dir=\"ltr\">(.*?)</p>",
            "<div class=\"article__text\">(.*?)</div>"
        };

        public static readonly string
                BaseUrl1 = "https://www.24sata.hr/",
                BaseUrl2 = "https://www.24sata.hr/news",
                TimeHtml = "datetime=\"",
                ContentHtml = "<div class=\"article__text\">",
                ContentEndHtml = "<footer class=\"article__footer cf\">",
                ArticleListHtml = "<div class=\"row\"><div class=\"ui segment\"><h1>@title@</h1><p>@lead@</p><a target=\"_blank\" href=\"@link@\"><button class=\"ui right floated primary button\">Otvori originalni članak</button></a><a href=\"/articles/24h&id=@article@\"><button class=\"ui right floated secondary button\">Pretpregledaj članak</button></a><br><br><br><br></div></div><br>",
                ArticleHtml = File.ReadAllText("html/templates/24h-article.html"),
                Html = File.ReadAllText("html/portals/24h.html").Replace("@articles@", "Nedostupno na neodređeno vrijeme zbog nedavne promjene dizajna što je pokvarilo kompatibilnost s ovim programom.");
    }

    public static class Index
    {
        public static readonly string
                BaseUrl = "https://www.index.hr",
                ScrapUrl = "https://www.index.hr/najnovije?kategorija=3",
                ArticleListHtml = "<div class=\"row\"><div class=\"ui segment\"><h1>@title@</h1><p>@lead@</p><a target=\"_blank\" href=\"@link@\"><button class=\"ui right floated primary button\">Otvori originalni članak</button></a><a href=\"/articles/index&id=@article@\"><button class=\"ui right floated secondary button\">Pretpregledaj članak</button></a><br><br><br><br></div></div><br>",
                ArticleHtml = File.ReadAllText("html/templates/index-article.html"),
                Html = File.ReadAllText("html/portals/index.html");
    }

    public static class Jutarnji
    {
        public static readonly string
                BaseUrl = "https://www.jutarnji.hr/vijesti/hrvatska",
                ScrapUrl1 = "https://www.jutarnji.hr/vijesti/hrvatska/?page=1",
                ScrapUrl2 = "https://www.jutarnji.hr/vijesti/hrvatska/?page=2",
                ScrapUrl3 = "https://www.jutarnji.hr/vijesti/hrvatska/?page=3",
                ArticleListHtml = "<div class=\"row\"><div class=\"ui segment\"><h1>@title@</h1><p>@lead@</p><a target=\"_blank\" href=\"@link@\"><button class=\"ui right floated primary button\">Otvori originalni članak</button></a><a href=\"/articles/jutarnji&id=@article@\"><button class=\"ui right floated secondary button\">Pretpregledaj članak</button></a><br><br><br><br></div></div><br>",
                ArticleHtml = File.ReadAllText("html/templates/jutarnji-article.html"),
                Html = File.ReadAllText("html/portals/jutarnji.html");
    }

    public static class Vecernji
    {
        public static readonly string
                BaseUrl = "https://www.vecernji.hr",
                ScrapUrl = "https://www.vecernji.hr/najnovije-vijesti/?page=",
                ArticleListHtml = "<div class=\"row\"><div class=\"ui segment\"><h1>@title@</h1><p>@lead@</p><a target=\"_blank\" href=\"@link@\"><button class=\"ui right floated primary button\">Otvori originalni članak</button></a><a href=\"/articles/vecernji&id=@article@\"><button class=\"ui right floated secondary button\">Pretpregledaj članak</button></a><br><br><br><br></div></div><br>",
                ArticleHtml = File.ReadAllText("html/templates/vecernji-article.html"),
                Html = File.ReadAllText("html/portals/vecernji.html");
    }

    public static class Net
    {
        public static readonly string
                BaseUrl = "https://www.net.hr",
                ScrapUrl1 = "https://www.net.hr/kategorija/danas/hrvatska/",
                ScrapUrl2 = "https://www.net.hr/kategorija/danas/hrvatska/page/2/",
                ScrapUrl3 = "https://www.net.hr/kategorija/danas/hrvatska/page/3/",
                ArticleListHtml = "<div class=\"row\"><div class=\"ui segment\"><h1>@title@</h1><p>@lead@</p><a target=\"_blank\" href=\"@link@\"><button class=\"ui right floated primary button\">Otvori originalni članak</button></a><a href=\"/articles/net&id=@article@\"><button class=\"ui right floated secondary button\">Pretpregledaj članak</button></a><br><br><br><br></div></div><br>",
                ArticleHtml = File.ReadAllText("html/templates/net-article.html"),
                Html = File.ReadAllText("html/portals/net.html");
    }
}