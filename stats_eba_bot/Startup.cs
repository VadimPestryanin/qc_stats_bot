using System;
using System.Collections.Generic;
using System.Text;
using Aleab.Common.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using QCStats;
using stats_eba_bot.ApiWrapper;
using stats_eba_bot.BotWrapper;
using stats_eba_bot.Cache;
using stats_eba_bot.DataContext;
using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace stats_eba_bot
{
    public class Startup
    {
        protected IServiceProvider _serviceProvider;

        public static IConfiguration Configuration;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            Console.WriteLine($"Current Environment: {Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}");
            
            _serviceProvider = services.BuildServiceProvider();
            services.AddDbContext<PlayersContext>(options => { options.UseSqlite("Data Source=qcStats.db"); });
            services.AddOptions();
            SetupBindings(services);

            _serviceProvider = services.BuildServiceProvider();

            var dataContext = _serviceProvider.GetService<PlayersContext>();
            dataContext.Database.Migrate();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
          
        }


        private void SetupBindings(IServiceCollection services)
        {
            QCStatsAPI.ConfigureServices(services);
            services.AddSingleton(Configuration);
            services.AddScoped<TelegramBotService>();
            services.AddScoped<SqlLiteCacheService>();
            services.AddScoped<QCApiService>();
            services.AddSingleton<IHostedService, TelegramBotHostedService>();


        }
    }
}
