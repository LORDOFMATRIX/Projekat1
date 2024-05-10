using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Diagnostics;
using System.IO;

namespace Gif_Server
{
    internal class Server
    {
       // private static Dictionary<string, byte[]> cache = new Dictionary<string, byte[]>();
        private static readonly object count_lock = new object();
        public static HttpListener listener;
        public static string url = "http://localhost:8000/";
        private static int requestCount = 0;


        public Server()
        {
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            Console.WriteLine("Listening for connections on {0}", url);
        }
        public static void HandleRequest(object instance)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            HttpListenerContext context = (HttpListenerContext)instance;
            HttpListenerRequest request = context.Request;
            HttpListenerResponse response = context.Response;
            if (request.Url.AbsolutePath == "/favicon.ico")
            {
                return;
            }
            lock (count_lock) { 
            Console.WriteLine("Request broj: {0}", ++requestCount);
            }
            Console.WriteLine(request.Url.ToString());
            Console.WriteLine(request.HttpMethod);
            Console.WriteLine(request.UserHostName);
            Console.WriteLine();


           // Console.WriteLine("path je: " + request.Url.AbsolutePath);

                

            if (request.Url.AbsolutePath.Length < 4 &&
               (request.Url.AbsolutePath.Length > 5 && request.Url.AbsolutePath.Substring(request.Url.AbsolutePath.Length - 4) != ".gif"))
            {
                ReturnBadRequest(response);

            }
            else
            {
                try
                {
                    // Thread.Sleep(1000);
                    string location = request.Url.AbsolutePath.Substring(1); //path bez '/'
                    byte[] file = null;
                    if (((file = Cache.ReadFromCache(location)) == null)) 
                    {

                        var files = Directory.EnumerateFiles(@"C:\Users\Branko\source\repos\Gif Server\Gif Server\Slike", location, SearchOption.AllDirectories);
                        file = File.ReadAllBytes(files.First());
                        Cache.WriteToCache(location, file);
                    }
                    Console.WriteLine("vreme potrebno za citanje fajla: " + stopwatch.ElapsedMilliseconds + " milisekundi");


                    StringBuilder sbHeader = new StringBuilder();
                    sbHeader.AppendLine("HTTP/1.1 200 OK");
                    sbHeader.AppendLine("Content-Length: " + file.Length);
                    sbHeader.AppendLine();

                    response.ContentType = "image/gif";
                    response.ContentEncoding = Encoding.ASCII;
                    response.ContentLength64 = file.LongLength;
                    Console.WriteLine();
                    response.OutputStream.Write(file, 0, file.Length);

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    ReturnBadRequest(response);
                }
                finally
                {
                    stopwatch.Stop();
                    Console.WriteLine("Potrebno vreme je: " + stopwatch.ElapsedMilliseconds + " milisekunde");
                }
            }

        }
        public static void ReturnBadRequest(HttpListenerResponse response)
        {
            byte[] data = Encoding.UTF8.GetBytes("<h1>Nepostojeca slika</h1>");
            response.ContentType = "text/html";
            response.ContentEncoding = Encoding.UTF8;
            response.ContentLength64 = data.LongLength;
            response.OutputStream.Write(data, 0, data.Length);

        }
    }
}
