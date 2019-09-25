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
using System.Threading;

namespace Portals
{
    public class Program
    {
        private static Thread[] Threads = null;
        private static HttpServer Server = null;
        private static bool IsRunning = false;
        public static ThreadSafeList<Article> _24h = null;
        public static bool IsScrapping24h = false;

        static void Main(string[] args)
        {
            try
            {
                Threads = new Thread[2];
                IsRunning = true;
                Console.WriteLine("Starting the 'Scrappers' and 'WebServer' threads");

                Threads[0] = new Thread(WebServer);
                Threads[0].Start();

                Threads[1] = new Thread(Scrappers);
                Threads[1].Start();

                while (IsRunning)
                {
                    Thread.Sleep(Constants.MainThreadSleepInterval);
                    Console.WriteLine($"Total requests since application boot: {HttpServer.RequestCount}");
                }
            }
            catch (Exception ex)
            {
                IsRunning = false;
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.WriteLine("IsRunning is false, stopping everything");
                _24h = null;
                Server.Stop();
                Server = null;
                Threads[0].Join();
                Threads[1].Join();
                Threads = null;
                Console.WriteLine("Stopped everything");
            }
        }

        static void Scrappers()
        {
            Console.WriteLine("'Scrappers' thread started");
            try
            {
                while (IsRunning)
                {
                    Scrap24h();
                    Thread.Sleep(Constants.ScrappersSleepInterval);
                }
            }
            catch (Exception ex)
            {
                IsRunning = false;
                Console.WriteLine(ex.ToString());
            }
        }

        static void WebServer()
        {
            Console.WriteLine("'WebServer' thread started");
            try
            {
                Server = new HttpServer(Constants.HttpServerPort);
                Server.Start();
            }
            catch (Exception ex)
            {
                IsRunning = false;
                Console.WriteLine(ex.ToString());
            }
        }

        static void Scrap24h()
        {
            try
            {
                IsScrapping24h = true;
                _24h = Utilities.Scrap24h();
                Console.WriteLine($"Scrapped 24sata.hr -> Total articles: {_24h.Count(a => true)}");
                Utilities.ClearDirectory("html/articles/24h");
                ThreadSafeList<string> remove = new ThreadSafeList<string>();
                _24h.ForEach(a => 
                {
                    a.ReplaceInvalidText();
                    if (!a.ShouldBeDisplayed())
                        remove.Add(a.ID);
                });
                remove.ForEach(s => { _24h.Remove(a => a.ID == s); });
                Console.WriteLine($"Filtering out {remove.Count(s => true)} articles that shouldn't be displayed");
                remove.Clear();
                _24h.ForEach(a => { File.WriteAllText("html/articles/24h/" + a.ID + ".html", a.ToHtml()); });
                IsScrapping24h = false;
            }
            catch (Exception ex)
            {
                IsRunning = false;
                Console.WriteLine(ex.ToString());
            }
        }
    }
}