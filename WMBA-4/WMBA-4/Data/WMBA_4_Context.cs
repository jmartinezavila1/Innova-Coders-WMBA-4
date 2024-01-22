using Microsoft.EntityFrameworkCore;
using System.Numerics;
using WMBA_4.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace WMBA_4.Data
{
    public class WMBA_4_Context : DbContext
    {
        public WMBA_4_Context(DbContextOptions<WMBA_4_Context> options)
            : base(options)
        {
        }
        public DbSet<City> Cities { get; set; }
        public DbSet<Division> Divisions { get; set; }
        public DbSet<Game> Games { get; set; }

        public DbSet<GameLineUp> GameLineUps { get; set; }

        public DbSet<GameLineUpPosition> GameLineUpPositions { get; set; }

        public DbSet<GameType> GameTypes { get; set; }

        public DbSet<League> Leagues { get; set; }

        public DbSet<Location> Locations { get; set; }

        public DbSet<Player> Players { get; set; }

        public DbSet<Position> Positions { get; set; }

        public DbSet<ScorePlayer> ScorePlayers { get; set; }

        public DbSet<Season> Seasons { get; set; }

        public DbSet<Staff> Staff { get; set; }

        public DbSet<Team> Teams { get; set; }

        public DbSet<TeamGame> TeamGame { get; set; }

        public DbSet<TeamStaff> TeamStaff { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ///Prevent Cascade Delete from Game to GameType
            //so we are prevented from deleting a GameTpe with
            //Games assigned
            modelBuilder.Entity<GameType>()
                .HasMany<Game>(gt => gt.Games)
                .WithOne(t => t.GameType)
                .HasForeignKey(t => t.GameTypeID)
                .OnDelete(DeleteBehavior.Restrict);


            ///Prevent Cascade Delete from Game to Seasons
            //so we are prevented from deleting a Seasons with
            //Games assigned
            modelBuilder.Entity<Season>()
                .HasMany<Game>(gt => gt.Games)
                .WithOne(t => t.Season)
                .HasForeignKey(t => t.SeasonID)
                .OnDelete(DeleteBehavior.Restrict);

            ///Prevent Cascade Delete from Game to Location
            //so we are prevented from deleting a Location with
            //Games assigned
            modelBuilder.Entity<Location>()
                .HasMany<Game>(gt => gt.Games)
                .WithOne(t => t.Location)
                .HasForeignKey(t => t.LocationID)
                .OnDelete(DeleteBehavior.Restrict);


            ///Prevent Cascade Delete from Location to City
            //so we are prevented from deleting a City with
            //Location assigned
            modelBuilder.Entity<City>()
                .HasMany<Location>(gt => gt.Locations)
                .WithOne(t => t.City)
                .HasForeignKey(t => t.CityID)
                .OnDelete(DeleteBehavior.Restrict);


            ///Prevent Cascade Delete from League to City
            //so we are prevented from deleting a City with
            //League assigned
            modelBuilder.Entity<City>()
                .HasMany<League>(gt => gt.Leagues)
                .WithOne(t => t.City)
                .HasForeignKey(t => t.CityID)
                .OnDelete(DeleteBehavior.Restrict);


            ///Prevent Cascade Delete from Divisions to League
            //so we are prevented from deleting a League with
            //Division assigned
            modelBuilder.Entity<League>()
                .HasMany<Division>(gt => gt.Divisions)
                .WithOne(t => t.League)
                .HasForeignKey(t => t.LeagueID)
                .OnDelete(DeleteBehavior.Restrict);

            //Many to Many Intersection
            modelBuilder.Entity<TeamGame>()
            .HasKey(t => new { t.TeamID, t.GameID });


            //Prevent Cascade Delete from Team to TeamGame
            //so we are prevented from deleting a Team used for a Game
            modelBuilder.Entity<Team>()
                .HasMany<TeamGame>(tg => tg.TeamGames)
                .WithOne(t => t.Team)
                .HasForeignKey(t => t.TeamID)
                .OnDelete(DeleteBehavior.Restrict);

            //Many to Many Intersection
            modelBuilder.Entity<TeamStaff>()
            .HasKey(t => new { t.TeamID, t.StaffID });

            // Prevent Cascade Delete from Staff to TeamStaff
            //so we are prevented from deleting a Staff with
            //Teams assigned
            modelBuilder.Entity<Staff>()
                .HasMany<TeamStaff>(ft => ft.TeamStaff)
                .WithOne(f => f.Staff)
                .HasForeignKey(f => f.StaffID)
                .OnDelete(DeleteBehavior.Restrict);


            //Prevent Cascade Delete from Game to Scoreplayer
            //so we are prevented from deleting a FunctionType with
            //Functions assigned
            modelBuilder.Entity<Game>()
                .HasMany<ScorePlayer>(ft => ft.ScorePlayers)
                .WithOne(f => f.Game)
                .HasForeignKey(f => f.GameID)
                .OnDelete(DeleteBehavior.Restrict);


            //Prevent Cascade Delete from Teams to players
            //so we are prevented from deleting a Player with
            //Team assigned
            modelBuilder.Entity<Team>()
                .HasMany<Player>(ft => ft.Players)
                .WithOne(f => f.Team)
                .HasForeignKey(f => f.TeamID)
                .OnDelete(DeleteBehavior.Restrict);


            //Prevent Cascade Delete from Teams to players
            //so we are prevented from deleting a Player with
            //Team assigned
            modelBuilder.Entity<Team>()
                .HasMany<Player>(ft => ft.Players)
                .WithOne(f => f.Team)
                .HasForeignKey(f => f.TeamID)
                .OnDelete(DeleteBehavior.Restrict);

            //Add a unique index to Team name 
            modelBuilder.Entity<Team>()
            .HasIndex(t => t.Name)
            .IsUnique();

            //Add a unique index to Seasons code
            modelBuilder.Entity<Season>()
            .HasIndex(s => s.SeasonCode)
            .IsUnique();

            //Add a unique index to Divisons
            modelBuilder.Entity<Division>()
            .HasIndex(d => d.DivisionName)
            .IsUnique();

            //Add a unique index to Positions
            modelBuilder.Entity<Position>()
            .HasIndex(p => p.PositionName)
            .IsUnique();

            //Many to Many Intersection
            modelBuilder.Entity<GameLineUpPosition>()
            .HasKey(t => new { t.GameLineUpID, t.PositionID });


           
        }








    }
    }
