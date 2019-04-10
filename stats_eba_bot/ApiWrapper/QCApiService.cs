using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using QCStats;
using QCStats.Model.QC;

namespace stats_eba_bot.ApiWrapper
{
    public class QCApiService
    {
        private readonly QCStatsAPI _api;

        public QCApiService(QCStatsAPI api)
        {
            _api = api;
        }

        public async Task<int> GetPlayerRating(string playerName)
        {
            PlayerData player = await _api.Players.GetPlayerStatsAsync(playerName);
            PlayerRating duelRating = null;
            player.Ratings.TryGetValue(QCStats.Enums.RankedGameMode.Duel, out duelRating);
            if (duelRating != null)
            {
                return duelRating.Rating;
            }

            return 0;
        } 
        
        public async Task<int> CheckPlayerStats(string playerName)
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
