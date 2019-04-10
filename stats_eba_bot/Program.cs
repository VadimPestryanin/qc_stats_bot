using QCStats;
using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac; 
using Autofac.Extensions.DependencyInjection;
using QCStats.Model.QC;
using Telegram.Bot;
using Telegram.Bot.Args;

using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace stats_eba_bot
{

    public class Program
    {
        public static void Main(string[] args)
        {
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
                
    }
}
