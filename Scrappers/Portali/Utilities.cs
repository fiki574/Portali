﻿/*
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
using System.IO;

namespace Portals
{
    public static class Utilities
    {
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

        public static ThreadSafeList<Article> CreateList(PortalType type)
        {
            ThreadSafeList<Article> articles = new ThreadSafeList<Article>();
            string path = "", portal = "";
            if (type == PortalType.H24)
            {
                path = "html/articles/24h";
                portal = "24sata.hr";
            }
            else if (type == PortalType.Index)
            {
                path = "html/articles/index";
                portal = "index.hr";
            }
            else if (type == PortalType.Jutarnji)
            {
                path = "html/articles/jutarnji";
                portal = "jutarnji.hr";
            }
            else if (type == PortalType.Vecernji)
            {
                path = "html/articles/vecernji";
                portal = "vecernji.hr";
            }
            else if (type == PortalType.Net)
            {
                path = "html/articles/net";
                portal = "net.hr";
            }

            ClearDirectory(path);
            articles.Clear();
            articles = HapScrap.ScrapPortal(type);
            Console.WriteLine($"Scrapped {portal} -> Total articles: {articles.Count(a => true)}");
            articles = UpdateList(articles, type);
            articles.ForEach(a =>
            {
                try
                {
                    File.WriteAllText(path + "/" + a.ID + ".html", a.ToHtml(type));
                }
                catch
                {
                }
            });
            return articles;
        }

        private static ThreadSafeList<Article> UpdateList(ThreadSafeList<Article> articles, PortalType type)
        {
            ThreadSafeList<Article> updated = new ThreadSafeList<Article>();
            articles.ForEach(a =>
            {
                a.ReplaceInvalidText();
                if (a.ShouldBeDisplayed(type))
                    updated.Add(a);
            });
            Console.WriteLine($"Filtering out {articles.Length - updated.Length} articles that shouldn't be displayed");
            return updated;
        }
    }
}