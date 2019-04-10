using stats_eba_bot.DataContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace stats_eba_bot.Cache
{
    public class SqlLiteCacheService
    {

        private readonly PlayersContext _context;

        public SqlLiteCacheService(PlayersContext context)
        {
            _context = context;
        }

        public void SaveInCache(string key, int value)
        {

            var existing = GetFromCache(key);
            if (existing == null)
            {
                _context.PlayerStatistic.Add(new PlayerStatistic()
                {
                    PlayerName = key,
                    DuelRating = value
                });
            }
            else
            {
                existing.DuelRating = value;
            }

            _context.SaveChanges();


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
