using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace stats_eba_bot.DataContext
{
    public class PlayersContext : DbContext
    {
        public DbSet<PlayerStatistic> PlayerStatistic { get; set; }

        public PlayersContext(DbContextOptions<PlayersContext> options)
            : base(options)
        {
        }
        
    }

    public class PlayerStatistic
    {
        public Guid Id { get; set; }
        public string PlayerName { get; set; }
        public int DuelRating { get; set; }

        public PlayerStatistic()
        {
            this.Id = Guid.NewGuid();
        }

    }
}
