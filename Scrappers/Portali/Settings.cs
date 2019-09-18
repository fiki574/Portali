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
                ContentEndHtml = "<footer class=\"article__footer cf\">";
    }

    public static class Index
    {
    }

    public static class Vecernji
    {
    }

    public static class Jutarnji
    {
    }
}