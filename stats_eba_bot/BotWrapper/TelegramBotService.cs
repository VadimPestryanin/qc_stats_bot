using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using QCStats;
using QCStats.Model.QC;
using stats_eba_bot.ApiWrapper;
using stats_eba_bot.Cache;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace stats_eba_bot.BotWrapper
{
    public class TelegramBotService
    {
        //private readonly AppSettings _appSettings;

        private readonly QCApiService _qCApiService;
        private TelegramBotClient _botClient;
        private readonly SqlLiteCacheService _cacheService;

        public TelegramBotService(SqlLiteCacheService cacheService,QCApiService qCApiService)
        {
            _cacheService = cacheService;
            _qCApiService = qCApiService;
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
                if (e.Message.Text.StartsWith("/stats"))
                {
                    var name = e.Message.Text.Replace("/stats", "");
                    name = name.TrimStart();
                    if (!string.IsNullOrEmpty(name))
                    {
                        int rating = await _qCApiService.GetPlayerRating(name);
                        if (rating > 0)
                        {
                            var cached = _cacheService.GetFromCache(name);
                            if (cached != null)
                            {
                                await _botClient.SendTextMessageAsync(
                                    chatId: e.Message.Chat,
                                    text: $"{name} duel rating in cache is: {cached.DuelRating}"
                                );
                            }
                            else
                            {
                                _cacheService.SaveInCache(name,rating);
                            }
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
                    else//TODO split
                    {
                        var all = _cacheService.ListAllRecords();

                        string allRecords = String.Join("\n",
                            all.Select(a => $"{a.PlayerName} duel rating in cache {a.DuelRating}"));

                        await _botClient.SendTextMessageAsync(
                            chatId: e.Message.Chat,
                            text: allRecords
                        );
                    }
                    
                }

            }
            //else
            //{
            //    await _botClient.SendTextMessageAsync(
            //        chatId: e.Message.Chat,
            //        text: "You said:\n" + e.Message.Text
            //    );
            //}
        }

       
       
    }
}

