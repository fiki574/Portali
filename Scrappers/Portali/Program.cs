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

        static void Scrappers()
        {
            Console.WriteLine("'Scrappers' thread started");
            try
            {
                while (IsRunning)
                {
                    Stopwatch sw = new Stopwatch();
                    sw.Start();

                    H24 = Utilities.CreateList(PortalType.H24);
                    Index = Utilities.CreateList(PortalType.Index);
                    Jutarnji = Utilities.CreateList(PortalType.Jutarnji);
                    Vecernji = Utilities.CreateList(PortalType.Vecernji);
                    Net = Utilities.CreateList(PortalType.Net);

                    sw.Stop();
                    var ts = sw.Elapsed;
                    var time = ts.ToString("mm\\:ss");
                    var count = H24.Length + Index.Length + Jutarnji.Length + Vecernji.Length + Net.Length;

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
    }
}