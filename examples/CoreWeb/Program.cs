/*
 * Copyright (c) 2017 Håkan Edling
 *
 * This software may be modified and distributed under the terms
 * of the MIT license.  See the LICENSE file for details.
 * 
 * http://github.com/tidyui/coreweb
 * 
 */

using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace CoreWeb
{
    public class Program
    {
        /// <summary>
        /// Starts the web application.
        /// </summary>
        /// <param name="args">Optional command arguments</param>
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseIISIntegration()
                .UseStartup<Startup>()
                .UseApplicationInsights()
                .Build();
            host.Run();
        }
    }
}
