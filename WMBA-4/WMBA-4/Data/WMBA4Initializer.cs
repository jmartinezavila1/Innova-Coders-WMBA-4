using Microsoft.EntityFrameworkCore;
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
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                //context.Database.Migrate();
                //To randomly generate data
                Random random = new Random();

                //Cities 
                if (!context.Cities.Any())
                {
                    var cities = new List<City>
                    {
                        new City { ID = 1, CityName = "Unknow" },
                        new City { ID = 2, CityName = "Toronto" },
                        new City { ID = 3, CityName = "Vancouver" },
                        new City { ID = 4, CityName = "Montreal" },
                        new City { ID = 5, CityName = "Calgary" },
                        new City { ID = 6, CityName = "Edmonton" },
                        new City { ID = 7, CityName = "Ottawa" },
                        new City { ID = 8, CityName = "Winnipeg" },
                        new City { ID = 9, CityName = "Quebec City" },
                        new City { ID = 10, CityName = "Hamilton" },
                        new City { ID = 11, CityName = "Halifax" },
                        new City { ID = 12, CityName = "London" },
                        new City { ID = 13, CityName = "Victoria" },
                        new City { ID = 14, CityName = "Saskatoon" }

                    };
                    context.Cities.AddRange(cities);
                    context.SaveChanges();
                }


                // Club
                if (!context.Clubs.Any())
                {
                    var clubs = new List<Club>
                    {
                        new Club { ID = 1, ClubName = "Welland Minor Baseball Association", Status=true, CityID = 1 },


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
                        new Team { ID = 1, Name = "Whitecaps", DivisionID = 1 },
                        new Team { ID = 2, Name = "Bisons", DivisionID = 1 },
                        new Team { ID = 3, Name = "Trash Pandas", DivisionID = 1 },
                        new Team { ID = 4, Name = "Dragons",  DivisionID = 1 },
                        new Team { ID = 5, Name = "Bananas", DivisionID = 1 },
                        new Team { ID = 6, Name = "Iron Birds", DivisionID = 1 }

                    };

                    context.Teams.AddRange(teams);
                    context.SaveChanges();
                }
                // Roles 
                if (!context.Roles.Any())
                {
                    var roles = new List<Role>
                    {
                        new Role { ID = 1, Description = "Coach" },
                        new Role { ID = 2, Description = "Scorekeeper" },

                    };

                    context.Roles.AddRange(roles);
                    context.SaveChanges();
                }

                // Staff
                if (!context.Staff.Any())
                {
                    var staffMembers = new List<Staff>
                    {
                        new Staff { FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", RoleId = 1 },
                        new Staff { FirstName = "Jane", LastName = "Smith", Email = "jane.smith@example.com" , RoleId = 1 },
                        new Staff { FirstName = "Robert", LastName = "Johnson", Email = "robert.johnson@example.com" , RoleId = 1},
                        new Staff { FirstName = "Emily", LastName = "Williams", Email = "emily.williams@example.com", RoleId = 1 },
                        new Staff { FirstName = "Michael", LastName = "Brown", Email = "michael.brown@example.com" , RoleId = 1},
                        new Staff { FirstName = "Olivia", LastName = "Miller", Email = "olivia.miller@example.com" , RoleId = 1},
                        new Staff { FirstName = "William", LastName = "Davis", Email = "william.davis@example.com" , RoleId = 1},
                        new Staff { FirstName = "Sophia", LastName = "Garcia", Email = "sophia.garcia@example.com" , RoleId = 1},
                        new Staff { FirstName = "James", LastName = "Martinez", Email = "james.martinez@example.com", RoleId = 1 },
                        new Staff { FirstName = "Emma", LastName = "Jackson", Email = "emma.jackson@example.com" , RoleId = 1},
                        new Staff { FirstName = "Alexander", LastName = "Taylor", Email = "alexander.taylor@example.com", RoleId = 2},
                        new Staff { FirstName = "Ava", LastName = "Anderson", Email = "ava.anderson@example.com", RoleId = 2},
                        new Staff { FirstName = "Daniel", LastName = "Thomas", Email = "daniel.thomas@example.com" , RoleId = 2},
                        new Staff { FirstName = "Mia", LastName = "Moore", Email = "mia.moore@example.com" , RoleId = 2},
                        new Staff { FirstName = "Ethan", LastName = "Clark", Email = "ethan.clark@example.com" , RoleId = 2},
                        new Staff { FirstName = "Isabella", LastName = "Lewis", Email = "isabella.lewis@example.com" , RoleId = 2},
                        new Staff { FirstName = "Benjamin", LastName = "Hill", Email = "benjamin.hill@example.com", RoleId = 2 },
                        new Staff { FirstName = "Avery", LastName = "King", Email = "avery.king@example.com" , RoleId = 2},
                        new Staff { FirstName = "Ryan", LastName = "Cooper", Email = "ryan.cooper@example.com" , RoleId = 2},
                        new Staff { FirstName = "Nora", LastName = "Baker", Email = "nora.baker@example.com" , RoleId = 2}
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
                        new TeamStaff { TeamID = 1, StaffID = 7 },
                        new TeamStaff { TeamID = 2, StaffID = 8 },
                        new TeamStaff { TeamID = 1, StaffID = 9 },
                        new TeamStaff { TeamID = 2, StaffID = 10 },
                        new TeamStaff { TeamID = 3, StaffID = 11 },
                        new TeamStaff { TeamID = 4, StaffID = 12 },
                        new TeamStaff { TeamID = 5, StaffID = 13 },
                        new TeamStaff { TeamID = 6, StaffID = 14 },
                        new TeamStaff { TeamID = 1, StaffID = 15 },
                        new TeamStaff { TeamID = 2, StaffID = 16 },
                        new TeamStaff { TeamID = 1, StaffID = 17 },
                        new TeamStaff { TeamID = 1, StaffID = 18 },
                        new TeamStaff { TeamID = 1, StaffID = 19 },
                        new TeamStaff { TeamID = 2, StaffID = 20 }
                    };

                    context.TeamStaff.AddRange(teamStaff);
                    context.SaveChanges();
                }

                // Seasons 
                if (!context.Seasons.Any())
                {
                    var seasons = new List<Season>
                    {
                        new Season { ID = 1, SeasonCode = "2024", SeasonName = "Winter 2024" },

                    };

                    context.Seasons.AddRange(seasons);
                    context.SaveChanges();
                }

                // GameTypes 
                if (!context.GameTypes.Any())
                {
                    var gameTypes = new List<GameType>
                    {
                        new GameType { ID = 1, Description = "Regular Season" },
                        new GameType { ID = 2, Description = "Playoff" }
                    };

                    context.GameTypes.AddRange(gameTypes);
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

                // Positions
                if (!context.Positions.Any())
                {
                    var positions = new List<Position>
                    {
                        new Position { ID = 1, PositionCode = "U", PositionName = "Undefined" },
                        new Position { ID = 2, PositionCode = "P", PositionName = "Pitcher" },
                        new Position { ID = 3, PositionCode = "C", PositionName = "Catcher" },
                        new Position { ID = 4, PositionCode = "1B", PositionName = "First Base" },
                        new Position { ID = 5, PositionCode = "2B", PositionName = "Second Base" },
                        new Position { ID = 6, PositionCode = "SS", PositionName = "Shortstop" },
                        new Position { ID = 7, PositionCode = "3B", PositionName = "Third Base" },
                        new Position { ID = 8, PositionCode = "LF", PositionName = "Left Field" },
                        new Position { ID = 9, PositionCode = "CF", PositionName = "Center Field" },
                        new Position { ID = 10, PositionCode = "RF", PositionName = "Right Field" },
                        new Position { ID = 11, PositionCode = "DH", PositionName = "Designated Hitter" }

                    };

                    context.Positions.AddRange(positions);
                    context.SaveChanges();
                }

                // Games 
                if (!context.Games.Any())
                {
                    var games = new List<Game>
                    {
                        new Game { ID = 1, Date = new DateTime(2024, 1, 15), Status=true, LocationID = 1, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 2, Date = new DateTime(2024, 2, 15), Status=true, LocationID = 2, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 3, Date = new DateTime(2024, 3, 15), Status=true, LocationID = 3, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 4, Date = new DateTime(2024, 4, 15), Status=true, LocationID = 4, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 5, Date = new DateTime(2024, 5, 15), Status=true, LocationID = 5, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 6, Date = new DateTime(2024, 6, 10), Status=true, LocationID = 1, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 7, Date = new DateTime(2024, 7, 10), Status=true, LocationID = 2, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 8, Date = new DateTime(2024, 8, 10), Status=true, LocationID = 3, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 9, Date = new DateTime(2024, 9, 10), Status=true, LocationID = 4, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 10, Date = new DateTime(2024, 10, 10), Status=true, LocationID = 5, SeasonID = 1, GameTypeID=1},

                        new Game { ID = 11, Date = new DateTime(2024, 1, 15), Status=true, LocationID = 1, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 12, Date = new DateTime(2024, 2, 15), Status=true, LocationID = 2, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 13, Date = new DateTime(2024, 3, 15), Status=true, LocationID = 3, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 14, Date = new DateTime(2024, 4, 15), Status=true, LocationID = 4, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 15, Date = new DateTime(2024, 5, 15), Status=true, LocationID = 5, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 16, Date = new DateTime(2024, 6, 10), Status=true, LocationID = 1, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 17, Date = new DateTime(2024, 7, 10), Status=true, LocationID = 2, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 18, Date = new DateTime(2024, 8, 10), Status=true, LocationID = 3, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 19, Date = new DateTime(2024, 9, 10), Status=true, LocationID = 4, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 20, Date = new DateTime(2024, 10, 10), Status=true, LocationID = 5, SeasonID = 1, GameTypeID=1},

                        new Game { ID = 21, Date = new DateTime(2024, 1, 15), Status=true, LocationID = 1, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 22, Date = new DateTime(2024, 2, 15), Status=true, LocationID = 2, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 23, Date = new DateTime(2024, 3, 15), Status=true, LocationID = 3, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 24, Date = new DateTime(2024, 4, 15), Status=true, LocationID = 4, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 25, Date = new DateTime(2024, 5, 15), Status=true, LocationID = 5, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 26, Date = new DateTime(2024, 6, 10), Status=true, LocationID = 1, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 27, Date = new DateTime(2024, 7, 10), Status=true, LocationID = 2, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 28, Date = new DateTime(2024, 8, 10), Status=true, LocationID = 3, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 29, Date = new DateTime(2024, 9, 10), Status=true, LocationID = 4, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 30, Date = new DateTime(2024, 10, 10), Status=true, LocationID = 5, SeasonID = 1, GameTypeID=1},

                        new Game { ID = 31, Date = new DateTime(2024, 1, 15), Status=true, LocationID = 1, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 32, Date = new DateTime(2024, 2, 15), Status=true, LocationID = 2, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 33, Date = new DateTime(2024, 3, 15), Status=true, LocationID = 3, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 34, Date = new DateTime(2024, 4, 15), Status=true, LocationID = 4, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 35, Date = new DateTime(2024, 5, 15), Status=true, LocationID = 5, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 36, Date = new DateTime(2024, 6, 10), Status=true, LocationID = 1, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 37, Date = new DateTime(2024, 7, 10), Status=true, LocationID = 2, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 38, Date = new DateTime(2024, 8, 10), Status=true, LocationID = 3, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 39, Date = new DateTime(2024, 9, 10), Status=true, LocationID = 4, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 40, Date = new DateTime(2024, 10, 10), Status=true, LocationID = 5, SeasonID = 1, GameTypeID=1},

                        new Game { ID = 41, Date = new DateTime(2024, 1, 15), Status=true, LocationID = 1, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 42, Date = new DateTime(2024, 2, 15), Status=true, LocationID = 2, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 43, Date = new DateTime(2024, 3, 15), Status=true, LocationID = 3, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 44, Date = new DateTime(2024, 4, 15), Status=true, LocationID = 4, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 45, Date = new DateTime(2024, 5, 15), Status=true, LocationID = 5, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 46, Date = new DateTime(2024, 6, 10), Status=true, LocationID = 1, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 47, Date = new DateTime(2024, 7, 10), Status=true, LocationID = 2, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 48, Date = new DateTime(2024, 8, 10), Status=true, LocationID = 3, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 49, Date = new DateTime(2024, 9, 10), Status=true, LocationID = 4, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 50, Date = new DateTime(2024, 10, 10), Status=true, LocationID = 5, SeasonID = 1, GameTypeID=1},

                        new Game { ID = 51, Date = new DateTime(2024, 1, 15), Status=true, LocationID = 1, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 52, Date = new DateTime(2024, 2, 15), Status=true, LocationID = 2, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 53, Date = new DateTime(2024, 3, 15), Status=true, LocationID = 3, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 54, Date = new DateTime(2024, 4, 15), Status=true, LocationID = 4, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 55, Date = new DateTime(2024, 5, 15), Status=true, LocationID = 5, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 56, Date = new DateTime(2024, 6, 10), Status=true, LocationID = 1, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 57, Date = new DateTime(2024, 7, 10), Status=true, LocationID = 2, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 58, Date = new DateTime(2024, 8, 10), Status=true, LocationID = 3, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 59, Date = new DateTime(2024, 9, 10), Status=true, LocationID = 4, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 60, Date = new DateTime(2024, 10, 10), Status=true, LocationID = 5, SeasonID = 1, GameTypeID=1},

                        new Game { ID = 61, Date = new DateTime(2024, 1, 15), Status=true, LocationID = 1, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 62, Date = new DateTime(2024, 2, 15), Status=true, LocationID = 2, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 63, Date = new DateTime(2024, 3, 15), Status=true, LocationID = 3, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 64, Date = new DateTime(2024, 4, 15), Status=true, LocationID = 4, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 65, Date = new DateTime(2024, 5, 15), Status=true, LocationID = 5, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 66, Date = new DateTime(2024, 6, 10), Status=true, LocationID = 1, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 67, Date = new DateTime(2024, 7, 10), Status=true, LocationID = 2, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 68, Date = new DateTime(2024, 8, 10), Status=true, LocationID = 3, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 69, Date = new DateTime(2024, 9, 10), Status=true, LocationID = 4, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 70, Date = new DateTime(2024, 10, 10), Status=true, LocationID = 5, SeasonID = 1, GameTypeID=1},

                        new Game { ID = 71, Date = new DateTime(2024, 1, 15), Status=true, LocationID = 1, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 72, Date = new DateTime(2024, 2, 15), Status=true, LocationID = 2, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 73, Date = new DateTime(2024, 3, 15), Status=true, LocationID = 3, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 74, Date = new DateTime(2024, 4, 15), Status=true, LocationID = 4, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 75, Date = new DateTime(2024, 5, 15), Status=true, LocationID = 5, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 76, Date = new DateTime(2024, 6, 10), Status=true, LocationID = 1, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 77, Date = new DateTime(2024, 7, 10), Status=true, LocationID = 2, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 78, Date = new DateTime(2024, 8, 10), Status=true, LocationID = 3, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 79, Date = new DateTime(2024, 9, 10), Status=true, LocationID = 4, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 80, Date = new DateTime(2024, 10, 10), Status=true, LocationID = 5, SeasonID = 1, GameTypeID=1},

                                                new Game { ID = 81, Date = new DateTime(2024, 1, 15), Status=true, LocationID = 1, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 82, Date = new DateTime(2024, 2, 15), Status=true, LocationID = 2, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 83, Date = new DateTime(2024, 3, 15), Status=true, LocationID = 3, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 84, Date = new DateTime(2024, 4, 15), Status=true, LocationID = 4, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 85, Date = new DateTime(2024, 5, 15), Status=true, LocationID = 5, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 86, Date = new DateTime(2024, 6, 10), Status=true, LocationID = 1, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 87, Date = new DateTime(2024, 7, 10), Status=true, LocationID = 2, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 88, Date = new DateTime(2024, 8, 10), Status=true, LocationID = 3, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 89, Date = new DateTime(2024, 9, 10), Status=true, LocationID = 4, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 90, Date = new DateTime(2024, 10, 10), Status=true, LocationID = 5, SeasonID = 1, GameTypeID=1},

                        new Game { ID = 91, Date = new DateTime(2024, 1, 15), Status=true, LocationID = 1, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 92, Date = new DateTime(2024, 2, 15), Status=true, LocationID = 2, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 93, Date = new DateTime(2024, 3, 15), Status=true, LocationID = 3, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 94, Date = new DateTime(2024, 4, 15), Status=true, LocationID = 4, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 95, Date = new DateTime(2024, 5, 15), Status=true, LocationID = 5, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 96, Date = new DateTime(2024, 6, 10), Status=true, LocationID = 1, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 97, Date = new DateTime(2024, 7, 10), Status=true, LocationID = 2, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 98, Date = new DateTime(2024, 8, 10), Status=true, LocationID = 3, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 99, Date = new DateTime(2024, 9, 10), Status=true, LocationID = 4, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 100, Date = new DateTime(2024, 10, 10), Status=true, LocationID = 5, SeasonID = 1, GameTypeID=1},

                        new Game { ID = 101, Date = new DateTime(2024, 1, 15), Status=true, LocationID = 1, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 102, Date = new DateTime(2024, 2, 15), Status=true, LocationID = 2, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 103, Date = new DateTime(2024, 3, 15), Status=true, LocationID = 3, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 104, Date = new DateTime(2024, 4, 15), Status=true, LocationID = 4, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 105, Date = new DateTime(2024, 5, 15), Status=true, LocationID = 5, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 106, Date = new DateTime(2024, 6, 10), Status=true, LocationID = 1, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 107, Date = new DateTime(2024, 7, 10), Status=true, LocationID = 2, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 108, Date = new DateTime(2024, 8, 10), Status=true, LocationID = 3, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 109, Date = new DateTime(2024, 9, 10), Status=true, LocationID = 4, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 110, Date = new DateTime(2024, 10, 10), Status=true, LocationID = 5, SeasonID = 1, GameTypeID=1},

                        new Game { ID = 111, Date = new DateTime(2024, 1, 15), Status=true, LocationID = 1, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 112, Date = new DateTime(2024, 2, 15), Status=true, LocationID = 2, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 113, Date = new DateTime(2024, 3, 15), Status=true, LocationID = 3, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 114, Date = new DateTime(2024, 4, 15), Status=true, LocationID = 4, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 115, Date = new DateTime(2024, 5, 15), Status=true, LocationID = 5, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 116, Date = new DateTime(2024, 6, 10), Status=true, LocationID = 1, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 117, Date = new DateTime(2024, 7, 10), Status=true, LocationID = 2, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 118, Date = new DateTime(2024, 8, 10), Status=true, LocationID = 3, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 119, Date = new DateTime(2024, 9, 10), Status=true, LocationID = 4, SeasonID = 1, GameTypeID=1},
                        new Game { ID = 120, Date = new DateTime(2024, 10, 10), Status=true, LocationID = 5, SeasonID = 1, GameTypeID=1}

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

                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 7, TeamID = 3, GameID = 4 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 3, TeamID = 4, GameID = 4 },

                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 8, TeamID = 3, GameID = 5 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 6, TeamID = 4, GameID = 5 },

                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 5, TeamID = 2, GameID = 6 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 7, TeamID = 5, GameID = 6 },

                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 3, TeamID = 1, GameID = 7 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 8, TeamID = 2, GameID = 7 },

                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 6, TeamID = 3, GameID = 8 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 5, TeamID = 4, GameID = 8 },

                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 7, TeamID = 5, GameID = 9 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 3, TeamID = 6, GameID = 9 },

                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 8, TeamID = 1, GameID = 10 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 6, TeamID = 2, GameID = 10 },

                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 5, TeamID = 1, GameID = 11 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 7, TeamID = 2, GameID = 11 },

                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 3, TeamID = 3, GameID = 12 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 8, TeamID = 4, GameID = 12 },

                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 6, TeamID = 5, GameID = 13 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 5, TeamID = 6, GameID = 13 },

                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 7, TeamID = 1, GameID = 14 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 3, TeamID = 2, GameID = 14 },

                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 8, TeamID = 3, GameID = 15 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 6, TeamID = 4, GameID = 15 },

                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 5, TeamID = 5, GameID = 16 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 7, TeamID = 6, GameID = 16 },

                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 3, TeamID = 1, GameID = 17 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 8, TeamID = 2, GameID = 17 },

                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 6, TeamID = 3, GameID = 18 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 5, TeamID = 4, GameID = 18 },

                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 7, TeamID = 5, GameID = 19 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 3, TeamID = 6, GameID = 19 },

                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 8, TeamID = 1, GameID = 20 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 6, TeamID = 2, GameID = 20 },

                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 5, TeamID = 1, GameID = 21 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 7, TeamID = 2, GameID = 21 },

                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 3, TeamID = 3, GameID = 22 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 8, TeamID = 4, GameID = 22 },

                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 6, TeamID = 5, GameID = 23 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 5, TeamID = 6, GameID = 23 },

                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 7, TeamID = 1, GameID = 24 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 3, TeamID = 2, GameID = 24 },

                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 8, TeamID = 3, GameID = 25 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 6, TeamID = 4, GameID = 25 },

                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 5, TeamID = 5, GameID = 26 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 7, TeamID = 6, GameID = 26 },

                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 3, TeamID = 1, GameID = 27 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 8, TeamID = 2, GameID = 27 },

                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 6, TeamID = 3, GameID = 28 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 5, TeamID = 4, GameID = 28 },

                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 7, TeamID = 5, GameID = 29 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 3, TeamID = 6, GameID = 29 },

                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 8, TeamID = 1, GameID = 30 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 6, TeamID = 2, GameID = 30 },

                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 5, TeamID = 1, GameID = 31 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 7, TeamID = 2, GameID = 31 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 3, TeamID = 3, GameID = 32 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 8, TeamID = 4, GameID = 32 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 6, TeamID = 5, GameID = 33 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 5, TeamID = 6, GameID = 33 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 7, TeamID = 1, GameID = 34 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 3, TeamID = 2, GameID = 34 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 8, TeamID = 3, GameID = 35 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 6, TeamID = 4, GameID = 35 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 5, TeamID = 5, GameID = 36 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 7, TeamID = 6, GameID = 36 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 3, TeamID = 1, GameID = 37 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 8, TeamID = 2, GameID = 37 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 6, TeamID = 3, GameID = 38 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 5, TeamID = 4, GameID = 38 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 7, TeamID = 5, GameID = 39 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 3, TeamID = 6, GameID = 39 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 8, TeamID = 1, GameID = 40 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 6, TeamID = 2, GameID = 40 },

                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 5, TeamID = 1, GameID = 41 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 7, TeamID = 2, GameID = 41 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 3, TeamID = 3, GameID = 42 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 8, TeamID = 4, GameID = 42 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 6, TeamID = 5, GameID = 43 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 5, TeamID = 6, GameID = 43 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 7, TeamID = 1, GameID = 44 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 3, TeamID = 2, GameID = 44 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 8, TeamID = 3, GameID = 45 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 6, TeamID = 4, GameID = 45 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 5, TeamID = 5, GameID = 46 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 7, TeamID = 6, GameID = 46 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 3, TeamID = 1, GameID = 47 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 8, TeamID = 2, GameID = 47 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 6, TeamID = 3, GameID = 48 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 5, TeamID = 4, GameID = 48 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 7, TeamID = 5, GameID = 49 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 3, TeamID = 6, GameID = 49 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 8, TeamID = 1, GameID = 50 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 6, TeamID = 2, GameID = 50 },

                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 5, TeamID = 1, GameID = 51 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 7, TeamID = 2, GameID = 51 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 3, TeamID = 3, GameID = 52 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 8, TeamID = 4, GameID = 52 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 6, TeamID = 5, GameID = 53 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 5, TeamID = 6, GameID = 53 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 7, TeamID = 1, GameID = 54 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 3, TeamID = 2, GameID = 54 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 8, TeamID = 3, GameID = 55 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 6, TeamID = 4, GameID = 55 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 5, TeamID = 5, GameID = 56 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 7, TeamID = 6, GameID = 56 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 3, TeamID = 1, GameID = 57 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 8, TeamID = 2, GameID = 57 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 6, TeamID = 3, GameID = 58 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 5, TeamID = 4, GameID = 58 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 7, TeamID = 5, GameID = 59 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 3, TeamID = 6, GameID = 59 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 8, TeamID = 1, GameID = 60 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 6, TeamID = 2, GameID = 60 },

                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 5, TeamID = 1, GameID = 61 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 7, TeamID = 2, GameID = 61 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 3, TeamID = 3, GameID = 62 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 8, TeamID = 4, GameID = 62 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 6, TeamID = 5, GameID = 63 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 5, TeamID = 6, GameID = 63 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 7, TeamID = 1, GameID = 64 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 3, TeamID = 2, GameID = 64 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 8, TeamID = 3, GameID = 65 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 6, TeamID = 4, GameID = 65 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 5, TeamID = 5, GameID = 66 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 7, TeamID = 6, GameID = 66 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 3, TeamID = 1, GameID = 67 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 8, TeamID = 2, GameID = 67 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 6, TeamID = 3, GameID = 68 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 5, TeamID = 4, GameID = 68 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 7, TeamID = 5, GameID = 69 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 3, TeamID = 6, GameID = 69 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 8, TeamID = 1, GameID = 70 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 6, TeamID = 2, GameID = 70 },

                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 5, TeamID = 1, GameID = 71 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 7, TeamID = 2, GameID = 71 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 3, TeamID = 3, GameID = 72 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 8, TeamID = 4, GameID = 72 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 6, TeamID = 5, GameID = 73 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 5, TeamID = 6, GameID = 73 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 7, TeamID = 1, GameID = 74 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 3, TeamID = 2, GameID = 74 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 8, TeamID = 3, GameID = 75 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 6, TeamID = 4, GameID = 75 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 5, TeamID = 5, GameID = 76 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 7, TeamID = 6, GameID = 76 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 3, TeamID = 1, GameID = 77 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 8, TeamID = 2, GameID = 77 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 6, TeamID = 3, GameID = 78 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 5, TeamID = 4, GameID = 78 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 7, TeamID = 5, GameID = 79 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 3, TeamID = 6, GameID = 79 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 8, TeamID = 1, GameID = 80 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 6, TeamID = 2, GameID = 80 },

                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 5, TeamID = 1, GameID = 81 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 7, TeamID = 2, GameID = 81 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 3, TeamID = 3, GameID = 82 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 8, TeamID = 4, GameID = 82 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 6, TeamID = 5, GameID = 83 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 5, TeamID = 6, GameID = 83 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 7, TeamID = 1, GameID = 84 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 3, TeamID = 2, GameID = 84 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 8, TeamID = 3, GameID = 85 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 6, TeamID = 4, GameID = 85 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 5, TeamID = 5, GameID = 86 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 7, TeamID = 6, GameID = 86 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 3, TeamID = 1, GameID = 87 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 8, TeamID = 2, GameID = 87 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 6, TeamID = 3, GameID = 88 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 5, TeamID = 4, GameID = 88 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 7, TeamID = 5, GameID = 89 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 3, TeamID = 6, GameID = 89 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 8, TeamID = 1, GameID = 90 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 6, TeamID = 2, GameID = 90 },

                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 5, TeamID = 1, GameID = 91 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 7, TeamID = 2, GameID = 91 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 3, TeamID = 3, GameID = 92 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 8, TeamID = 4, GameID = 92 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 6, TeamID = 5, GameID = 93 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 5, TeamID = 6, GameID = 93 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 7, TeamID = 1, GameID = 94 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 3, TeamID = 2, GameID = 94 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 8, TeamID = 3, GameID = 95 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 6, TeamID = 4, GameID = 95 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 5, TeamID = 5, GameID = 96 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 7, TeamID = 6, GameID = 96 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 3, TeamID = 1, GameID = 97 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 8, TeamID = 2, GameID = 97 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 6, TeamID = 3, GameID = 98 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 5, TeamID = 4, GameID = 98 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 7, TeamID = 5, GameID = 99 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 3, TeamID = 6, GameID = 99 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 8, TeamID = 1, GameID = 100 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 6, TeamID = 2, GameID = 100 },

                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 5, TeamID = 1, GameID = 101 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 7, TeamID = 2, GameID = 101 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 3, TeamID = 3, GameID = 102 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 8, TeamID = 4, GameID = 102 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 6, TeamID = 5, GameID = 103 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 5, TeamID = 6, GameID = 103 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 7, TeamID = 1, GameID = 104 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 3, TeamID = 2, GameID = 104 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 8, TeamID = 3, GameID = 105 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 6, TeamID = 4, GameID = 105 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 5, TeamID = 5, GameID = 106 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 7, TeamID = 6, GameID = 106 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 3, TeamID = 1, GameID = 107 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 8, TeamID = 2, GameID = 107 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 6, TeamID = 3, GameID = 108 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 5, TeamID = 4, GameID = 108 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 7, TeamID = 5, GameID = 109 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 3, TeamID = 6, GameID = 109 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 8, TeamID = 1, GameID = 110 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 6, TeamID = 2, GameID = 110 },

                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 5, TeamID = 1, GameID = 111 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 7, TeamID = 2, GameID = 111 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 3, TeamID = 3, GameID = 112 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 8, TeamID = 4, GameID = 112 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 6, TeamID = 5, GameID = 113 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 5, TeamID = 6, GameID = 113 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 7, TeamID = 1, GameID = 114 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 3, TeamID = 2, GameID = 114 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 8, TeamID = 3, GameID = 115 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 6, TeamID = 4, GameID = 115 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 5, TeamID = 5, GameID = 116 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 7, TeamID = 6, GameID = 116 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 3, TeamID = 1, GameID = 117 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 8, TeamID = 2, GameID = 117 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 6, TeamID = 3, GameID = 118 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 5, TeamID = 4, GameID = 118 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 7, TeamID = 5, GameID = 119 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 3, TeamID = 6, GameID = 119 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 8, TeamID = 1, GameID = 120 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 6, TeamID = 2, GameID = 120 }
                    };

                    context.TeamGame.AddRange(teamGames);
                    context.SaveChanges();
                }

                // Players
                if (!context.Players.Any())
                {
                    var players = new List<Player>
                    {
                        //team1
                        new Player { ID = 1, TeamID = 1, MemberID = "M0000001", FirstName = "John", LastName = "Doe", JerseyNumber = "10" },
                        new Player { ID = 2, TeamID = 1, MemberID = "M0000002", FirstName = "Jane", LastName = "Smith", JerseyNumber = "22" },
                        new Player { ID = 3, TeamID = 1, MemberID = "M0000003", FirstName = "Michael", LastName = "Johnson", JerseyNumber = "5" },
                        new Player { ID = 4, TeamID = 1, MemberID = "M0000004", FirstName = "Emily", LastName = "Williams", JerseyNumber = "15" },
                        new Player { ID = 5, TeamID = 1, MemberID = "M0000005", FirstName = "David", LastName = "Brown", JerseyNumber = "7" },
                        new Player { ID = 6, TeamID = 1, MemberID = "M0000006", FirstName = "Olivia", LastName = "Jones", JerseyNumber = "33" },
                        new Player { ID = 7, TeamID = 1, MemberID = "M0000007", FirstName = "Andrew", LastName = "Miller", JerseyNumber = "19" },
                        new Player { ID = 8, TeamID = 1, MemberID = "M0000008", FirstName = "Sophia", LastName = "Davis", JerseyNumber = "8" },
                        new Player { ID = 9, TeamID = 1, MemberID = "M0000009", FirstName = "William", LastName = "Taylor", JerseyNumber = "3" },
                        new Player { ID = 10, TeamID = 1, MemberID = "M0000010", FirstName = "Emma", LastName = "Anderson", JerseyNumber = "25" },
                        new Player { ID = 11, TeamID = 1, MemberID = "M0000011", FirstName = "Liam", LastName = "Moore", JerseyNumber = "12" },
                        new Player { ID = 12, TeamID = 1, MemberID = "M0000012", FirstName = "Ava", LastName = "White", JerseyNumber = "14" },
                        //team2
                        new Player { ID = 13, TeamID = 2, MemberID = "M0000013", FirstName = "Ethan", LastName = "Harris", JerseyNumber = "9" },
                        new Player { ID = 14, TeamID = 2, MemberID = "M0000014", FirstName = "Chloe", LastName = "Martin", JerseyNumber = "18" },
                        new Player { ID = 15, TeamID = 2, MemberID = "M0000015", FirstName = "Benjamin", LastName = "Clark", JerseyNumber = "6" },
                        new Player { ID = 16, TeamID = 2, MemberID = "M0000016", FirstName = "Grace", LastName = "Lewis", JerseyNumber = "20" },
                        new Player { ID = 17, TeamID = 2, MemberID = "M0000017", FirstName = "Lucas", LastName = "Walker", JerseyNumber = "11" },
                        new Player { ID = 18, TeamID = 2, MemberID = "M0000018", FirstName = "Lily", LastName = "Allen", JerseyNumber = "23" },
                        new Player { ID = 19, TeamID = 2, MemberID = "M0000019", FirstName = "Henry", LastName = "Young", JerseyNumber = "4" },
                        new Player { ID = 20, TeamID = 2, MemberID = "M0000020", FirstName = "Sophie", LastName = "Turner", JerseyNumber = "30" },
                        new Player { ID = 21, TeamID = 2, MemberID = "M0000021", FirstName = "Owen", LastName = "Baker", JerseyNumber = "17" },
                        new Player { ID = 22, TeamID = 2, MemberID = "M0000022", FirstName = "Zoe", LastName = "Cooper", JerseyNumber = "21" },
                        new Player { ID = 23, TeamID = 2, MemberID = "M0000023", FirstName = "Elijah", LastName = "Perry", JerseyNumber = "2" },
                        new Player { ID = 24, TeamID = 2, MemberID = "M0000024", FirstName = "Mia", LastName = "Fisher", JerseyNumber = "16" },
                        //team3
                        new Player { ID = 25, TeamID = 3, MemberID = "M0000025", FirstName = "Caleb", LastName = "Reed", JerseyNumber = "13" },
                        new Player { ID = 26, TeamID = 3, MemberID = "M0000026", FirstName = "Avery", LastName = "Murphy", JerseyNumber = "29" },
                        new Player { ID = 27, TeamID = 3, MemberID = "M0000027", FirstName = "Gabriel", LastName = "Bell", JerseyNumber = "24" },
                        new Player { ID = 28, TeamID = 3, MemberID = "M0000028", FirstName = "Scarlett", LastName = "Ward", JerseyNumber = "31" },
                        new Player { ID = 29, TeamID = 3, MemberID = "M0000029", FirstName = "Isaac", LastName = "Cox", JerseyNumber = "1" },
                        new Player { ID = 30, TeamID = 3, MemberID = "M0000030", FirstName = "Hannah", LastName = "Lopez", JerseyNumber = "26" },
                        new Player { ID = 31, TeamID = 3, MemberID = "M0000031", FirstName = "Jackson", LastName = "Rogers", JerseyNumber = "22" },
                        new Player { ID = 32, TeamID = 3, MemberID = "M0000032", FirstName = "Aria", LastName = "Hill", JerseyNumber = "8" },
                        new Player { ID = 33, TeamID = 3, MemberID = "M0000033", FirstName = "Lincoln", LastName = "Wright", JerseyNumber = "14" },
                        new Player { ID = 34, TeamID = 3, MemberID = "M0000034", FirstName = "Ella", LastName = "Barnes", JerseyNumber = "19" },
                        //team4
                        new Player { ID = 35, TeamID = 4, MemberID = "M0000035", FirstName = "Mason", LastName = "Ferguson", JerseyNumber = "10" },
                        new Player { ID = 36, TeamID = 4, MemberID = "M0000036", FirstName = "Aurora", LastName = "Simmons", JerseyNumber = "28" },
                        new Player { ID = 37, TeamID = 4, MemberID = "M0000037", FirstName = "Landon", LastName = "Powell", JerseyNumber = "6" },
                        new Player { ID = 38, TeamID = 4, MemberID = "M0000038", FirstName = "Piper", LastName = "Watson", JerseyNumber = "23" },
                        new Player { ID = 39, TeamID = 4, MemberID = "M0000039", FirstName = "Logan", LastName = "Hughes", JerseyNumber = "9" },
                        new Player { ID = 40, TeamID = 4, MemberID = "M0000040", FirstName = "Aaliyah", LastName = "Fletcher", JerseyNumber = "20" },
                        new Player { ID = 41, TeamID = 4, MemberID = "M0000041", FirstName = "Mason", LastName = "Ferguson", JerseyNumber = "11" },
                        new Player { ID = 42, TeamID = 4, MemberID = "M0000042", FirstName = "Aurora", LastName = "Simmons", JerseyNumber = "26" },
                        new Player { ID = 43, TeamID = 4, MemberID = "M0000043", FirstName = "Landon", LastName = "Powell", JerseyNumber = "2" },
                        new Player { ID = 44, TeamID = 4, MemberID = "M0000044", FirstName = "Piper", LastName = "Watson", JerseyNumber = "1" },
                        new Player { ID = 45, TeamID = 4, MemberID = "M0000045", FirstName = "Logan", LastName = "Hughes", JerseyNumber = "4" },

                    };

                    context.Players.AddRange(players);
                    context.SaveChanges();

                }

                // GameLineUps
                if (!context.GameLineUps.Any())
                {
                    var gameLineUps = new List<GameLineUp>
                    {
                        //Game 1
                        new GameLineUp { ID = 1, BattingOrder = 1, GameID = 1, PlayerID = 1, TeamID=1},
                        new GameLineUp { ID = 2, BattingOrder = 2, GameID = 1, PlayerID = 2, TeamID=1},
                        new GameLineUp { ID = 3, BattingOrder = 3, GameID = 1, PlayerID = 3, TeamID=1},
                        new GameLineUp { ID = 4, BattingOrder = 4, GameID = 1, PlayerID = 4, TeamID=1},

                        new GameLineUp { ID = 5, BattingOrder = 1, GameID = 1, PlayerID = 13, TeamID=2},
                        new GameLineUp { ID = 6, BattingOrder = 2, GameID = 1, PlayerID = 14, TeamID=2},
                        new GameLineUp { ID = 7, BattingOrder = 3, GameID = 1, PlayerID = 15, TeamID=2},
                        new GameLineUp { ID = 8, BattingOrder = 4, GameID = 1, PlayerID = 16, TeamID=2},

                        //Game 4
                        new GameLineUp { ID = 9, BattingOrder = 1, GameID = 4,  PlayerID = 25, TeamID=3},
                        new GameLineUp { ID = 10, BattingOrder = 2, GameID = 4, PlayerID = 26, TeamID=3},

                        new GameLineUp { ID = 11, BattingOrder = 1, GameID = 4, PlayerID = 35, TeamID=4},
                        new GameLineUp { ID = 12, BattingOrder = 2, GameID = 4, PlayerID = 36, TeamID=4},

                        //Game 6
                        new GameLineUp { ID = 13, BattingOrder = 1, GameID = 6, PlayerID = 1, TeamID=2},
                        new GameLineUp { ID = 14, BattingOrder = 2, GameID = 6, PlayerID = 2, TeamID=2},
                        new GameLineUp { ID = 15, BattingOrder = 3, GameID = 6, PlayerID = 3, TeamID=2},
                        new GameLineUp { ID = 16, BattingOrder = 4, GameID = 6, PlayerID = 4, TeamID=2},
                        new GameLineUp { ID = 17, BattingOrder = 8, GameID = 8, PlayerID = 5, TeamID=2},
                        new GameLineUp { ID = 18, BattingOrder = 9, GameID = 8, PlayerID = 6, TeamID=2},
                        new GameLineUp { ID = 19, BattingOrder = 1, GameID = 9, PlayerID = 7, TeamID=2},
                        new GameLineUp { ID = 20, BattingOrder = 2, GameID = 9, PlayerID = 8, TeamID=2}

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