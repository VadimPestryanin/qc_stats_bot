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
using stats_eba_bot.DataContext;
using Telegram.Bot;
using Telegram.Bot.Args;
using stats_eba_bot.Helpers;

namespace stats_eba_bot.BotWrapper
{
    public enum CommandCode : int
    {
        AddPlayer,
        UpdateCache,
        ListPlayers,
        RemovePlayer,
        Help,
        Unknown
    }

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
                var commandCode = ParseCommand(e.Message.Text);
                
                if (commandCode == CommandCode.Help)
                {
                    await PrintHelp(e); //reply to same chat
                }

                if (commandCode == CommandCode.ListPlayers)
                {
                    await ListAllPlayers(e); //reply to same chat
                }
                else if (commandCode == CommandCode.UpdateCache)
                {
                    await UpdatePlayersCache(e);
                }
                else if (commandCode == CommandCode.AddPlayer)
                {
                    var name = e.Message.Text.Replace("/addplayer", "");
                    name = name.TrimStart();
                    if (!string.IsNullOrEmpty(name))
                    {
                        int rating = await _qCApiService.GetPlayerRating(name);
                        if (rating > 0)
                        {
                            var cached = _cacheService.GetFromCache(name);
                            if (cached == null)
                            {
                                _cacheService.SaveInCache(name,rating);
                            }
                            await ListAllPlayers(e); //reply to same chat
                        }
                        else
                        {
                            await _botClient.SendTextMessageAsync(
                                chatId: e.Message.Chat,
                                text: $"{name} was not found in duels"
                            );
                        }
                    }
                }else if (commandCode == CommandCode.RemovePlayer)
                {
                    var name = e.Message.Text.Replace("/removeplayer", ""); //DRY LOL
                    name = name.TrimStart();
                    if (!string.IsNullOrEmpty(name))
                    {
                        var cached = _cacheService.GetFromCache(name);
                        if (cached != null)
                        {
                            _cacheService.RemoveFromCache(cached);
                            await ListAllPlayers(e);
                        }
                        else
                        {
                            await _botClient.SendTextMessageAsync(
                                chatId: e.Message.Chat,
                                text: $"{name} was not found in cache"
                            );
                        }
                    }
                }
            }
        }

        private async Task UpdatePlayersCache(MessageEventArgs e = null)
        {
            var allPlayers = _cacheService.ListAllRecords();
            allPlayers = allPlayers.Where(a => a.LastUpdatedDate < DateTime.UtcNow.AddMinutes(-5)).ToList();
            foreach (var player in allPlayers)
            {
                int rating = await _qCApiService.GetPlayerRating(player.PlayerName);
                if (rating > 0)
                {
                    _cacheService.SaveInCache(player.PlayerName, rating);
                }
            }
            if(e != null) await ListAllPlayers(e); //reply to same chat
        }


      

        private string FormatMessageLine(PlayerStatistic player, int position)
        {
            var playerUrl = $"https://stats.quake.com/profile/{player.PlayerName}";
            var str = 
                $"[{player.PlayerName}]({playerUrl}) {EmojiHelper.GeneratePositionEmoju(position)} Duel rating  is: {player.DuelRating}. Updated on {player.LastUpdatedDate.ToString()} \n";
            return str;
        }

        private async Task ListAllPlayers(MessageEventArgs e)
        {
            var all = _cacheService.ListAllRecords();

          
            string allRecords = String.Join("",
                all.Select(a => FormatMessageLine(a, all.IndexOf(a))));

            if (!string.IsNullOrEmpty(allRecords))
            {
                await _botClient.SendTextMessageAsync(
                    chatId: e.Message.Chat,
                    text: allRecords,
                    parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
                );
            }
            else
            {
                await _botClient.SendTextMessageAsync(
                    chatId: e.Message.Chat,
                    text: "Database is empty, add players first"
                );
            }
          
        }

        private async Task PrintHelp(MessageEventArgs e)
        {
            var all = _cacheService.ListAllRecords();


            string message = "available commands:\n" +
                             "/updatecache - refresh current player cache \n" +
                             "/stats - display saved players stats \n" +
                             "/addplayer <name> - add new player in cache \n" +
                             "/removeplayer <name> - remove player from cache";
          
            await _botClient.SendTextMessageAsync(
                chatId: e.Message.Chat,
                text: message,
                parseMode: Telegram.Bot.Types.Enums.ParseMode.Markdown
            );
        }

        private CommandCode ParseCommand(string message)
        {
            var lowercaseMessage = message.ToLower();
           
            if (lowercaseMessage == "/updatecache") return CommandCode.UpdateCache;
            
            if (lowercaseMessage == "/stats") return CommandCode.UpdateCache;
            
            if (lowercaseMessage.StartsWith("/addplayer")) return CommandCode.AddPlayer;
            
            if (lowercaseMessage.StartsWith("/removeplayer")) return CommandCode.RemovePlayer;

            if (lowercaseMessage.StartsWith("/help")) return CommandCode.Help;

            return CommandCode.Unknown;
        }
    }
}

