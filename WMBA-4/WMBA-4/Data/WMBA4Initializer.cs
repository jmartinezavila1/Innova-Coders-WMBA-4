﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using WMBA_4.Models;


namespace WMBA_4.Data
{
    public static class WMBA4Initializer
    {
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            WMBA_4_Context context = applicationBuilder.ApplicationServices.CreateScope()
                .ServiceProvider.GetRequiredService<WMBA_4_Context>();

            try
            {
                //We can use this to delete the database and start fresh.
                //context.Database.EnsureDeleted();
                //context.Database.EnsureCreated();
                context.Database.Migrate();
                //To randomly generate data
                Random random = new Random();

                //Cities 
                if (!context.Cities.Any())
                {
                    var cities = new List<City>
                    {
                        new City { ID = 1, CityName = "Toronto" },
                        new City { ID = 2, CityName = "Vancouver" },
                        new City { ID = 3, CityName = "Montreal" },
                        new City { ID = 4, CityName = "Calgary" },
                        new City { ID = 5, CityName = "Edmonton" },
                        new City { ID = 6, CityName = "Ottawa" },
                        new City { ID = 7, CityName = "Winnipeg" },
                        new City { ID = 8, CityName = "Quebec City" },
                        new City { ID = 9, CityName = "Hamilton" },
                        new City { ID = 10, CityName = "Halifax" },
                        new City { ID = 11, CityName = "London" },
                        new City { ID = 12, CityName = "Victoria" },
                        new City { ID = 13, CityName = "Saskatoon" }

                    };
                    context.Cities.AddRange(cities);
                    context.SaveChanges();
                }


                // Club
                if (!context.Clubs.Any())
                {
                    var clubs = new List<Club>
                    {
                        new Club { ID = 1, ClubName = "Welland Minor Baseball Association", CityID = 1 },
                     

                    };

                    context.Clubs.AddRange(clubs);
                    context.SaveChanges();
                }

                // Divisions 
                if (!context.Divisions.Any())
                {
                    var divisions = new List<Division>
                    {                   
                        new Division { ID = 1, DivisionName = "9U",ClubID = 1},
                        new Division { ID = 2, DivisionName = "11U",ClubID = 1},
                        new Division { ID = 3, DivisionName = "13U",ClubID = 1},
                        new Division { ID = 4, DivisionName = "15U",ClubID = 1},
                        new Division { ID = 5, DivisionName = "18U",ClubID = 1},
                    };

                    context.Divisions.AddRange(divisions);
                    context.SaveChanges();
                }


                // Teams 
                if (!context.Teams.Any())
                {
                    var teams = new List<Team>
                    {
                        new Team { ID = 1, Name = "Toronto Blue Jays", Coach_Name = "Charlie Montoyo", DivisionID = 1},
                        new Team { ID = 2, Name = "Montreal Expos", Coach_Name = "No Manager", DivisionID = 1 },
                        new Team { ID = 3, Name = "Vancouver Canadians", Coach_Name = "John Schneider", DivisionID = 2 },
                        new Team { ID = 4, Name = "Calgary Cannons", Coach_Name = "No Manager", DivisionID = 3 },
                        new Team { ID = 5, Name = "Edmonton Trappers", Coach_Name = "No Manager", DivisionID = 4 },
                        new Team { ID = 6, Name = "Ottawa Lynx", Coach_Name = "No Manager" , DivisionID = 1},
                        new Team { ID = 7, Name = "Winnipeg Goldeyes", Coach_Name = "Rick Forney" , DivisionID = 2},
                        new Team { ID = 8, Name = "Quebec Capitales", Coach_Name = "Patrick Scalabrini", DivisionID = 3 },
                        new Team { ID = 9, Name = "Hamilton Thunderbirds", Coach_Name = "No Manager", DivisionID = 4},
                        new Team { ID = 10, Name = "Halifax Hurricanes", Coach_Name = "No Manager" , DivisionID = 1},
                        new Team { ID = 11, Name = "London Majors", Coach_Name = "No Manager", DivisionID = 2 },
                        new Team { ID = 12, Name = "Victoria HarbourCats", Coach_Name = "Brian McRae", DivisionID = 3 },
                        new Team { ID = 13, Name = "Saskatoon Yellow Jackets", Coach_Name = "No Manager",DivisionID = 4 },
                        new Team { ID = 14, Name = "Calgary Mavericks", Coach_Name = "No Manager", DivisionID = 1 },
                        new Team { ID = 15, Name = "13U Bananas", Coach_Name = "Orv Franchuk", DivisionID = 5 },
                        new Team { ID = 16, Name = "13U Iron Birds", Coach_Name = "No Manager", DivisionID = 5 },
                        new Team { ID = 17, Name = "13U Whitecaps", Coach_Name = "No Manager", DivisionID = 5 },
                        new Team { ID = 18, Name = "15U Bisons", Coach_Name = "No Manager", DivisionID = 5 },
                        new Team { ID = 19, Name = "15U Dragons", Coach_Name = "No Manager", DivisionID = 1 },
                        new Team { ID = 20, Name = "15U Trash Pandas", Coach_Name = "No Manager", DivisionID = 2 },
                        
                    };

                    context.Teams.AddRange(teams);
                    context.SaveChanges();
                }

                // Staff
                if (!context.Staff.Any())
                {
                    var staffMembers = new List<Staff>
                    {
                        new Staff { FirstName = "John", LastName = "Doe", Email = "john.doe@example.com" },
                        new Staff { FirstName = "Jane", LastName = "Smith", Email = "jane.smith@example.com" },
                        new Staff { FirstName = "Robert", LastName = "Johnson", Email = "robert.johnson@example.com" },
                        new Staff { FirstName = "Emily", LastName = "Williams", Email = "emily.williams@example.com" },
                        new Staff { FirstName = "Michael", LastName = "Brown", Email = "michael.brown@example.com" },
                        new Staff { FirstName = "Olivia", LastName = "Miller", Email = "olivia.miller@example.com" },
                        new Staff { FirstName = "William", LastName = "Davis", Email = "william.davis@example.com" },
                        new Staff { FirstName = "Sophia", LastName = "Garcia", Email = "sophia.garcia@example.com" },
                        new Staff { FirstName = "James", LastName = "Martinez", Email = "james.martinez@example.com" },
                        new Staff { FirstName = "Emma", LastName = "Jackson", Email = "emma.jackson@example.com" },
                        new Staff { FirstName = "Alexander", LastName = "Taylor", Email = "alexander.taylor@example.com" },
                        new Staff { FirstName = "Ava", LastName = "Anderson", Email = "ava.anderson@example.com" },
                        new Staff { FirstName = "Daniel", LastName = "Thomas", Email = "daniel.thomas@example.com" },
                        new Staff { FirstName = "Mia", LastName = "Moore", Email = "mia.moore@example.com" },
                        new Staff { FirstName = "Ethan", LastName = "Clark", Email = "ethan.clark@example.com" },
                        new Staff { FirstName = "Isabella", LastName = "Lewis", Email = "isabella.lewis@example.com" },
                        new Staff { FirstName = "Benjamin", LastName = "Hill", Email = "benjamin.hill@example.com" },
                        new Staff { FirstName = "Avery", LastName = "King", Email = "avery.king@example.com" },
                        new Staff { FirstName = "Ryan", LastName = "Cooper", Email = "ryan.cooper@example.com" },
                        new Staff { FirstName = "Nora", LastName = "Baker", Email = "nora.baker@example.com" }
                    };

                    context.Staff.AddRange(staffMembers);
                    context.SaveChanges();
                }


                // TeamStaff
                if (!context.TeamStaff.Any())
                {
                    var teamStaff = new List<TeamStaff>
                    {
                        new TeamStaff { TeamID = 1, StaffID = 1 },
                        new TeamStaff { TeamID = 2, StaffID = 2 },
                        new TeamStaff { TeamID = 3, StaffID = 3 },
                        new TeamStaff { TeamID = 4, StaffID = 4 },
                        new TeamStaff { TeamID = 5, StaffID = 5 },
                        new TeamStaff { TeamID = 6, StaffID = 6 },
                        new TeamStaff { TeamID = 7, StaffID = 7 },
                        new TeamStaff { TeamID = 8, StaffID = 8 },
                        new TeamStaff { TeamID = 9, StaffID = 9 },
                        new TeamStaff { TeamID = 10, StaffID = 10 },
                        new TeamStaff { TeamID = 11, StaffID = 11 },
                        new TeamStaff { TeamID = 12, StaffID = 12 },
                        new TeamStaff { TeamID = 13, StaffID = 13 },
                        new TeamStaff { TeamID = 14, StaffID = 14 },
                        new TeamStaff { TeamID = 15, StaffID = 15 },
                        new TeamStaff { TeamID = 16, StaffID = 16 },
                        new TeamStaff { TeamID = 17, StaffID = 17 },
                        new TeamStaff { TeamID = 18, StaffID = 18 },
                        new TeamStaff { TeamID = 19, StaffID = 19 },
                        new TeamStaff { TeamID = 20, StaffID = 20 }
                    };

                    context.TeamStaff.AddRange(teamStaff);
                    context.SaveChanges();
                }

                // Seasons 
                if (!context.Seasons.Any())
                {
                    var seasons = new List<Season>
                    {
                        new Season { ID = 1, SeasonCode = "2023", SeasonName = "Summer 2023" },

                    };

                    context.Seasons.AddRange(seasons);
                    context.SaveChanges();
                }


                // Locations 
                if (!context.Locations.Any())
                {
                    var locations = new List<Location>
                    {
                        new Location { ID = 1, LocationName = "Rogers Centre", CityID = 1 },
                        new Location { ID = 2, LocationName = "Olympic Stadium", CityID = 2 },
                        new Location { ID = 3, LocationName = "BC Place", CityID = 3 },
                        new Location { ID = 4, LocationName = "McMahon Stadium", CityID = 4 },
                        new Location { ID = 5, LocationName = "Truist Park", CityID = 5 },
                        new Location { ID = 6, LocationName = "TD Place Stadium", CityID = 6 },
                        new Location { ID = 7, LocationName = "Shaw Park", CityID = 7 },
                        new Location { ID = 8, LocationName = "Stade Canac", CityID = 8 },
                        new Location { ID = 9, LocationName = "Tim Hortons Field", CityID = 9 },
                        new Location { ID = 10, LocationName = "Scotiabank Centre", CityID = 10 }

                    };

                    context.Locations.AddRange(locations);
                    context.SaveChanges();
                }



                // Games 
                if (!context.Games.Any())
                {
                    var games = new List<Game>
                    {
                         // Games for 2022_
                        new Game { ID = 1, Date = new DateTime(2022, 1, 1), LocationID = 1, SeasonID = 1},
                        new Game { ID = 2, Date = new DateTime(2022, 2, 1), LocationID = 2, SeasonID = 1},
                        new Game { ID = 3, Date = new DateTime(2022, 3, 1), LocationID = 3, SeasonID = 1},
                        new Game { ID = 4, Date = new DateTime(2022, 4, 1), LocationID = 4, SeasonID = 1},
                        new Game { ID = 5, Date = new DateTime(2022, 5, 1), LocationID = 5, SeasonID = 1},


                        // Games for 2023
                        new Game { ID = 6, Date = new DateTime(2023, 1, 1), LocationID = 6, SeasonID = 1},
                        new Game { ID = 7, Date = new DateTime(2023, 2, 1), LocationID = 7, SeasonID = 1},
                        new Game { ID = 8, Date = new DateTime(2023, 3, 1), LocationID = 8, SeasonID = 1},
                        new Game { ID = 9, Date = new DateTime(2023, 4, 1), LocationID = 9, SeasonID = 1},
                        new Game { ID = 10, Date = new DateTime(2023, 5, 1), LocationID = 10, SeasonID = 1}


                    };

                    context.Games.AddRange(games);
                    context.SaveChanges();
                }


                // TeamGame
                if (!context.TeamGame.Any())
                {
                    var teamGames = new List<TeamGame>
                    {
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 5, TeamID = 1, GameID = 1 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 7, TeamID = 2, GameID = 1 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 3, TeamID = 3, GameID = 2 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 8, TeamID = 4, GameID = 2 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 6, TeamID = 5, GameID = 3 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 5, TeamID = 6, GameID = 3 },
                        //new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 7, TeamID = 7, GameID = 4 },
                        //new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 3, TeamID = 8, GameID = 4 },
                        //new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 8, TeamID = 9, GameID = 5 },
                        //new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 6, TeamID = 10, GameID = 5 },
                        //new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 5, TeamID = 11, GameID = 6 },
                        //new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 7, TeamID = 12, GameID = 6 },
                        //new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 3, TeamID = 13, GameID = 7 },
                        //new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 8, TeamID = 14, GameID = 7 },
                        //new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 6, TeamID = 15, GameID = 8 },
                        //new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 5, TeamID = 16, GameID = 8 },
                        //new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 7, TeamID = 17, GameID = 9 },
                        //new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 3, TeamID = 18, GameID = 9 },
                        //new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 8, TeamID = 19, GameID = 10 },
                        //new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 6, TeamID = 20, GameID = 10 }
                    };

                    context.TeamGame.AddRange(teamGames);
                    context.SaveChanges();
                }

                // Positions
                if (!context.Positions.Any())
                {
                    var positions = new List<Position>
                    {
                        new Position { ID = 1, PositionCode = "P", PositionName = "Pitcher" },
                        new Position { ID = 2, PositionCode = "C", PositionName = "Catcher" },
                        new Position { ID = 3, PositionCode = "1B", PositionName = "First Base" },
                        new Position { ID = 4, PositionCode = "2B", PositionName = "Second Base" },
                        new Position { ID = 5, PositionCode = "SS", PositionName = "Shortstop" },
                        new Position { ID = 6, PositionCode = "3B", PositionName = "Third Base" },
                        new Position { ID = 7, PositionCode = "LF", PositionName = "Left Field" },
                        new Position { ID = 8, PositionCode = "CF", PositionName = "Center Field" },
                        new Position { ID = 9, PositionCode = "RF", PositionName = "Right Field" },
                        new Position { ID = 10, PositionCode = "DH", PositionName = "Designated Hitter" }

                    };

                    context.Positions.AddRange(positions);
                    context.SaveChanges();
                }


                // Players
                if (!context.Players.Any())
                {
                    var players = new List<Player>
                    {
                        new Player { ID = 1, TeamID = 1, MemberID = "M001", FirstName = "John", LastName = "Doe", JerseyNumber = "10" },
                        new Player { ID = 2, TeamID = 1, MemberID = "M002", FirstName = "Jane", LastName = "Smith", JerseyNumber = "22" },
                        new Player { ID = 3, TeamID = 1, MemberID = "M003", FirstName = "Michael", LastName = "Johnson", JerseyNumber = "5" },
                        new Player { ID = 4, TeamID = 1, MemberID = "M004", FirstName = "Emily", LastName = "Williams", JerseyNumber = "15" },
                        new Player { ID = 5, TeamID = 1, MemberID = "M005", FirstName = "David", LastName = "Brown", JerseyNumber = "7" },
                        new Player { ID = 6, TeamID = 1, MemberID = "M006", FirstName = "Olivia", LastName = "Jones", JerseyNumber = "33" },
                        new Player { ID = 7, TeamID = 1, MemberID = "M007", FirstName = "Andrew", LastName = "Miller", JerseyNumber = "19" },
                        new Player { ID = 8, TeamID = 1, MemberID = "M008", FirstName = "Sophia", LastName = "Davis", JerseyNumber = "8" },
                        new Player { ID = 9, TeamID = 1, MemberID = "M009", FirstName = "William", LastName = "Taylor", JerseyNumber = "3" },
                        new Player { ID = 10, TeamID = 1, MemberID = "M010", FirstName = "Emma", LastName = "Anderson", JerseyNumber = "25" },
                        new Player { ID = 11, TeamID = 1, MemberID = "M011", FirstName = "Liam", LastName = "Moore", JerseyNumber = "12" },
                        new Player { ID = 12, TeamID = 1, MemberID = "M012", FirstName = "Ava", LastName = "White", JerseyNumber = "14" },
                       
                        new Player { ID = 13, TeamID = 2, MemberID = "M013", FirstName = "Ethan", LastName = "Harris", JerseyNumber = "9" },
                        new Player { ID = 14, TeamID = 2, MemberID = "M014", FirstName = "Chloe", LastName = "Martin", JerseyNumber = "18" },
                        new Player { ID = 15, TeamID = 2, MemberID = "M015", FirstName = "Benjamin", LastName = "Clark", JerseyNumber = "6" },
                        new Player { ID = 16, TeamID = 2, MemberID = "M016", FirstName = "Grace", LastName = "Lewis", JerseyNumber = "20" },
                        new Player { ID = 17, TeamID = 2, MemberID = "M017", FirstName = "Lucas", LastName = "Walker", JerseyNumber = "11" },
                        new Player { ID = 18, TeamID = 2, MemberID = "M018", FirstName = "Lily", LastName = "Allen", JerseyNumber = "23" },
                        new Player { ID = 19, TeamID = 2, MemberID = "M019", FirstName = "Henry", LastName = "Young", JerseyNumber = "4" },
                        new Player { ID = 20, TeamID = 2, MemberID = "M020", FirstName = "Sophie", LastName = "Turner", JerseyNumber = "30" },
                        new Player { ID = 21, TeamID = 2, MemberID = "M021", FirstName = "Owen", LastName = "Baker", JerseyNumber = "17" },
                        new Player { ID = 22, TeamID = 2, MemberID = "M022", FirstName = "Zoe", LastName = "Cooper", JerseyNumber = "21" },
                        new Player { ID = 23, TeamID = 2, MemberID = "M023", FirstName = "Elijah", LastName = "Perry", JerseyNumber = "2" },
                        new Player { ID = 24, TeamID = 2, MemberID = "M024", FirstName = "Mia", LastName = "Fisher", JerseyNumber = "16" },
                       
                        new Player { ID = 25, TeamID = 3, MemberID = "M025", FirstName = "Caleb", LastName = "Reed", JerseyNumber = "13" },
                        new Player { ID = 26, TeamID = 3, MemberID = "M026", FirstName = "Avery", LastName = "Murphy", JerseyNumber = "29" },
                        new Player { ID = 27, TeamID = 3, MemberID = "M027", FirstName = "Gabriel", LastName = "Bell", JerseyNumber = "24" },
                        new Player { ID = 28, TeamID = 3, MemberID = "M028", FirstName = "Scarlett", LastName = "Ward", JerseyNumber = "31" },
                        new Player { ID = 29, TeamID = 3, MemberID = "M029", FirstName = "Isaac", LastName = "Cox", JerseyNumber = "1" },
                        new Player { ID = 30, TeamID = 3, MemberID = "M030", FirstName = "Hannah", LastName = "Lopez", JerseyNumber = "26" },
                        new Player { ID = 31, TeamID = 3, MemberID = "M031", FirstName = "Jackson", LastName = "Rogers", JerseyNumber = "22" },
                        new Player { ID = 32, TeamID = 3, MemberID = "M032", FirstName = "Aria", LastName = "Hill", JerseyNumber = "8" },
                        new Player { ID = 33, TeamID = 3, MemberID = "M033", FirstName = "Lincoln", LastName = "Wright", JerseyNumber = "14" },
                        new Player { ID = 34, TeamID = 3, MemberID = "M034", FirstName = "Ella", LastName = "Barnes", JerseyNumber = "19" },
                       
                        new Player { ID = 35, TeamID = 4, MemberID = "M035", FirstName = "Mason", LastName = "Ferguson", JerseyNumber = "10" },
                        new Player { ID = 36, TeamID = 4, MemberID = "M036", FirstName = "Aurora", LastName = "Simmons", JerseyNumber = "28" },
                        new Player { ID = 37, TeamID = 4, MemberID = "M037", FirstName = "Landon", LastName = "Powell", JerseyNumber = "6" },
                        new Player { ID = 38, TeamID = 4, MemberID = "M038", FirstName = "Piper", LastName = "Watson", JerseyNumber = "23" },
                        new Player { ID = 39, TeamID = 4, MemberID = "M039", FirstName = "Logan", LastName = "Hughes", JerseyNumber = "9" },
                        new Player { ID = 40, TeamID = 4, MemberID = "M040", FirstName = "Aaliyah", LastName = "Fletcher", JerseyNumber = "20" },
                        new Player { ID = 41, TeamID = 4, MemberID = "M041", FirstName = "Mason", LastName = "Ferguson", JerseyNumber = "11" },
                        new Player { ID = 42, TeamID = 4, MemberID = "M042", FirstName = "Aurora", LastName = "Simmons", JerseyNumber = "26" },
                        new Player { ID = 43, TeamID = 4, MemberID = "M043", FirstName = "Landon", LastName = "Powell", JerseyNumber = "2" },
                        new Player { ID = 44, TeamID = 4, MemberID = "M044", FirstName = "Piper", LastName = "Watson", JerseyNumber = "1" },
                        new Player { ID = 45, TeamID = 4, MemberID = "M045", FirstName = "Logan", LastName = "Hughes", JerseyNumber = "4" },

                    };

                    context.Players.AddRange(players);
                    context.SaveChanges();

                }

                // GameLineUps
                if (!context.GameLineUps.Any())
                {
                    var gameLineUps = new List<GameLineUp>
                    {
                        new GameLineUp { ID = 1, BattingOrder = 1, GameID = 1, PlayerID = 1, TeamID = 1 },
                        new GameLineUp { ID = 2, BattingOrder = 2, GameID = 1, PlayerID = 2, TeamID = 1 },
                        new GameLineUp { ID = 3, BattingOrder = 3, GameID = 1, PlayerID = 3, TeamID = 2 },
                        new GameLineUp { ID = 4, BattingOrder = 4, GameID = 1, PlayerID = 4, TeamID = 2 },
                        new GameLineUp { ID = 5, BattingOrder = 5, GameID = 2, PlayerID = 5, TeamID = 3 },
                        new GameLineUp { ID = 6, BattingOrder = 6, GameID = 2, PlayerID = 6, TeamID = 3 },
                        new GameLineUp { ID = 7, BattingOrder = 7, GameID = 3, PlayerID = 7, TeamID = 4 },
                        new GameLineUp { ID = 8, BattingOrder = 8, GameID = 3, PlayerID = 8, TeamID = 4 },
                        new GameLineUp { ID = 9, BattingOrder = 9, GameID = 4, PlayerID = 9, TeamID = 5 },
                        new GameLineUp { ID = 10, BattingOrder = 1, GameID = 4, PlayerID = 10, TeamID = 5 },
                        new GameLineUp { ID = 11, BattingOrder = 2, GameID = 5, PlayerID = 11, TeamID = 6 },
                        new GameLineUp { ID = 12, BattingOrder = 3, GameID = 5, PlayerID = 12, TeamID = 6 },
                        new GameLineUp { ID = 13, BattingOrder = 4, GameID = 6, PlayerID = 13, TeamID = 7 },
                        new GameLineUp { ID = 14, BattingOrder = 5, GameID = 6, PlayerID = 14, TeamID = 7 },
                        new GameLineUp { ID = 15, BattingOrder = 6, GameID = 7, PlayerID = 15, TeamID = 8 },
                        new GameLineUp { ID = 16, BattingOrder = 7, GameID = 7, PlayerID = 16, TeamID = 8 },
                        new GameLineUp { ID = 17, BattingOrder = 8, GameID = 8, PlayerID = 17, TeamID = 9 },
                        new GameLineUp { ID = 18, BattingOrder = 9, GameID = 8, PlayerID = 18, TeamID = 9 },
                        new GameLineUp { ID = 19, BattingOrder = 1, GameID = 9, PlayerID = 19, TeamID = 10 },
                        new GameLineUp { ID = 20, BattingOrder = 2, GameID = 9, PlayerID = 20, TeamID = 10 }

                    };

                    context.GameLineUps.AddRange(gameLineUps);
                    context.SaveChanges();
                }

                // GameLineUpPositions
                if (!context.GameLineUpPositions.Any())
                {
                    var gameLineUpPositions = new List<GameLineUpPosition>
                    {
                        new GameLineUpPosition { GameLineUpID = 1, PositionID = 1 },
                        new GameLineUpPosition { GameLineUpID = 1, PositionID = 2 },
                        new GameLineUpPosition { GameLineUpID = 2, PositionID = 3 },
                        new GameLineUpPosition { GameLineUpID = 2, PositionID = 4 },
                        new GameLineUpPosition { GameLineUpID = 3, PositionID = 5 },
                        new GameLineUpPosition { GameLineUpID = 3, PositionID = 6 },
                        new GameLineUpPosition { GameLineUpID = 4, PositionID = 7 },
                        new GameLineUpPosition { GameLineUpID = 4, PositionID = 8 },
                        new GameLineUpPosition { GameLineUpID = 5, PositionID = 9 },
                        new GameLineUpPosition { GameLineUpID = 5, PositionID = 10 },
                        new GameLineUpPosition { GameLineUpID = 6, PositionID = 1 },
                        new GameLineUpPosition { GameLineUpID = 6, PositionID = 2 },
                        new GameLineUpPosition { GameLineUpID = 7, PositionID = 3 },
                        new GameLineUpPosition { GameLineUpID = 7, PositionID = 4 },
                        new GameLineUpPosition { GameLineUpID = 8, PositionID = 5 },
                        new GameLineUpPosition { GameLineUpID = 8, PositionID = 6 },
                        new GameLineUpPosition { GameLineUpID = 9, PositionID = 7 },
                        new GameLineUpPosition { GameLineUpID = 9, PositionID = 8 },
                        new GameLineUpPosition { GameLineUpID = 10, PositionID = 9 },
                        new GameLineUpPosition { GameLineUpID = 10, PositionID = 10 }

                    };

                    context.GameLineUpPositions.AddRange(gameLineUpPositions);
                    context.SaveChanges();
                }


                // ScorePlayers
                if (!context.ScorePlayers.Any())
                {
                    var scorePlayers = new List<ScorePlayer>
                    {
                        new ScorePlayer
                        {
                            InningNumber = 1,
                            H = 3,
                            RBI = 2,
                            R = 1,
                            StrikeOut = 0,
                            GroundOut = 1,
                            PopOut = 0,
                            Flyout = 1,
                            Singles = 2,
                            Doubles = 1,
                            Triples = 0,
                            HR = 1,
                            BB = 1,
                            HBP = 0,
                            SB = 1,
                            SAC = 0,
                            PA = 4,
                            AB = 3,
                            GameID = 1,
                            PlayerID = 1
                        },
                        new ScorePlayer
                        {
                            InningNumber = 1,
                            H = 3,
                            RBI = 2,
                            R = 1,
                            StrikeOut = 0,
                            GroundOut = 1,
                            PopOut = 0,
                            Flyout = 1,
                            Singles = 2,
                            Doubles = 1,
                            Triples = 0,
                            HR = 1,
                            BB = 1,
                            HBP = 0,
                            SB = 1,
                            SAC = 0,
                            PA = 4,
                            AB = 3,
                            GameID = 1,
                            PlayerID = 2
                        },
                    };

                    context.ScorePlayers.AddRange(scorePlayers);
                    context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.GetBaseException().Message);
            }
        }
    }

}