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

namespace Portals
{
    public class Program
    {
        private static Thread[] Threads = null;
        private static HttpServer Server = null;
        private static bool IsRunning = false;
        public static ThreadSafeList<Article>
            H24 = new ThreadSafeList<Article>(),
            Index = new ThreadSafeList<Article>(),
            Jutarnji = new ThreadSafeList<Article>(),
            Vecernji = new ThreadSafeList<Article>(),
            Net = new ThreadSafeList<Article>();

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
                    Thread.Sleep(Constants.MainThreadSleepInterval);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                IsRunning = false;
            }
            finally
            {
                if (!IsRunning)
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

            if (args != null && args.Length == 1 && args[0].Equals("--restart-always"))
            {
                Console.WriteLine("Restarting...");
                Main(args);
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
                Console.WriteLine(ex.ToString());
                IsRunning = false;
            }
        }

        static void Scrappers()
        {
            Console.WriteLine("'Scrappers' thread started");
            try
            {
                while (IsRunning)
                {
                    var time = Utilities.StopWatch(() =>
                    {
                        //if (!Utilities.TryCatch(() => H24 = Utilities.CreateList(PortalType.H24)))
                            //Console.WriteLine("Exception occured while scrapping 24sata.hr");

                        if (!Utilities.TryCatch(() => Index = Utilities.CreateList(PortalType.Index)))
                            Console.WriteLine("Exception occured while scrapping index.hr");

                        if (!Utilities.TryCatch(() => Jutarnji = Utilities.CreateList(PortalType.Jutarnji)))
                            Console.WriteLine("Exception occured while scrapping jutarnji.hr");

                        if (!Utilities.TryCatch(() => Vecernji = Utilities.CreateList(PortalType.Vecernji)))
                            Console.WriteLine("Exception occured while scrapping vecernji.hr");

                        if (!Utilities.TryCatch(() => Net = Utilities.CreateList(PortalType.Net)))
                            Console.WriteLine("Exception occured while scrapping net.hr");
                    });
                    var count = H24.Length + Index.Length + Jutarnji.Length + Vecernji.Length + Net.Length;
                    Console.WriteLine($"Elapsed time: {time} | Total articles: {count}");
                    Thread.Sleep(Constants.ScrappersSleepInterval);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                IsRunning = false;
            }
        }
    }
}