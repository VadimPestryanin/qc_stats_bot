using stats_eba_bot.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using stats_eba_bot.ApiWrapper;

namespace stats_eba_bot.Cache
{
    public class SqlLiteCacheService
    {

        private readonly PlayersContext _context;

        public SqlLiteCacheService(PlayersContext context)
        {
            _context = context;
        }

        public void SaveInCache(string playerName, int duelRating, DateTime? lastDuelTime)
        {
            var existing = GetFromCache(playerName);
            if (existing == null)
            {
                _context.PlayerStatistic.Add(new PlayerStatistic()
                {
                    PlayerName = playerName,
                    DuelRating = duelRating,
                    LastDuelPlayed = lastDuelTime,
                    LastUpdatedDate = DateTime.UtcNow
                });
            }
            else 
            {
                existing.DuelRating = duelRating;
                existing.LastDuelPlayed = lastDuelTime;
                existing.LastUpdatedDate = DateTime.UtcNow;
            }
        }
        
        public void CommitChanges()
        {
            _context.SaveChanges();
        }

        public void RemoveFromCache(PlayerStatistic stat)
        {
            if (stat != null)
            {
                _context.PlayerStatistic.Remove(stat);
            }
        }

        public PlayerStatistic GetFromCache(string key)
        {
            var stat = _context.PlayerStatistic.FirstOrDefault(p => p.PlayerName == key);
            if (stat != null)
            {
                return stat;
            }

            return null;
        }

        public List<PlayerStatistic> ListAllRecords()
        {
            return _context.PlayerStatistic.OrderByDescending(a=>a.DuelRating).ToList();
        }
    }
}
