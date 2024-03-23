using Microsoft.EntityFrameworkCore.Migrations;

namespace WMBA_4.Data
{
    public static class ExtraMigration
    {
        public static void Steps(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"
		    Drop View IF EXISTS [PlayersEndSeason];
		    Create View PlayersEndSeason as
                SELECT
    			p.ID,
    			P.FirstName,
    			p.LastName,
    			P.MemberID,
    			p.JerseyNumber,
    			(SELECT SeasonCode FROM Seasons) AS Season,
    			d.DivisionName,
    			c.ClubName,
    			t.Name AS Team
			    FROM Players p
			    JOIN Teams t ON t.ID = p.TeamID
			    JOIN Divisions d ON d.ID = t.DivisionID
			    JOIN Clubs c ON c.ID= d.ClubID
            ");

            migrationBuilder.Sql(
                               @"
                Drop View IF EXISTS [GamesEndSeason];
		        Create View GamesEndSeason as
                SELECT
                g.ID,
                g.Date,
                l.LocationName,
                d.DivisionName,
                th.Name as TeamHome,
                tg.score as scoreHome,
                th2.Name as TeamVisitor,
                tg2.score as scoreVisitor,
                CASE 
                    WHEN g.Status = 0 THEN 'Inactive'
                    WHEN g.Status = 1 THEN 'Active'
                    ELSE 'Desconocido'
                END as Status_
            FROM
                Games g
            JOIN Locations l ON l.ID = g.LocationID
            JOIN TeamGame tg ON tg.GameID=g.ID and tg.IsHomeTeam=true
            JOIN TeamGame tg2 ON tg2.GameID=g.ID and tg2.IsVisitorTeam=true
            JOIN Teams th ON th.ID = tg.TeamID 
            JOIN Teams th2 ON th2.ID = tg2.TeamID
            JOIN Divisions d ON d.ID = th2.DivisionID
            ");

            migrationBuilder.Sql(
                               @"
                Drop View IF EXISTS [PlayerStats];
		        Create View PlayerStats as
               SELECT
                    p.ID,
                    P.FirstName || ' ' || p.LastName AS Player,
                    p.JerseyNumber,
                    t.Name AS Team,
                    GamesPlayed.Game AS G,
                    SUM(s.H)AS H,
                    SUM(s.RBI) AS RBI,
                    SUM(s.Singles)AS Singles,
                    SUM(s.Doubles)AS Doubles,
                    SUM(s.Triples)AS Triples,
                    SUM(s.HR) AS HR,
                    SUM(s.BB)AS BB,
                    SUM(s.PA)AS PA,
                    SUM(s.AB)AS AB,
                    SUM(s.Run)AS Run,
                    SUM(s.HBP)AS HBP,
                    SUM(s.StrikeOut)AS SO,
                    SUM(s.Out)AS Out,
                    printf(""%.3f"", IFNULL(SUM(s.H)*1.0/SUM(s.AB),0)) AS [AVG],
                    printf(""%.3f"", IFNULL((SUM(s.H)*1.0 + SUM(s.BB) + SUM(s.HBP))/(SUM(s.AB) + SUM(s.BB) + SUM(s.HBP)),0)) AS [OBP],
                    printf(""%.3f"", IFNULL(((SUM(s.Singles)*1.0 + 2 * SUM(s.Doubles)+ (3 *SUM(s.Triples) )+ 4*SUM(s.HR))/IFNULL(SUM(s.AB),1)),0)) AS [SLG],
                    printf(""%.3f"", IFNULL(
                      ((SUM(s.H)*1.0 + SUM(s.BB) + SUM(s.HBP)) / NULLIF(SUM(s.AB) + SUM(s.BB) + SUM(s.HBP), 0)) 
                      + 
                      ((SUM(s.Singles)*1.0 + 2 * SUM(s.Doubles) + 3 * SUM(s.Triples) + 4 * SUM(s.HR)) / NULLIF(SUM(s.AB), 0)), 
                      0
                    )) AS [OPS]
                    FROM ScorePlayers s
                    JOIN GameLineUps g ON g.ID = s.GameLineUpID
                    JOIN Games gm ON gm.ID  = g.GameID
                    JOIN Players p ON p.ID = g.PlayerID
                    JOIN (SELECT gl.PlayerID AS player,COUNT(gl.ID) AS Game FROM GameLineUps gl
                    GROUP BY PlayerID)GamesPlayed ON GamesPlayed.player=p.ID
                    JOIN Teams t ON t.ID = p.TeamID
                    GROUP BY p.ID
            ");

        }
    }
}
