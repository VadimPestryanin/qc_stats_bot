using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace stats_eba_bot.BotWrapper
{
    public class TelegramBotHostedService: IHostedService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public TelegramBotHostedService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            // Create a new scope to retrieve scoped services
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var telegramBotService =  scope.ServiceProvider.GetRequiredService<TelegramBotService>();
                telegramBotService.SetupBot();

                await Task.Delay(Int32.MaxValue, cancellationToken);
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            // noop
            return Task.CompletedTask;
        }
    }
}
