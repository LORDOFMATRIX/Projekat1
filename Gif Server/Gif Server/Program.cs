using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net;
using System.IO;
using System.Diagnostics;

namespace Gif_Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            new Server();   

            while (Server.listener.IsListening)
            {
                ThreadPool.QueueUserWorkItem(Server.HandleRequest,Server.listener.GetContext());
            }



        }

        

    }
}
