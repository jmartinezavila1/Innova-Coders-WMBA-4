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
                    // DivisionID 1
                    new Team { ID = 1, Name = "Whitecaps", DivisionID = 1 },
                    new Team { ID = 2, Name = "Bisons", DivisionID = 1 },
                    new Team { ID = 3, Name = "Trash Pandas", DivisionID = 1 },
                    new Team { ID = 4, Name = "Dragons",  DivisionID = 1 },
                    new Team { ID = 5, Name = "Bananas", DivisionID = 1 },
                    new Team { ID = 6, Name = "Iron Birds", DivisionID = 1 },
    
                    // DivisionID 2
                    new Team { ID = 7, Name = "Bambinos", DivisionID = 2 },
                    new Team { ID = 8, Name = "Raptors", DivisionID = 2 },
                    new Team { ID = 9, Name = "Falcons", DivisionID = 2 },
                    new Team { ID = 10, Name = "Cadets", DivisionID = 2 },
                    new Team { ID = 11, Name = "Mariners", DivisionID = 2 },
    
                    // DivisionID 3
                    new Team { ID = 12, Name = "Thunderbirds", DivisionID = 3 },
                    new Team { ID = 13, Name = "Bears", DivisionID = 3 },
                    new Team { ID = 14, Name = "Rockies", DivisionID = 3 },
                    new Team { ID = 15, Name = "Hawks", DivisionID = 3 },
                    new Team { ID = 16, Name = "Wildcats", DivisionID = 3 },
    
                    // DivisionID 4
                    new Team { ID = 17, Name = "Wolves", DivisionID = 4 },
                    new Team { ID = 18, Name = "Panthers", DivisionID = 4 },
                    new Team { ID = 19, Name = "Polar Bears", DivisionID = 4 },
                    new Team { ID = 20, Name = "Penguins", DivisionID = 4 },
                    new Team { ID = 21, Name = "Eagles", DivisionID = 4 },
    
                    // DivisionID 5
                    new Team { ID = 22, Name = "Lions", DivisionID = 5 },
                    new Team { ID = 23, Name = "Mustangs", DivisionID = 5 },
                    new Team { ID = 24, Name = "Huskies", DivisionID = 5 },
                    new Team { ID = 25, Name = "Dolphins", DivisionID = 5 },
                    new Team { ID = 26, Name = "Sharks", DivisionID = 5 }
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
                        new Season { ID = 1, SeasonCode = "2024", SeasonName = "Summer 2024" },

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
                        new Location { ID = 10, LocationName = "Scotiabank Centre", CityID = 10 },
                        new Location { ID = 11, LocationName = "Memorial Park (Diamond 1)", CityID = 1 },
                        new Location { ID = 12, LocationName = "Memorial Park (Diamond 2)", CityID = 1 },
                        new Location { ID = 13, LocationName = "Memorial Park (Diamond 3)", CityID = 1 },
                        new Location { ID = 14, LocationName = "Memorial Park (Diamond 4)", CityID = 1 },
                        new Location { ID = 15, LocationName = "Memorial Park (Diamond 5)", CityID = 1 },
                        new Location { ID = 16, LocationName = "Memorial Park (Diamond 6)", CityID = 1 },
                        new Location { ID = 17, LocationName = "Memorial Park (Diamond 7)", CityID = 1 }
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
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 1, GameID = 1 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 2, GameID = 1 },     
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 3, GameID = 2 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 4, GameID = 2 },                                                                                         
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 5, GameID = 3 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 6, GameID = 3 },                                                                                         
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 7, GameID = 4 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 8, GameID = 4 },                                                                                        
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 9, GameID = 5 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 10, GameID = 5 },                                                                                         
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 11, GameID = 6 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 12, GameID = 6 },                                                                                        
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 13, GameID = 7 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 14, GameID = 7 },                                                                                         
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 15, GameID = 8 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 16, GameID = 8 },                                                                                         
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 17, GameID = 9 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 18, GameID = 9 },                                                                                         
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 19, GameID = 10 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 20, GameID = 10 },                                                                                         
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 21, GameID = 11 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 22, GameID = 11 },                                                                                         
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 23, GameID = 12 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 24, GameID = 12 },                                                                                         
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 25, GameID = 13 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 26, GameID = 13 },                                                                                         
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 1, GameID = 14 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 2, GameID = 14 },                                                                                         
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 3, GameID = 15 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 4, GameID = 15 },                                                                                         
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 5, GameID = 16 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 6, GameID = 16 },                                                                                         
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 7, GameID = 17 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 8, GameID = 17 },                                                                                         
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 9, GameID = 18 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 10, GameID = 18 },                                                                                         
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 11, GameID = 19 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 12, GameID = 19 },                                                                                         
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 13, GameID = 20 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 14, GameID = 20 },                                                                                         
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 15, GameID = 21 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 16, GameID = 21 },                                                                                         
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 17, GameID = 22 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 18, GameID = 22 },                                                                                         
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 19, GameID = 23 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 20, GameID = 23 },                                                                                         
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 21, GameID = 24 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 22, GameID = 24 },                                                                                         
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 23, GameID = 25 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 24, GameID = 25 },                                                                                         
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 25, GameID = 26 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 26, GameID = 26 },                                                                                         
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 1, GameID = 27 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 2, GameID = 27 },                                                                                         
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 3, GameID = 28 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 4, GameID = 28 },                                                                                         
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 5, GameID = 29 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 6, GameID = 29 },                                                                                         
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 7, GameID = 30 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 8, GameID = 30 },                                                                                         
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 9, GameID = 31 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 10, GameID = 31 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 11, GameID = 32 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 12, GameID = 32 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 13, GameID = 33 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 14, GameID = 33 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 15, GameID = 34 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 16, GameID = 34 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 17, GameID = 35 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 18, GameID = 35 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 19, GameID = 36 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 20, GameID = 36 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 21, GameID = 37 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 22, GameID = 37 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 23, GameID = 38 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 24, GameID = 38 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 25, GameID = 39 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 26, GameID = 39 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 1, GameID = 40 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 2, GameID = 40 },                                                                                         
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 3, GameID = 41 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 4, GameID = 41 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 5, GameID = 42 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 6, GameID = 42 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 7, GameID = 43 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 8, GameID = 43 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 9, GameID = 44 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 10, GameID = 44 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 11, GameID = 45 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 12, GameID = 45 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 13, GameID = 46 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 14, GameID = 46 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 15, GameID = 47 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 16, GameID = 47 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 17, GameID = 48 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 18, GameID = 48 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 19, GameID = 49 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 20, GameID = 49 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 21, GameID = 50 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 22, GameID = 50 },                                                                                         
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 23, GameID = 51 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 24, GameID = 51 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 25, GameID = 52 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 26, GameID = 52 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 1, GameID = 53 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 2, GameID = 53 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 3, GameID = 54 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 4, GameID = 54 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 5, GameID = 55 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 6, GameID = 55 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 7, GameID = 56 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 8, GameID = 56 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 9, GameID = 57 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 10, GameID = 57 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 11, GameID = 58 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 12, GameID = 58 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 13, GameID = 59 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 14, GameID = 59 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 15, GameID = 60 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 16, GameID = 60 },                                                                                         
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 17, GameID = 61 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 18, GameID = 61 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 19, GameID = 62 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 20, GameID = 62 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 21, GameID = 63 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 22, GameID = 63 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 23, GameID = 64 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 24, GameID = 64 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 25, GameID = 65 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 26, GameID = 65 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 1, GameID = 66 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 2, GameID = 66 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 3, GameID = 67 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 4, GameID = 67 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 5, GameID = 68 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 6, GameID = 68 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 7, GameID = 69 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 8, GameID = 69 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 9, GameID = 70 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 10, GameID = 70 },                                                                                        
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 11, GameID = 71 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 12, GameID = 71 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 13, GameID = 72 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 14, GameID = 72 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 15, GameID = 73 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 16, GameID = 73 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 17, GameID = 74 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 18, GameID = 74 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 19, GameID = 75 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 20, GameID = 75 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 21, GameID = 76 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 22, GameID = 76 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 23, GameID = 77 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 24, GameID = 77 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 25, GameID = 78 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 26, GameID = 78 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 1, GameID = 79 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 2, GameID = 79 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 3, GameID = 80 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 4, GameID = 80 },                                                                                         
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 5, GameID = 81 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 6, GameID = 81 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 7, GameID = 82 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 8, GameID = 82 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 9, GameID = 83 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 10, GameID = 83 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 11, GameID = 84 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 12, GameID = 84 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 13, GameID = 85 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 14, GameID = 85 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 15, GameID = 86 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 16, GameID = 86 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 17, GameID = 87 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 18, GameID = 87 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 19, GameID = 88 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 20, GameID = 88 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 21, GameID = 89 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 22, GameID = 89 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 23, GameID = 90 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 24, GameID = 90 },                                                                                         
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 25, GameID = 91 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 26, GameID = 91 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 1, GameID = 92 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 2, GameID = 92 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 3, GameID = 93 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 4, GameID = 93 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 5, GameID = 94 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 6, GameID = 94 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 7, GameID = 95 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 8, GameID = 95 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 9, GameID = 96 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 10, GameID = 96 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 11, GameID = 97 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 12, GameID = 97 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 13, GameID = 98 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 14, GameID = 98 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 15, GameID = 99 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 16, GameID = 99 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 17, GameID = 100 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 18, GameID = 100 },                                                                                         
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 19, GameID = 101 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 20, GameID = 101 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 21, GameID = 102 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 22, GameID = 102 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 23, GameID = 103 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 24, GameID = 103 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 25, GameID = 104 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 26, GameID = 104 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 1, GameID = 105 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 2, GameID = 105 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 3, GameID = 106 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 4, GameID = 106 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 5, GameID = 107 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 6, GameID = 107 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 7, GameID = 108 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 8, GameID = 108 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 9, GameID = 109 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 10, GameID = 109 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 11, GameID = 110 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 12, GameID = 110 },                                                                                         
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 13, GameID = 111 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 14, GameID = 111 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 15, GameID = 112 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 16, GameID = 112 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 17, GameID = 113 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 18, GameID = 113 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 19, GameID = 114 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 20, GameID = 114 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 21, GameID = 115 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 22, GameID = 115 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 23, GameID = 116 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 24, GameID = 116 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 25, GameID = 117 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 26, GameID = 117 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 1, GameID = 118 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 2, GameID = 118 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 3, GameID = 119 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 4, GameID = 119 },
                        new TeamGame { IsHomeTeam = true, IsVisitorTeam = false, score = 0, TeamID = 5, GameID = 120 },
                        new TeamGame { IsHomeTeam = false, IsVisitorTeam = true, score = 0, TeamID = 6, GameID = 120 }
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
                        new Player { ID = 32, TeamID = 3, MemberID = "M0000033", FirstName = "Lincoln", LastName = "Wright", JerseyNumber = "14" },
                        new Player { ID = 33, TeamID = 3, MemberID = "M0000034", FirstName = "Ella", LastName = "Barnes", JerseyNumber = "19" },
                        
                        //team4
                        new Player { ID = 34, TeamID = 4, MemberID = "M0000035", FirstName = "Mason", LastName = "Ferguson", JerseyNumber = "10" },                        
                        new Player { ID = 35, TeamID = 4, MemberID = "M0000037", FirstName = "Landon", LastName = "Powell", JerseyNumber = "6" },
                        new Player { ID = 36, TeamID = 4, MemberID = "M0000038", FirstName = "Piper", LastName = "Watson", JerseyNumber = "23" },
                        new Player { ID = 37, TeamID = 4, MemberID = "M0000039", FirstName = "Logan", LastName = "Hudson", JerseyNumber = "9" },
                        new Player { ID = 38, TeamID = 4, MemberID = "M0000040", FirstName = "Aaliyah", LastName = "Fletcher", JerseyNumber = "20" },
                        new Player { ID = 39, TeamID = 4, MemberID = "M0000041", FirstName = "Mason", LastName = "Drguson", JerseyNumber = "11" },
                        new Player { ID = 40, TeamID = 4, MemberID = "M0000042", FirstName = "Aurora", LastName = "Simmons", JerseyNumber = "26" },
                        new Player { ID = 41, TeamID = 4, MemberID = "M0000043", FirstName = "Landon", LastName = "Powellshell", JerseyNumber = "2" },
                        new Player { ID = 42, TeamID = 4, MemberID = "M0000044", FirstName = "Piper", LastName = "Watson", JerseyNumber = "1" },
                        new Player { ID = 43, TeamID = 4, MemberID = "M0000045", FirstName = "Logan", LastName = "Hughes", JerseyNumber = "4" },
                        
                        //team5                        
                        new Player { ID = 44, TeamID = 5, MemberID = "M0000047", FirstName = "Sophie", LastName = "Clark", JerseyNumber = "22" },
                        new Player { ID = 45, TeamID = 5, MemberID = "M0000048", FirstName = "Nathan", LastName = "Brown", JerseyNumber = "5" },
                        new Player { ID = 46, TeamID = 5, MemberID = "M0000049", FirstName = "Grace", LastName = "Allen", JerseyNumber = "15" },
                        new Player { ID = 47, TeamID = 5, MemberID = "M0000050", FirstName = "Elijah", LastName = "Hernandez", JerseyNumber = "7" },
                        new Player { ID = 48, TeamID = 5, MemberID = "M0000051", FirstName = "Oliver", LastName = "Young", JerseyNumber = "33" },
                        new Player { ID = 49, TeamID = 5, MemberID = "M0000052", FirstName = "Ava", LastName = "Garcia", JerseyNumber = "19" },
                        new Player { ID = 50, TeamID = 5, MemberID = "M0000053", FirstName = "Samuel", LastName = "Flores", JerseyNumber = "8" },
                        new Player { ID = 51, TeamID = 5, MemberID = "M0000054", FirstName = "Liam", LastName = "Ramirez", JerseyNumber = "3" },
                        new Player { ID = 52, TeamID = 5, MemberID = "M0000055", FirstName = "Emma", LastName = "Sanchez", JerseyNumber = "25" },
                        new Player { ID = 53, TeamID = 5, MemberID = "M0000056", FirstName = "Caleb", LastName = "Perez", JerseyNumber = "12" },
                        new Player { ID = 54, TeamID = 5, MemberID = "M0000057", FirstName = "Sophia", LastName = "Torres", JerseyNumber = "14" },
                        
                        //team6
                        new Player { ID = 55, TeamID = 6, MemberID = "M0000058", FirstName = "Joshua", LastName = "Martinez", JerseyNumber = "9" },
                        new Player { ID = 56, TeamID = 6, MemberID = "M0000059", FirstName = "Lily", LastName = "Anderson", JerseyNumber = "18" },
                        new Player { ID = 57, TeamID = 6, MemberID = "M0000060", FirstName = "Daniel", LastName = "Thompson", JerseyNumber = "6" },
                        new Player { ID = 58, TeamID = 6, MemberID = "M0000061", FirstName = "Isabella", LastName = "Gonzalez", JerseyNumber = "20" },
                        new Player { ID = 59, TeamID = 6, MemberID = "M0000062", FirstName = "Jack", LastName = "Hernandez", JerseyNumber = "11" },
                        new Player { ID = 60, TeamID = 6, MemberID = "M0000063", FirstName = "Chloe", LastName = "Lopez", JerseyNumber = "23" },
                        new Player { ID = 61, TeamID = 6, MemberID = "M0000064", FirstName = "Ryan", LastName = "Young", JerseyNumber = "4" },
                        new Player { ID = 62, TeamID = 6, MemberID = "M0000065", FirstName = "Hannah", LastName = "Gomez", JerseyNumber = "30" },
                        new Player { ID = 63, TeamID = 6, MemberID = "M0000066", FirstName = "Oscar", LastName = "Hill", JerseyNumber = "17" },
                        new Player { ID = 64, TeamID = 6, MemberID = "M0000067", FirstName = "Aria", LastName = "Russell", JerseyNumber = "21" },
                        new Player { ID = 65, TeamID = 6, MemberID = "M0000068", FirstName = "Evan", LastName = "Price", JerseyNumber = "2" },
                        new Player { ID = 66, TeamID = 6, MemberID = "M0000069", FirstName = "Leah", LastName = "Phillips", JerseyNumber = "16" },
                        
                        //team7
                        new Player { ID = 67, TeamID = 7, MemberID = "M0000070", FirstName = "Christian", LastName = "Howard", JerseyNumber = "13" },
                        new Player { ID = 68, TeamID = 7, MemberID = "M0000071", FirstName = "Maya", LastName = "King", JerseyNumber = "29" },
                        new Player { ID = 69, TeamID = 7, MemberID = "M0000072", FirstName = "Nicholas", LastName = "Scott", JerseyNumber = "24" },
                        new Player { ID = 70, TeamID = 7, MemberID = "M0000073", FirstName = "Bella", LastName = "Adams", JerseyNumber = "31" },
                        new Player { ID = 71, TeamID = 7, MemberID = "M0000074", FirstName = "Eli", LastName = "Rivera", JerseyNumber = "1" },
                        new Player { ID = 72, TeamID = 7, MemberID = "M0000075", FirstName = "Mila", LastName = "Gutierrez", JerseyNumber = "26" },
                        new Player { ID = 73, TeamID = 7, MemberID = "M0000076", FirstName = "James", LastName = "Parker", JerseyNumber = "22" },
                        new Player { ID = 74, TeamID = 7, MemberID = "M0000077", FirstName = "Aurora", LastName = "Morales", JerseyNumber = "8" },
                        new Player { ID = 75, TeamID = 7, MemberID = "M0000078", FirstName = "David", LastName = "Nelson", JerseyNumber = "14" },
                        new Player { ID = 76, TeamID = 7, MemberID = "M0000079", FirstName = "Luna", LastName = "Carter", JerseyNumber = "19" },
                        
                        //team8
                        new Player { ID = 77, TeamID = 8, MemberID = "M0000080", FirstName = "Lucas", LastName = "Perez", JerseyNumber = "10" },
                        new Player { ID = 78, TeamID = 8, MemberID = "M0000081", FirstName = "Sofia", LastName = "Evans", JerseyNumber = "28" },
                        new Player { ID = 79, TeamID = 8, MemberID = "M0000082", FirstName = "Adrian", LastName = "Garcia", JerseyNumber = "6" },
                        new Player { ID = 80, TeamID = 8, MemberID = "M0000083", FirstName = "Ella", LastName = "Cook", JerseyNumber = "23" },
                        new Player { ID = 81, TeamID = 8, MemberID = "M0000084", FirstName = "Mateo", LastName = "Martinez", JerseyNumber = "9" },
                        new Player { ID = 82, TeamID = 8, MemberID = "M0000085", FirstName = "Luna", LastName = "Bailey", JerseyNumber = "20" },
                        new Player { ID = 83, TeamID = 8, MemberID = "M0000086", FirstName = "Levi", LastName = "Ward", JerseyNumber = "11" },
                        new Player { ID = 84, TeamID = 8, MemberID = "M0000087", FirstName = "Aubrey", LastName = "Cooper", JerseyNumber = "26" },
                        new Player { ID = 85, TeamID = 8, MemberID = "M0000088", FirstName = "Aaron", LastName = "Morris", JerseyNumber = "2" },
                        new Player { ID = 86, TeamID = 8, MemberID = "M0000089", FirstName = "Bella", LastName = "Peterson", JerseyNumber = "1" },
                        new Player { ID = 87, TeamID = 8, MemberID = "M0000090", FirstName = "Eli", LastName = "Gomez", JerseyNumber = "4" },
                        
                        //team9
                        new Player { ID = 88, TeamID = 9, MemberID = "M0000091", FirstName = "Alexander", LastName = "James", JerseyNumber = "10" },
                        new Player { ID = 89, TeamID = 9, MemberID = "M0000092", FirstName = "Natalie", LastName = "Hill", JerseyNumber = "22" },
                        new Player { ID = 90, TeamID = 9, MemberID = "M0000093", FirstName = "Christopher", LastName = "Coleman", JerseyNumber = "5" },
                        new Player { ID = 91, TeamID = 9, MemberID = "M0000094", FirstName = "Madison", LastName = "Lee", JerseyNumber = "15" },
                        new Player { ID = 92, TeamID = 9, MemberID = "M0000095", FirstName = "Evan", LastName = "Russell", JerseyNumber = "7" },
                        new Player { ID = 93, TeamID = 9, MemberID = "M0000096", FirstName = "Nora", LastName = "Allen", JerseyNumber = "33" },
                        new Player { ID = 94, TeamID = 9, MemberID = "M0000097", FirstName = "Carter", LastName = "Long", JerseyNumber = "19" },
                        new Player { ID = 95, TeamID = 9, MemberID = "M0000098", FirstName = "Victoria", LastName = "Wood", JerseyNumber = "8" },
                        new Player { ID = 96, TeamID = 9, MemberID = "M0000099", FirstName = "Dominic", LastName = "Baker", JerseyNumber = "3" },
                        new Player { ID = 97, TeamID = 9, MemberID = "M0000100", FirstName = "Eleanor", LastName = "Evans", JerseyNumber = "25" },
                        new Player { ID = 98, TeamID = 9, MemberID = "M0000101", FirstName = "Julian", LastName = "Gonzalez", JerseyNumber = "12" },
                        new Player { ID = 99, TeamID = 9, MemberID = "M0000102", FirstName = "Evelyn", LastName = "Stewart", JerseyNumber = "14" },
                        
                        //team10
                        new Player { ID = 100, TeamID = 10, MemberID = "M0000103", FirstName = "William", LastName = "Jones", JerseyNumber = "9" },
                        new Player { ID = 101, TeamID = 10, MemberID = "M0000104", FirstName = "Avery", LastName = "Brown", JerseyNumber = "18" },
                        new Player { ID = 102, TeamID = 10, MemberID = "M0000105", FirstName = "Aiden", LastName = "Williams", JerseyNumber = "6" },
                        new Player { ID = 103, TeamID = 10, MemberID = "M0000106", FirstName = "Madeline", LastName = "Taylor", JerseyNumber = "20" },
                        new Player { ID = 104, TeamID = 10, MemberID = "M0000107", FirstName = "Jackson", LastName = "Moore", JerseyNumber = "11" },
                        new Player { ID = 105, TeamID = 10, MemberID = "M0000108", FirstName = "Scarlett", LastName = "Clark", JerseyNumber = "23" },
                        new Player { ID = 106, TeamID = 10, MemberID = "M0000109", FirstName = "Henry", LastName = "Lewis", JerseyNumber = "4" },
                        new Player { ID = 107, TeamID = 10, MemberID = "M0000110", FirstName = "Emma", LastName = "Turner", JerseyNumber = "30" },
                        new Player { ID = 108, TeamID = 10, MemberID = "M0000111", FirstName = "Landon", LastName = "Baker", JerseyNumber = "17" },
                        new Player { ID = 109, TeamID = 10, MemberID = "M0000112", FirstName = "Samantha", LastName = "Cooper", JerseyNumber = "21" },
                        new Player { ID = 110, TeamID = 10, MemberID = "M0000113", FirstName = "Elijah", LastName = "Perry", JerseyNumber = "2" },
                        new Player { ID = 111, TeamID = 10, MemberID = "M0000114", FirstName = "Ava", LastName = "Fisher", JerseyNumber = "16" },

                        //team11
                        new Player { ID = 112, TeamID = 11, MemberID = "M0000115", FirstName = "Noah", LastName = "Lopez", JerseyNumber = "13" },
                        new Player { ID = 113, TeamID = 11, MemberID = "M0000116", FirstName = "Aria", LastName = "Hill", JerseyNumber = "29" },
                        new Player { ID = 114, TeamID = 11, MemberID = "M0000117", FirstName = "Ethan", LastName = "Wright", JerseyNumber = "24" },
                        new Player { ID = 115, TeamID = 11, MemberID = "M0000118", FirstName = "Grace", LastName = "Green", JerseyNumber = "31" },
                        new Player { ID = 116, TeamID = 11, MemberID = "M0000119", FirstName = "Benjamin", LastName = "Scott", JerseyNumber = "1" },
                        new Player { ID = 117, TeamID = 11, MemberID = "M0000120", FirstName = "Aurora", LastName = "Adams", JerseyNumber = "26" },
                        new Player { ID = 118, TeamID = 11, MemberID = "M0000121", FirstName = "Lucas", LastName = "Parker", JerseyNumber = "22" },
                        new Player { ID = 119, TeamID = 11, MemberID = "M0000122", FirstName = "Ella", LastName = "Morales", JerseyNumber = "8" },
                        new Player { ID = 120, TeamID = 11, MemberID = "M0000123", FirstName = "Emma", LastName = "Nelson", JerseyNumber = "14" },
                        new Player { ID = 121, TeamID = 11, MemberID = "M0000124", FirstName = "Levi", LastName = "Carter", JerseyNumber = "19" },

                        //team12
                        new Player { ID = 122, TeamID = 12, MemberID = "M0000125", FirstName = "Jacob", LastName = "Cook", JerseyNumber = "10" },
                        new Player { ID = 123, TeamID = 12, MemberID = "M0000126", FirstName = "Avery", LastName = "Bailey", JerseyNumber = "28" },
                        new Player { ID = 124, TeamID = 12, MemberID = "M0000127", FirstName = "Michael", LastName = "Ward", JerseyNumber = "6" },
                        new Player { ID = 125, TeamID = 12, MemberID = "M0000128", FirstName = "Amelia", LastName = "Cooper", JerseyNumber = "23" },
                        new Player { ID = 126, TeamID = 12, MemberID = "M0000129", FirstName = "Daniel", LastName = "Morris", JerseyNumber = "9" },
                        new Player { ID = 127, TeamID = 12, MemberID = "M0000130", FirstName = "Lily", LastName = "Wood", JerseyNumber = "20" },
                        new Player { ID = 128, TeamID = 12, MemberID = "M0000131", FirstName = "Mason", LastName = "Foster", JerseyNumber = "11" },
                        new Player { ID = 129, TeamID = 12, MemberID = "M0000132", FirstName = "Mia", LastName = "Gonzalez", JerseyNumber = "26" },
                        new Player { ID = 130, TeamID = 12, MemberID = "M0000133", FirstName = "Logan", LastName = "Miller", JerseyNumber = "2" },
                        new Player { ID = 131, TeamID = 12, MemberID = "M0000134", FirstName = "Harper", LastName = "Perez", JerseyNumber = "1" },

                        //team13
                        new Player { ID = 132, TeamID = 13, MemberID = "M0000135", FirstName = "Ethan", LastName = "Johnson", JerseyNumber = "10" },
                        new Player { ID = 133, TeamID = 13, MemberID = "M0000136", FirstName = "Sophia", LastName = "Martinez", JerseyNumber = "22" },
                        new Player { ID = 134, TeamID = 13, MemberID = "M0000137", FirstName = "Jackson", LastName = "Lopez", JerseyNumber = "5" },
                        new Player { ID = 135, TeamID = 13, MemberID = "M0000138", FirstName = "Ava", LastName = "Hernandez", JerseyNumber = "15" },
                        new Player { ID = 136, TeamID = 13, MemberID = "M0000139", FirstName = "William", LastName = "Garcia", JerseyNumber = "7" },
                        new Player { ID = 137, TeamID = 13, MemberID = "M0000140", FirstName = "Madison", LastName = "Young", JerseyNumber = "33" },
                        new Player { ID = 138, TeamID = 13, MemberID = "M0000141", FirstName = "Liam", LastName = "Taylor", JerseyNumber = "19" },
                        new Player { ID = 139, TeamID = 13, MemberID = "M0000142", FirstName = "Harper", LastName = "Lewis", JerseyNumber = "8" },
                        new Player { ID = 140, TeamID = 13, MemberID = "M0000143", FirstName = "Olivia", LastName = "Clark", JerseyNumber = "3" },
                        new Player { ID = 141, TeamID = 13, MemberID = "M0000144", FirstName = "Lucas", LastName = "Allen", JerseyNumber = "25" },

                        //team14
                        new Player { ID = 142, TeamID = 14, MemberID = "M0000145", FirstName = "Aiden", LastName = "Walker", JerseyNumber = "9" },
                        new Player { ID = 143, TeamID = 14, MemberID = "M0000146", FirstName = "Scarlett", LastName = "Hall", JerseyNumber = "18" },
                        new Player { ID = 144, TeamID = 14, MemberID = "M0000147", FirstName = "Gabriel", LastName = "Wright", JerseyNumber = "6" },
                        new Player { ID = 145, TeamID = 14, MemberID = "M0000148", FirstName = "Hannah", LastName = "Thompson", JerseyNumber = "20" },
                        new Player { ID = 146, TeamID = 14, MemberID = "M0000149", FirstName = "David", LastName = "Gomez", JerseyNumber = "11" },
                        new Player { ID = 147, TeamID = 14, MemberID = "M0000150", FirstName = "Layla", LastName = "Roberts", JerseyNumber = "23" },
                        new Player { ID = 148, TeamID = 14, MemberID = "M0000151", FirstName = "Carter", LastName = "Flores", JerseyNumber = "4" },
                        new Player { ID = 149, TeamID = 14, MemberID = "M0000152", FirstName = "Chloe", LastName = "Ramirez", JerseyNumber = "30" },
                        new Player { ID = 150, TeamID = 14, MemberID = "M0000153", FirstName = "Luke", LastName = "Sanchez", JerseyNumber = "17" },
                        new Player { ID = 151, TeamID = 14, MemberID = "M0000154", FirstName = "Evelyn", LastName = "King", JerseyNumber = "21" },
                        new Player { ID = 152, TeamID = 14, MemberID = "M0000155", FirstName = "Owen", LastName = "Ward", JerseyNumber = "2" },
                        new Player { ID = 153, TeamID = 14, MemberID = "M0000156", FirstName = "Zoe", LastName = "Rivera", JerseyNumber = "16" },

                        //team15
                        new Player { ID = 154, TeamID = 15, MemberID = "M0000157", FirstName = "Matthew", LastName = "Martinez", JerseyNumber = "10" },
                        new Player { ID = 155, TeamID = 15, MemberID = "M0000158", FirstName = "Eleanor", LastName = "Adams", JerseyNumber = "22" },
                        new Player { ID = 156, TeamID = 15, MemberID = "M0000159", FirstName = "Gabriel", LastName = "Wilson", JerseyNumber = "5" },
                        new Player { ID = 157, TeamID = 15, MemberID = "M0000160", FirstName = "Ariana", LastName = "Perez", JerseyNumber = "15" },
                        new Player { ID = 158, TeamID = 15, MemberID = "M0000161", FirstName = "Daniel", LastName = "Hernandez", JerseyNumber = "7" },
                        new Player { ID = 159, TeamID = 15, MemberID = "M0000162", FirstName = "Sophia", LastName = "Garcia", JerseyNumber = "33" },
                        new Player { ID = 160, TeamID = 15, MemberID = "M0000163", FirstName = "Jacob", LastName = "Lee", JerseyNumber = "19" },
                        new Player { ID = 161, TeamID = 15, MemberID = "M0000164", FirstName = "Emily", LastName = "Johnson", JerseyNumber = "8" },
                        new Player { ID = 162, TeamID = 15, MemberID = "M0000165", FirstName = "Logan", LastName = "Brown", JerseyNumber = "3" },
                        new Player { ID = 163, TeamID = 15, MemberID = "M0000166", FirstName = "Isabella", LastName = "Thompson", JerseyNumber = "25" },

                        //team16
                        new Player { ID = 164, TeamID = 16, MemberID = "M0000167", FirstName = "Elijah", LastName = "Taylor", JerseyNumber = "9" },
                        new Player { ID = 165, TeamID = 16, MemberID = "M0000168", FirstName = "Ava", LastName = "Harris", JerseyNumber = "18" },
                        new Player { ID = 166, TeamID = 16, MemberID = "M0000169", FirstName = "Mason", LastName = "Lewis", JerseyNumber = "6" },
                        new Player { ID = 167, TeamID = 16, MemberID = "M0000170", FirstName = "Madeline", LastName = "Adams", JerseyNumber = "20" },
                        new Player { ID = 168, TeamID = 16, MemberID = "M0000171", FirstName = "Oliver", LastName = "Gonzalez", JerseyNumber = "11" },
                        new Player { ID = 169, TeamID = 16, MemberID = "M0000172", FirstName = "Emma", LastName = "Wilson", JerseyNumber = "23" },
                        new Player { ID = 170, TeamID = 16, MemberID = "M0000173", FirstName = "Liam", LastName = "Diaztre", JerseyNumber = "4" },                        
                        new Player { ID = 171, TeamID = 16, MemberID = "M0000175", FirstName = "Logan", LastName = "Lopez", JerseyNumber = "17" },
                        new Player { ID = 172, TeamID = 16, MemberID = "M0000176", FirstName = "Sophia", LastName = "Evans", JerseyNumber = "21" },
                        new Player { ID = 173, TeamID = 16, MemberID = "M0000177", FirstName = "Jackson", LastName = "GomezPerez", JerseyNumber = "2" },                        

                        //team17
                        new Player { ID = 174, TeamID = 17, MemberID = "M0000179", FirstName = "James", LastName = "Hell", JerseyNumber = "10" },
                        new Player { ID = 175, TeamID = 17, MemberID = "M0000180", FirstName = "Isabella", LastName = "Rivera", JerseyNumber = "22" },                        
                        new Player { ID = 176, TeamID = 17, MemberID = "M0000183", FirstName = "William", LastName = "Mcbain", JerseyNumber = "7" },
                        new Player { ID = 177, TeamID = 17, MemberID = "M0000184", FirstName = "Charlotte", LastName = "Young", JerseyNumber = "33" },
                        new Player { ID = 178, TeamID = 17, MemberID = "M0000185", FirstName = "Ethan", LastName = "Taylor", JerseyNumber = "19" },
                        new Player { ID = 179, TeamID = 17, MemberID = "M0000186", FirstName = "Harper", LastName = "Maquia", JerseyNumber = "8" },
                        new Player { ID = 180, TeamID = 17, MemberID = "M0000187", FirstName = "Amelia", LastName = "Lopez", JerseyNumber = "3" },
                        new Player { ID = 181, TeamID = 17, MemberID = "M0000188", FirstName = "Mason", LastName = "Leeandro", JerseyNumber = "25" },

                        //team18
                        new Player { ID = 182, TeamID = 18, MemberID = "M0000189", FirstName = "Olivia", LastName = "Anderson", JerseyNumber = "9" },
                        new Player { ID = 183, TeamID = 18, MemberID = "M0000190", FirstName = "Lucas", LastName = "Garcia", JerseyNumber = "18" },                        
                        new Player { ID = 184, TeamID = 18, MemberID = "M0000192", FirstName = "Ella", LastName = "Perez", JerseyNumber = "20" },
                        new Player { ID = 185, TeamID = 18, MemberID = "M0000193", FirstName = "Jackson", LastName = "Ward", JerseyNumber = "11" },
                        new Player { ID = 186, TeamID = 18, MemberID = "M0000194", FirstName = "Sophia", LastName = "Tudo", JerseyNumber = "23" },
                        new Player { ID = 187, TeamID = 18, MemberID = "M0000195", FirstName = "Daniel", LastName = "Roberts", JerseyNumber = "4" },
                        new Player { ID = 188, TeamID = 18, MemberID = "M0000196", FirstName = "Ava", LastName = "Smith", JerseyNumber = "30" },
                        new Player { ID = 189, TeamID = 18, MemberID = "M0000197", FirstName = "William", LastName = "Buzo", JerseyNumber = "17" },
                        new Player { ID = 190, TeamID = 18, MemberID = "M0000198", FirstName = "Evelyn", LastName = "Hard", JerseyNumber = "21" },
                        new Player { ID = 191, TeamID = 18, MemberID = "M0000199", FirstName = "Logan", LastName = "Queen", JerseyNumber = "2" },
                        new Player { ID = 192, TeamID = 18, MemberID = "M0000200", FirstName = "Mia", LastName = "Johnson", JerseyNumber = "16" },

                        //team19
                        new Player { ID = 193, TeamID = 19, MemberID = "M0000201", FirstName = "Ethan", LastName = "Hernandez", JerseyNumber = "10" },
                        new Player { ID = 194, TeamID = 19, MemberID = "M0000202", FirstName = "Emma", LastName = "Smith", JerseyNumber = "22" },
                        new Player { ID = 195, TeamID = 19, MemberID = "M0000203", FirstName = "William", LastName = "Martinez", JerseyNumber = "5" },
                        new Player { ID = 196, TeamID = 19, MemberID = "M0000204", FirstName = "Olivia", LastName = "Gonzalez", JerseyNumber = "15" },                        
                        new Player { ID = 197, TeamID = 19, MemberID = "M0000206", FirstName = "Ava", LastName = "Lewis", JerseyNumber = "33" },
                        new Player { ID = 198, TeamID = 19, MemberID = "M0000207", FirstName = "Jackson", LastName = "Florez", JerseyNumber = "19" },
                        new Player { ID = 199, TeamID = 19, MemberID = "M0000208", FirstName = "Sophia", LastName = "Swiff", JerseyNumber = "8" },
                        new Player { ID = 200, TeamID = 19, MemberID = "M0000209", FirstName = "Liam", LastName = "White", JerseyNumber = "3" },
                        

                        //team20
                        new Player { ID = 201, TeamID = 20, MemberID = "M0000211", FirstName = "Jacob", LastName = "Harristood", JerseyNumber = "9" },                        
                        new Player { ID = 202, TeamID = 20, MemberID = "M0000213", FirstName = "Michael", LastName = "Jordan", JerseyNumber = "6" },
                        new Player { ID = 203, TeamID = 20, MemberID = "M0000214", FirstName = "Sophia", LastName = "Zerer", JerseyNumber = "20" },
                        new Player { ID = 204, TeamID = 20, MemberID = "M0000215", FirstName = "William", LastName = "Shakespaeare", JerseyNumber = "11" },
                        new Player { ID = 205, TeamID = 20, MemberID = "M0000216", FirstName = "Olivia", LastName = "Wilson", JerseyNumber = "23" },
                        new Player { ID = 206, TeamID = 20, MemberID = "M0000217", FirstName = "Liam", LastName = "Toribio", JerseyNumber = "4" },                        
                        new Player { ID = 207, TeamID = 20, MemberID = "M0000219", FirstName = "Logan", LastName = "Chavez", JerseyNumber = "17" },
                        new Player { ID = 208, TeamID = 20, MemberID = "M0000220", FirstName = "Sophia", LastName = "Sans", JerseyNumber = "21" },
                        new Player { ID = 209, TeamID = 20, MemberID = "M0000221", FirstName = "Jackson", LastName = "Gomez", JerseyNumber = "2" },                        

                        //team21
                        new Player { ID = 210, TeamID = 21, MemberID = "M0000223", FirstName = "James", LastName = "Holly", JerseyNumber = "10" },
                        new Player { ID = 211, TeamID = 21, MemberID = "M0000224", FirstName = "Isabella", LastName = "Qwert", JerseyNumber = "22" },
                        new Player { ID = 212, TeamID = 21, MemberID = "M0000225", FirstName = "Benjamin", LastName = "Wright", JerseyNumber = "5" },                        
                        new Player { ID = 213, TeamID = 21, MemberID = "M0000227", FirstName = "William", LastName = "Cabezedo", JerseyNumber = "7" },                        
                        new Player { ID = 214, TeamID = 21, MemberID = "M0000229", FirstName = "Ethan", LastName = "Gonzalez", JerseyNumber = "19" },
                        new Player { ID = 215, TeamID = 21, MemberID = "M0000230", FirstName = "Harper", LastName = "White", JerseyNumber = "8" },                        
                        new Player { ID = 216, TeamID = 21, MemberID = "M0000232", FirstName = "Mason", LastName = "FLee", JerseyNumber = "25" },

                        //team22
                        new Player { ID = 217, TeamID = 22, MemberID = "M0000233", FirstName = "Olivia", LastName = "Cuaji", JerseyNumber = "9" },
                        new Player { ID = 218, TeamID = 22, MemberID = "M0000234", FirstName = "Lucas", LastName = "Bon", JerseyNumber = "18" },
                        new Player { ID = 219, TeamID = 22, MemberID = "M0000235", FirstName = "Avery", LastName = "Scott", JerseyNumber = "6" },
                        new Player { ID = 220, TeamID = 22, MemberID = "M0000236", FirstName = "Ella", LastName = "Perez", JerseyNumber = "20" },
                        new Player { ID = 221, TeamID = 22, MemberID = "M0000237", FirstName = "Jackson", LastName = "Tard", JerseyNumber = "11" },
                        new Player { ID = 222, TeamID = 22, MemberID = "M0000238", FirstName = "Sophia", LastName = "Ramirez", JerseyNumber = "23" },                        
                        new Player { ID = 223, TeamID = 22, MemberID = "M0000241", FirstName = "William", LastName = "Cab", JerseyNumber = "17" },
                        new Player { ID = 224, TeamID = 22, MemberID = "M0000242", FirstName = "Evelyn", LastName = "Hernandez", JerseyNumber = "21" },
                        new Player { ID = 225, TeamID = 22, MemberID = "M0000243", FirstName = "Logan", LastName = "Tang", JerseyNumber = "2" },
                        new Player { ID = 226, TeamID = 22, MemberID = "M0000244", FirstName = "Mia", LastName = "Tuya", JerseyNumber = "16" },

                        //team23
                        new Player { ID = 227, TeamID = 23, MemberID = "M0000245", FirstName = "Ethan", LastName = "Adams", JerseyNumber = "10" },
                        new Player { ID = 228, TeamID = 23, MemberID = "M0000246", FirstName = "Emma", LastName = "Gonzalez", JerseyNumber = "22" },
                        new Player { ID = 229, TeamID = 23, MemberID = "M0000247", FirstName = "William", LastName = "Smith", JerseyNumber = "5" },
                        new Player { ID = 230, TeamID = 23, MemberID = "M0000248", FirstName = "Olivia", LastName = "Hernandez", JerseyNumber = "15" },
                        new Player { ID = 231, TeamID = 23, MemberID = "M0000249", FirstName = "Daniel", LastName = "Wright", JerseyNumber = "7" },                        
                        new Player { ID = 232, TeamID = 23, MemberID = "M0000251", FirstName = "Jackson", LastName = "Garcia", JerseyNumber = "19" },
                        new Player { ID = 233, TeamID = 23, MemberID = "M0000252", FirstName = "Sophia", LastName = "Taylor", JerseyNumber = "8" },
                        new Player { ID = 234, TeamID = 23, MemberID = "M0000253", FirstName = "Liam", LastName = "Black", JerseyNumber = "3" },
                        new Player { ID = 235, TeamID = 23, MemberID = "M0000254", FirstName = "Charlotte", LastName = "Lee", JerseyNumber = "25" },

                        //team24
                        new Player { ID = 236, TeamID = 24, MemberID = "M0000255", FirstName = "Jacob", LastName = "Harris", JerseyNumber = "9" },
                        new Player { ID = 237, TeamID = 24, MemberID = "M0000256", FirstName = "Ava", LastName = "Clark", JerseyNumber = "18" },
                        new Player { ID = 238, TeamID = 24, MemberID = "M0000257", FirstName = "Michael", LastName = "Lopez", JerseyNumber = "6" },
                        new Player { ID = 239, TeamID = 24, MemberID = "M0000258", FirstName = "Sophia", LastName = "Lopezcaste", JerseyNumber = "20" },
                        new Player { ID = 240, TeamID = 24, MemberID = "M0000259", FirstName = "William", LastName = "Gonzalez", JerseyNumber = "11" },
                        new Player { ID = 241, TeamID = 24, MemberID = "M0000260", FirstName = "Olivia", LastName = "Thomas", JerseyNumber = "23" },
                        new Player { ID = 242, TeamID = 24, MemberID = "M0000261", FirstName = "Liam", LastName = "Diaz", JerseyNumber = "4" },
                        new Player { ID = 243, TeamID = 24, MemberID = "M0000262", FirstName = "Abigail", LastName = "Young", JerseyNumber = "30" },
                        new Player { ID = 244, TeamID = 24, MemberID = "M0000263", FirstName = "Logan", LastName = "Rodriguez", JerseyNumber = "17" },
                        new Player { ID = 245, TeamID = 24, MemberID = "M0000264", FirstName = "Sophia", LastName = "Vans", JerseyNumber = "21" },
                        new Player { ID = 246, TeamID = 24, MemberID = "M0000265", FirstName = "Jackson", LastName = "Hand", JerseyNumber = "2" },
                        new Player { ID = 247, TeamID = 24, MemberID = "M0000266", FirstName = "Aria", LastName = "Torres", JerseyNumber = "16" },

                        //team25
                        new Player { ID = 248, TeamID = 25, MemberID = "M0000267", FirstName = "James", LastName = "Hallow", JerseyNumber = "10" },
                        new Player { ID = 249, TeamID = 25, MemberID = "M0000268", FirstName = "Isabella", LastName = "Poutre", JerseyNumber = "22" },                        
                        new Player { ID = 250, TeamID = 25, MemberID = "M0000270", FirstName = "Chloe", LastName = "Gomez", JerseyNumber = "15" },
                        new Player { ID = 251, TeamID = 25, MemberID = "M0000271", FirstName = "William", LastName = "Choleing", JerseyNumber = "7" },                        
                        new Player { ID = 252, TeamID = 25, MemberID = "M0000273", FirstName = "Ethan", LastName = "Sosa", JerseyNumber = "19" },
                        new Player { ID = 253, TeamID = 25, MemberID = "M0000274", FirstName = "Harper", LastName = "Thomson", JerseyNumber = "8" },                        
                        new Player { ID = 254, TeamID = 25, MemberID = "M0000276", FirstName = "Mason", LastName = "Lee", JerseyNumber = "25" },

                        //team26
                        new Player { ID = 255, TeamID = 26, MemberID = "M0000277", FirstName = "Olivia", LastName = "Videgaray", JerseyNumber = "9" },
                        new Player { ID = 256, TeamID = 26, MemberID = "M0000278", FirstName = "Lucas", LastName = "Smith", JerseyNumber = "18" },
                        new Player { ID = 257, TeamID = 26, MemberID = "M0000279", FirstName = "Avery", LastName = "Scott", JerseyNumber = "6" },
                        new Player { ID = 258, TeamID = 26, MemberID = "M0000280", FirstName = "Ella", LastName = "Perez", JerseyNumber = "20" },
                        new Player { ID = 259, TeamID = 26, MemberID = "M0000281", FirstName = "Jackson", LastName = "Aard", JerseyNumber = "11" },
                        new Player { ID = 260, TeamID = 26, MemberID = "M0000282", FirstName = "Sophia", LastName = "Ramirez", JerseyNumber = "23" },                        
                        new Player { ID = 261, TeamID = 26, MemberID = "M0000285", FirstName = "William", LastName = "Gosa", JerseyNumber = "17" },
                        new Player { ID = 262, TeamID = 26, MemberID = "M0000286", FirstName = "Evelyn", LastName = "Mcbain", JerseyNumber = "21" },
                        new Player { ID = 263, TeamID = 26, MemberID = "M0000287", FirstName = "Logan", LastName = "King", JerseyNumber = "2" },
                        new Player { ID = 264, TeamID = 26, MemberID = "M0000288", FirstName = "Mia", LastName = "Tenla", JerseyNumber = "16" }
                    };

                    context.Players.AddRange(players);
                    context.SaveChanges();

                }

                /// GameLineUps
                if (!context.GameLineUps.Any())
                {
                    var gameLineUps = new List<GameLineUp>();
                    int id = 1;
                    int gameID = 1;
                    int maxPlayers = 6; // Maximum number of players per team
                    int c=1;

                    // Map each TeamID to its PlayerID range
                    var teamIdToPlayerIdRange = new Dictionary<int, (int Min, int Max)>
                    {
                        {1, (1, 12)},
                        {2, (13, 24)},
                        {3, (25, 33)},
                        {4, (34, 43)},
                        {5, (44, 54)},
                        {6, (55, 66)},
                        {7, (67, 76)},
                        {8, (77, 87)},
                        {9, (88, 99)},
                        {10, (100, 111)},
                        {11, (112, 121)},
                        {12, (122, 131)},
                        {13, (132, 141)},
                        {14, (142, 153)},
                        {15, (154, 163)},
                        {16, (164, 173)},
                        {17, (174, 181)},
                        {18, (182, 192)},
                        {19, (193, 200)},
                        {20, (201, 209)},
                        {21, (210, 216)},
                        {22, (217, 226)},
                        {23, (227, 235)},
                        {24, (236, 247)},
                        {25, (248, 254)},
                        {26, (255, 264)}
                    };

                    while (c <= 9)//Almost 120 Games has been Assignemnt
                    {
                        for (int teamID = 1; teamID <= 26; teamID++)
                        {
                            int playerCount = new Random().Next(1, maxPlayers + 1); // Random number of players
                            int playerID = teamIdToPlayerIdRange[teamID].Min; // Start from the minimum PlayerID for this team

                            for (int i = 1; i <= playerCount; i++)
                            {
                                gameLineUps.Add(new GameLineUp { ID = id++, BattingOrder = i, GameID = gameID, PlayerID = playerID++, TeamID = teamID });
                            }

                            if (teamID % 2 == 0) // Increment gameID every two teams
                                gameID++;
                        }
                        c++;
                    }
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



                //// ScorePlayers
                //if (!context.ScorePlayers.Any())
                //{
                //    var scorePlayers = new List<ScorePlayer>
                //    {
                //        new ScorePlayer
                //        {
                //            H = 3,
                //            RBI = 2,
                //            Singles = 2,
                //            Doubles = 1,
                //            Triples = 0,
                //            HR = 1,
                //            BB = 1,
                //            PA = 4,
                //            AB = 3,
                //            Run = 1,
                //            HBP = 0,
                //            StrikeOut = 0,
                //            Out = 0,
                //            Fouls =0,
                //            Balls = 0,
                //            GameLineUpID=1
                //        },
                //        new ScorePlayer
                //        {
                //             H = 3,
                //            RBI = 2,
                //            Singles = 2,
                //            Doubles = 1,
                //            Triples = 0,
                //            HR = 1,
                //            BB = 1,
                //            PA = 4,
                //            AB = 3,
                //            Run = 1,
                //            HBP = 0,
                //            StrikeOut = 0,
                //            Out = 0,
                //            Fouls =0,
                //            Balls = 0,
                //            GameLineUpID=1
                //        },
                //    };

                //    context.ScorePlayers.AddRange(scorePlayers);
                //    context.SaveChanges();
                //}
            
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.GetBaseException().Message);
            }
        }
    }

}