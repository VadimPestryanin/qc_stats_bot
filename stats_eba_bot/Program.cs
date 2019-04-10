using QCStats;
using System;
using System.Threading;
using System.Threading.Tasks;
using Autofac; 
using Autofac.Extensions.DependencyInjection;
using QCStats.Model.QC;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace stats_eba_bot
{

    class Program
    {

        //Bot settings
        static ITelegramBotClient _botClient;
        private const string _apiKey = "763383540:AAH6FLP2jB1r6pj8nIQwsivAG7rvMbOhFwE";

        //Container Settings
        private static IContainer _container { get; set; }

        //QCApi
        private static QCStatsAPI _api;

        static void Main(string[] args)
        {
            SetupContainer();
            _botClient = new TelegramBotClient(_apiKey);
            var me = _botClient.GetMeAsync().Result;
            Console.WriteLine(
                $"Hello, World! I am user {me.Id} and my name is {me.FirstName}."
            );

            _botClient.OnMessage += Bot_OnMessage;
            _botClient.StartReceiving();
            Thread.Sleep(int.MaxValue);
        }

        private static void SetupContainer()
        {
            var builder = new ContainerBuilder();
            var serviceCollection = QCStatsAPI.ConfigureServices();
            builder.Populate(serviceCollection);
            _container = builder.Build();
        }

        private async static Task<int> CheckPlayerStats(string playerName)
        {
          
            PlayerData player = await _api.Players.GetPlayerStatsAsync(playerName);

            if (!string.IsNullOrWhiteSpace(player?.Name))
            {
                PlayerProfileStats stats = player.ProfileStats;
                TimeSpan totalTimePlayed = stats.TimePlayed;

                Console.WriteLine();
                Console.WriteLine("QC Stats", ConsoleColor.DarkRed);
                Console.WriteLine($" ({playerName})", ConsoleColor.Cyan);
                Console.WriteLine();
                Console.WriteLine($"{@"KDR",-16}", stats.Kdr, stats.OverallStats);
                Console.WriteLine($"{@"Accuracy",-16} {stats.Accuracy:P2}");
                Console.WriteLine($"{@"Win Rate",-16} {stats.WinRate:P2}");
                Console.WriteLine($"{@"Ranked Win Rate",-16} {stats.RankedWinRate:P2}");
                Console.WriteLine($"{@"Time Played",-16} {totalTimePlayed.Days * 24 + totalTimePlayed.Hours}h {totalTimePlayed.Minutes}m {totalTimePlayed.Seconds}s");
                Console.WriteLine("\n");

                PlayerRating duelRating = null;
                player.Ratings.TryGetValue(QCStats.Enums.RankedGameMode.Duel, out duelRating);
                if (duelRating != null)
                {
                    return duelRating.Rating;
                }

            }

            return 0;
        }


        static async void Bot_OnMessage(object sender, MessageEventArgs e) {
            if (e.Message.Text != null)
            {
                //Console.WriteLine($"Received a text message in chat {e.Message.Chat.Id}.");

                if (e.Message.Text.StartsWith("/stats"))
                {
                    using (ILifetimeScope scope = _container.BeginLifetimeScope())
                    {
                        _api = scope.Resolve<QCStatsAPI>();
                        var name = e.Message.Text.Replace("/stats ", "");
                        int rating = await CheckPlayerStats(name);
                        if (rating > 0)
                        {
                            await _botClient.SendTextMessageAsync(
                                chatId: e.Message.Chat,
                                text:  $"{name} duel rating is: {rating}"
                            );
                        }
                        else
                        {
                            await _botClient.SendTextMessageAsync(
                                chatId: e.Message.Chat,
                                text:  $"{name} was not found in duels"
                            );
                        }
                    }
                   
                }
                else
                {
                    await _botClient.SendTextMessageAsync(
                        chatId: e.Message.Chat,
                        text:   "You said:\n" + e.Message.Text
                    );
                }


             
            }
        }
    }
}
