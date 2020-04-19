/*
    Live feed of Croatian public news portals
    Copyright (C) 2020 Bruno Fištrek

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
using System.Text;
using System.Reflection;
using System.Threading;
using System.Collections.Generic;

namespace Portals
{
    public partial class HttpServer
    {
        private delegate string HttpHandlerDelegate(HttpServer server, HttpListenerRequest request, Dictionary<string, string> parameters);
        private Dictionary<string, KeyValuePair<HttpHandler, HttpHandlerDelegate>> m_handlers = new Dictionary<string, KeyValuePair<HttpHandler, HttpHandlerDelegate>>();
        private HttpListener Listener;

        public HttpServer(int port)
        {
            try
            {
                MapHandlers();
                Listener = new HttpListener();
                Listener.Prefixes.Add("http://*:" + port + "/");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void Start()
        {
            try
            {
                Listener.Start();
                Listener.BeginGetContext(OnGetContext, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public void Stop()
        {
            try
            {
                Listener.Close();
                Listener = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void MapHandlers()
        {
            try
            {
                foreach (MethodInfo methodInfo in typeof(HttpServer).GetMethods(BindingFlags.NonPublic | BindingFlags.Static))
                {
                    var attributes = methodInfo.GetCustomAttributes(typeof(HttpHandler), false);
                    if (attributes.Length < 1)
                        continue;

                    HttpHandler attribute = (HttpHandler)attributes[0];
                    if (m_handlers.ContainsKey(attribute.Url))
                        continue;

                    m_handlers.Add(attribute.Url, new KeyValuePair<HttpHandler, HttpHandlerDelegate>(attribute, (HttpHandlerDelegate)Delegate.CreateDelegate(typeof(HttpHandlerDelegate), methodInfo)));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void OnGetContext(IAsyncResult result)
        {
            try
            {
                var context = Listener.EndGetContext(result);
                ThreadPool.QueueUserWorkItem(HandleRequest, context);
            }
            finally
            {
                Listener.BeginGetContext(OnGetContext, null);
            }
        }

        private void HandleRequest(object oContext)
        {
            HttpListenerContext context = (HttpListenerContext)oContext;
            try
            {
                Dictionary<string, string> parameters = new Dictionary<string, string>();
                string[] tokens = context.Request.RawUrl.Split('&');
                foreach (var token in tokens)
                {
                    string[] keyValuePair = token.Split('=');
                    if (keyValuePair.Length != 2)
                        continue;

                    var key = WebUtility.UrlDecode(keyValuePair[0]);
                    var value = WebUtility.UrlDecode(keyValuePair[1]);
                    parameters.Add(key, value);
                }

                string[] raw = context.Request.RawUrl.Split('&');
                if (raw[0] == "/favicon.ico")
                    return;

                if (!m_handlers.TryGetValue(raw[0], out KeyValuePair<HttpHandler, HttpHandlerDelegate> pair))
                    return;

                var result = pair.Value(this, context.Request, parameters);
                if (result == null)
                    return;

                context.Response.ContentType = "text/html";
                context.Response.ContentEncoding = Encoding.UTF8;

                using (StreamWriter writer = new StreamWriter(context.Response.OutputStream, context.Response.ContentEncoding))
                    writer.Write(result);
            }
            finally
            {
                context.Response.Close();
            }
        }
    }
}