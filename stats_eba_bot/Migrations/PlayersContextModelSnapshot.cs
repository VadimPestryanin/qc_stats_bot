﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using stats_eba_bot.DataContext;

namespace stats_eba_bot.Migrations
{
    [DbContext(typeof(PlayersContext))]
    partial class PlayersContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity("stats_eba_bot.DataContext.PlayerStatistic", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("DuelRating");

                    b.Property<string>("PlayerName");

                    b.HasKey("Id");

                    b.ToTable("PlayerStatistic");
                });
#pragma warning restore 612, 618
        }
    }
}
