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

        public static void UpdateList(ThreadSafeList<Article> articles, PortalType type)
        {
            ThreadSafeList<string> remove = new ThreadSafeList<string>();
            articles.ForEach(a =>
            {
                a.ReplaceInvalidText();
                if (!a.ShouldBeDisplayed(type))
                    remove.Add(a.ID);
            });
            remove.ForEach(s => articles.Remove(a => a.ID == s));
            Console.WriteLine($"Filtering out {remove.Count(s => true)} articles that shouldn't be displayed");
        }
    }
}