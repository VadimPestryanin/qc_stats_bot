using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Options;
using QCStats;
using QCStats.Model.QC;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace stats_eba_bot.BotWrapper
{
    public class TelegramBotService
    {
        //private readonly AppSettings _appSettings;
        private readonly QCStatsAPI _api;
        private TelegramBotClient _botClient;
        public TelegramBotService( QCStatsAPI api)
        {
            _api = api;
        }

        public async void SetupBot()
        {
            _botClient = new TelegramBotClient("763383540:AAH6FLP2jB1r6pj8nIQwsivAG7rvMbOhFwE"); //TODO IOptions?
            var me = _botClient.GetMeAsync().Result;
            Console.WriteLine(
                $"Hello, World! I am user {me.Id} and my name is {me.FirstName}."
            );

            _botClient.OnMessage += OnBotMessage;
            _botClient.StartReceiving();
            Thread.Sleep(Int32.MaxValue);
        }

        async void OnBotMessage(object sender, MessageEventArgs e)
        {
            if (e.Message.Text != null)
            {
                //Console.WriteLine($"Received a text message in chat {e.Message.Chat.Id}.");

                if (e.Message.Text.StartsWith("/stats"))
                {

                    var name = e.Message.Text.Replace("/stats ", "");
                    int rating = await CheckPlayerStats(name);
                    if (rating > 0)
                    {
                        await _botClient.SendTextMessageAsync(
                            chatId: e.Message.Chat,
                            text: $"{name} duel rating is: {rating}"
                        );
                    }
                    else
                    {
                        await _botClient.SendTextMessageAsync(
                            chatId: e.Message.Chat,
                            text: $"{name} was not found in duels"
                        );
                    }
                }

            }
            else
            {
                await _botClient.SendTextMessageAsync(
                    chatId: e.Message.Chat,
                    text: "You said:\n" + e.Message.Text
                );
            }



        }
        private async Task<int> CheckPlayerStats(string playerName)
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
    }
}

