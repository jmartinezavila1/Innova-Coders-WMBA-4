using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WMBA_4.Data.WMBAMigrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CityName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "GameTypes",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Description = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameTypes", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PositionCode = table.Column<string>(type: "TEXT", nullable: true),
                    PositionName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Seasons",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SeasonCode = table.Column<string>(type: "TEXT", nullable: true),
                    SeasonName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seasons", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Staff",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FirstName = table.Column<string>(type: "TEXT", nullable: true),
                    LastName = table.Column<string>(type: "TEXT", nullable: true),
                    email = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Staff", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Leagues",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LeagueName = table.Column<string>(type: "TEXT", nullable: true),
                    EstablishYear = table.Column<int>(type: "INTEGER", nullable: false),
                    CityID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Leagues", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Leagues_Cities_CityID",
                        column: x => x.CityID,
                        principalTable: "Cities",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LocationName = table.Column<string>(type: "TEXT", nullable: true),
                    CityID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Locations_Cities_CityID",
                        column: x => x.CityID,
                        principalTable: "Cities",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Divisions",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    DivisionName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LeagueID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Divisions", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Divisions_Leagues_LeagueID",
                        column: x => x.LeagueID,
                        principalTable: "Leagues",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Date = table.Column<DateTime>(type: "TEXT", nullable: false),
                    score = table.Column<int>(type: "INTEGER", nullable: false),
                    LocationID = table.Column<int>(type: "INTEGER", nullable: false),
                    SeasonID = table.Column<int>(type: "INTEGER", nullable: false),
                    GameTypeID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Games_GameTypes_GameTypeID",
                        column: x => x.GameTypeID,
                        principalTable: "GameTypes",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Games_Locations_LocationID",
                        column: x => x.LocationID,
                        principalTable: "Locations",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Games_Seasons_SeasonID",
                        column: x => x.SeasonID,
                        principalTable: "Seasons",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Coach_Name = table.Column<string>(type: "TEXT", nullable: true),
                    DivisionID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Teams_Divisions_DivisionID",
                        column: x => x.DivisionID,
                        principalTable: "Divisions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MemberID = table.Column<string>(type: "TEXT", maxLength: 8, nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    JerseyNumber = table.Column<string>(type: "TEXT", nullable: true),
                    TeamID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Players_Teams_TeamID",
                        column: x => x.TeamID,
                        principalTable: "Teams",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeamGame",
                columns: table => new
                {
                    TeamID = table.Column<int>(type: "INTEGER", nullable: false),
                    GameID = table.Column<int>(type: "INTEGER", nullable: false),
                    IsHomeTeam = table.Column<bool>(type: "INTEGER", nullable: false),
                    IsVisitorTeam = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamGame", x => new { x.TeamID, x.GameID });
                    table.ForeignKey(
                        name: "FK_TeamGame_Games_GameID",
                        column: x => x.GameID,
                        principalTable: "Games",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeamGame_Teams_TeamID",
                        column: x => x.TeamID,
                        principalTable: "Teams",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeamStaff",
                columns: table => new
                {
                    TeamID = table.Column<int>(type: "INTEGER", nullable: false),
                    StaffID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamStaff", x => new { x.TeamID, x.StaffID });
                    table.ForeignKey(
                        name: "FK_TeamStaff_Staff_StaffID",
                        column: x => x.StaffID,
                        principalTable: "Staff",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TeamStaff_Teams_TeamID",
                        column: x => x.TeamID,
                        principalTable: "Teams",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameLineUps",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    BattingOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    GameID = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayerID = table.Column<int>(type: "INTEGER", nullable: false),
                    TeamID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameLineUps", x => x.ID);
                    table.ForeignKey(
                        name: "FK_GameLineUps_Games_GameID",
                        column: x => x.GameID,
                        principalTable: "Games",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameLineUps_Players_PlayerID",
                        column: x => x.PlayerID,
                        principalTable: "Players",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameLineUps_Teams_TeamID",
                        column: x => x.TeamID,
                        principalTable: "Teams",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ScorePlayers",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    InningNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    H = table.Column<int>(type: "INTEGER", nullable: false),
                    RBI = table.Column<int>(type: "INTEGER", nullable: false),
                    R = table.Column<int>(type: "INTEGER", nullable: false),
                    StrikeOut = table.Column<int>(type: "INTEGER", nullable: false),
                    GroundOut = table.Column<int>(type: "INTEGER", nullable: false),
                    PopOut = table.Column<int>(type: "INTEGER", nullable: false),
                    Flyout = table.Column<int>(type: "INTEGER", nullable: false),
                    Singles = table.Column<int>(type: "INTEGER", nullable: false),
                    Doubles = table.Column<int>(type: "INTEGER", nullable: false),
                    Triples = table.Column<int>(type: "INTEGER", nullable: false),
                    HR = table.Column<int>(type: "INTEGER", nullable: false),
                    BB = table.Column<int>(type: "INTEGER", nullable: false),
                    HBP = table.Column<int>(type: "INTEGER", nullable: false),
                    SB = table.Column<int>(type: "INTEGER", nullable: false),
                    SAC = table.Column<int>(type: "INTEGER", nullable: false),
                    PA = table.Column<int>(type: "INTEGER", nullable: false),
                    AB = table.Column<int>(type: "INTEGER", nullable: false),
                    GameID = table.Column<int>(type: "INTEGER", nullable: false),
                    PlayerID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScorePlayers", x => x.ID);
                    table.ForeignKey(
                        name: "FK_ScorePlayers_Games_GameID",
                        column: x => x.GameID,
                        principalTable: "Games",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScorePlayers_Players_PlayerID",
                        column: x => x.PlayerID,
                        principalTable: "Players",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameLineUpPositions",
                columns: table => new
                {
                    GameLineUpID = table.Column<int>(type: "INTEGER", nullable: false),
                    PositionID = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameLineUpPositions", x => new { x.GameLineUpID, x.PositionID });
                    table.ForeignKey(
                        name: "FK_GameLineUpPositions_GameLineUps_GameLineUpID",
                        column: x => x.GameLineUpID,
                        principalTable: "GameLineUps",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GameLineUpPositions_Positions_PositionID",
                        column: x => x.PositionID,
                        principalTable: "Positions",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Divisions_DivisionName",
                table: "Divisions",
                column: "DivisionName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Divisions_LeagueID",
                table: "Divisions",
                column: "LeagueID");

            migrationBuilder.CreateIndex(
                name: "IX_GameLineUpPositions_PositionID",
                table: "GameLineUpPositions",
                column: "PositionID");

            migrationBuilder.CreateIndex(
                name: "IX_GameLineUps_GameID",
                table: "GameLineUps",
                column: "GameID");

            migrationBuilder.CreateIndex(
                name: "IX_GameLineUps_PlayerID",
                table: "GameLineUps",
                column: "PlayerID");

            migrationBuilder.CreateIndex(
                name: "IX_GameLineUps_TeamID",
                table: "GameLineUps",
                column: "TeamID");

            migrationBuilder.CreateIndex(
                name: "IX_Games_GameTypeID",
                table: "Games",
                column: "GameTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_Games_LocationID",
                table: "Games",
                column: "LocationID");

            migrationBuilder.CreateIndex(
                name: "IX_Games_SeasonID",
                table: "Games",
                column: "SeasonID");

            migrationBuilder.CreateIndex(
                name: "IX_Leagues_CityID",
                table: "Leagues",
                column: "CityID");

            migrationBuilder.CreateIndex(
                name: "IX_Locations_CityID",
                table: "Locations",
                column: "CityID");

            migrationBuilder.CreateIndex(
                name: "IX_Players_TeamID",
                table: "Players",
                column: "TeamID");

            migrationBuilder.CreateIndex(
                name: "IX_Positions_PositionName",
                table: "Positions",
                column: "PositionName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ScorePlayers_GameID",
                table: "ScorePlayers",
                column: "GameID");

            migrationBuilder.CreateIndex(
                name: "IX_ScorePlayers_PlayerID",
                table: "ScorePlayers",
                column: "PlayerID");

            migrationBuilder.CreateIndex(
                name: "IX_Seasons_SeasonCode",
                table: "Seasons",
                column: "SeasonCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeamGame_GameID",
                table: "TeamGame",
                column: "GameID");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_DivisionID",
                table: "Teams",
                column: "DivisionID");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_Name",
                table: "Teams",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_TeamStaff_StaffID",
                table: "TeamStaff",
                column: "StaffID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GameLineUpPositions");

            migrationBuilder.DropTable(
                name: "ScorePlayers");

            migrationBuilder.DropTable(
                name: "TeamGame");

            migrationBuilder.DropTable(
                name: "TeamStaff");

            migrationBuilder.DropTable(
                name: "GameLineUps");

            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DropTable(
                name: "Staff");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "GameTypes");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "Seasons");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "Divisions");

            migrationBuilder.DropTable(
                name: "Leagues");

            migrationBuilder.DropTable(
                name: "Cities");
        }
    }
}
