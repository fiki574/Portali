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
using System.Threading;
using System.Diagnostics;

namespace Portals
{
    public class Program
    {
        private static Thread[] Threads = null;
        private static HttpServer Server = null;
        private static bool IsRunning = false;
        public static ThreadSafeList<Article> H24 = null, Index = null, Jutarnji = null, Vecernji = null, Net = null;

        static void Main(string[] args)
        {
            try
            {
                H24 = new ThreadSafeList<Article>();
                Index = new ThreadSafeList<Article>();
                Jutarnji = new ThreadSafeList<Article>();
                Vecernji = new ThreadSafeList<Article>();
                Net = new ThreadSafeList<Article>();

                Threads = new Thread[2];
                IsRunning = true;
                Console.WriteLine("Starting the 'Scrappers' and 'WebServer' threads");

                Threads[0] = new Thread(WebServer);
                Threads[0].Start();

                Threads[1] = new Thread(Scrappers);
                Threads[1].Start();

                while (IsRunning)
                    Thread.Sleep(Constants.MainThreadSleepInterval);
            }
            catch (Exception ex)
            {
                IsRunning = false;
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.WriteLine("IsRunning is false, stopping everything");
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
                    Stopwatch sw = new Stopwatch();
                    sw.Start();

                    Scrap24h();
                    ScrapIndex();
                    ScrapJutarnji();
                    ScrapVecernji();
                    ScrapNet();

                    sw.Stop();
                    var ts = sw.Elapsed;
                    var time = ts.ToString("mm\\:ss\\.ff");
                    var count = H24.Count(a => true) + Index.Count(a => true) + Jutarnji.Count(a => true) + Vecernji.Count(a => true) + Net.Count(a => true);

                    Console.WriteLine($"Elapsed time: {time} | Total articles: {count}");
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
                Utilities.ClearDirectory("html/articles/24h");
                H24.Clear();
                H24 = HapScrap.ScrapPortal(PortalType.H24);
                Console.WriteLine($"Scrapped 24sata.hr -> Total articles: {H24.Count(a => true)}");
                Utilities.UpdateList(H24, PortalType.H24);
                H24.ForEach(a => File.WriteAllText("html/articles/24h/" + a.ID + ".html", a.ToHtml(PortalType.H24)));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        static void ScrapIndex()
        {
            try
            {
                Utilities.ClearDirectory("html/articles/index");
                Index.Clear();
                Index = HapScrap.ScrapPortal(PortalType.Index);
                Console.WriteLine($"Scrapped index.hr -> Total articles: {Index.Count(a => true)}");
                Utilities.UpdateList(Index, PortalType.Index);
                Index.ForEach(a => File.WriteAllText("html/articles/index/" + a.ID + ".html", a.ToHtml(PortalType.Index)));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        static void ScrapJutarnji()
        {
            try
            {
                Utilities.ClearDirectory("html/articles/jutarnji");
                Jutarnji.Clear();
                Jutarnji = HapScrap.ScrapPortal(PortalType.Jutarnji);
                Console.WriteLine($"Scrapped jutarnji.hr -> Total articles: {Jutarnji.Count(a => true)}");
                Utilities.UpdateList(Jutarnji, PortalType.Jutarnji);
                Jutarnji.ForEach(a => File.WriteAllText("html/articles/jutarnji/" + a.ID + ".html", a.ToHtml(PortalType.Jutarnji)));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        static void ScrapVecernji()
        {
            try
            {
                Utilities.ClearDirectory("html/articles/vecernji");
                Vecernji.Clear();
                Vecernji = HapScrap.ScrapPortal(PortalType.Vecernji);
                Console.WriteLine($"Scrapped vecernji.hr -> Total articles: {Vecernji.Count(a => true)}");
                Utilities.UpdateList(Vecernji, PortalType.Vecernji);
                Vecernji.ForEach(a => File.WriteAllText("html/articles/vecernji/" + a.ID + ".html", a.ToHtml(PortalType.Vecernji)));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        static void ScrapNet()
        {
            try
            {
                Utilities.ClearDirectory("html/articles/net");
                Net.Clear();
                Net = HapScrap.ScrapPortal(PortalType.Net);
                Console.WriteLine($"Scrapped net.hr -> Total articles: {Net.Count(a => true)}");
                Utilities.UpdateList(Net, PortalType.Net);
                Net.ForEach(a => File.WriteAllText("html/articles/net/" + a.ID + ".html", a.ToHtml(PortalType.Net)));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}