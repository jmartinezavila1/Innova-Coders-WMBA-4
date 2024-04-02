using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using WMBA_4.CustomControllers;
using WMBA_4.Data;
using WMBA_4.Models;
using WMBA_4.ViewModels;

namespace WMBA_4.Controllers
{
    [Authorize]
    public class ReportController : CognizantController
    {
        private readonly WMBA_4_Context _context;

        public ReportController(WMBA_4_Context context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? TeamID, string searchString, string actionButton, string actionTButton, string sortDirection = "asc", string sortField = "Player", string sortTDirection = "asc", string sortTField = "Team", List<int> selectedPlayers = null, List<int> selectedTeams = null, Dictionary<int, string> playerRankings = null)
        {
            var playerStats = await _context.PlayerStats
                .OrderBy(p => p.Player)
                .ToListAsync();
            var teamStats = await _context.TeamStats.
                OrderBy(p => p.Team)
                .ToListAsync();

            //Filter
            //Count the number of filters applied - start by assuming no filters
            ViewData["Filtering"] = "btn-outline-secondary";
            int numberFilters = 0;

            // Filter playerStats based on searchString
            if (TeamID.HasValue)
            {
                teamStats = teamStats.Where(p => p.ID == TeamID).ToList();
                numberFilters++;
            }

            // Filter playerStats based on searchString
            if (!String.IsNullOrEmpty(searchString))
            {
                playerStats = playerStats.Where(p => p.Player.ToUpper().Contains(searchString.ToUpper())).ToList();
                numberFilters++;
            }
            if (numberFilters != 0)
            {
                //Toggle the Open/Closed state of the collapse depending on if we are filtering
                ViewData["Filtering"] = " btn-danger";
                //Show how many filters have been applied
                ViewData["numberFilters"] = "(" + numberFilters.ToString()
                    + " Filter" + (numberFilters > 1 ? "s" : "") + " Applied)";
                //Keep the Bootstrap collapse open
                //@ViewData["ShowFilter"] = " show";
            }

            //Team Sorting
            #region Team Sorting

            string[] sortTOptions = new[] { "Team", "G", "H", "RBI", "Singles", "Doubles", "Triples", "HR", "BB", "PA", "AB", "Run", "HBP", "SO", "Out", "AVG", "OBP", "SLG", "OPS" };

            if (!String.IsNullOrEmpty(actionTButton)) //Form Submitted!
            {
                if (!String.IsNullOrEmpty(actionTButton)) //Form Submitted!
                {
                    if (sortTOptions.Contains(actionTButton))//Change of sort is requested
                    {
                        if (actionTButton == sortTField) //Reverse order on same field
                        {
                            sortTDirection = sortTDirection == "asc" ? "desc" : "asc";
                        }
                        sortTField = actionTButton;//Sort by the button clicked
                    }
                }
            }
            if (sortTField == "Team")
            {
                if (sortTDirection == "asc")
                {
                    teamStats = teamStats
                        .OrderBy(p => p.Team).ToList();
                }
                else
                {
                    teamStats = teamStats
                        .OrderByDescending(p => p.Team).ToList();
                }
            }
            else if (sortTField == "G")
            {
                if (sortTDirection == "asc")
                {
                    teamStats = teamStats
                        .OrderBy(p => p.G).ToList();
                }
                else
                {
                    teamStats = teamStats
                        .OrderByDescending(p => p.G).ToList();
                }
            }

            //Filter by selected players for comparation

            if (selectedTeams != null && selectedTeams.Count > 0)
            {
                teamStats = teamStats.Where(p => selectedTeams.Contains(p.ID)).ToList();
            }
            #endregion



            //Player Sorting
            #region Player sorting

            //sorting sortoption array
            string[] sortOptions = new[] { "Player", "G", "H", "RBI", "Singles", "Doubles", "Triples", "HR", "BB", "PA", "AB", "Run", "HBP", "SO", "Out", "AVG", "OBP", "SLG", "OPS" };


            if (!String.IsNullOrEmpty(actionButton)) //Form Submitted!
            {
                if (!String.IsNullOrEmpty(actionButton)) //Form Submitted!
                {
                    if (sortOptions.Contains(actionButton))//Change of sort is requested
                    {
                        if (actionButton == sortField) //Reverse order on same field
                        {
                            sortDirection = sortDirection == "asc" ? "desc" : "asc";
                        }
                        sortField = actionButton;//Sort by the button clicked
                    }
                }
            }
            if (sortField == "Player")
            {
                if (sortDirection == "asc")
                {
                    playerStats = playerStats
                        .OrderBy(p => p.Player).ToList();
                }
                else
                {
                    playerStats = playerStats
                        .OrderByDescending(p => p.Player).ToList();
                }
            }
            else if (sortField == "G")
            {
                if (sortDirection == "asc")
                {
                    playerStats = playerStats
                        .OrderBy(p => p.G).ToList();
                }
                else
                {
                    playerStats = playerStats
                        .OrderByDescending(p => p.G).ToList();
                }
            }
            else if (sortField == "H")
            {
                if (sortDirection == "asc")
                {
                    playerStats = playerStats
                        .OrderBy(p => p.H).ToList();
                }
                else
                {
                    playerStats = playerStats
                        .OrderByDescending(p => p.H).ToList();
                }
            }
            else if (sortField == "RBI")
            {
                if (sortDirection == "asc")
                {
                    playerStats = playerStats
                        .OrderBy(p => p.RBI).ToList();
                }
                else
                {
                    playerStats = playerStats
                        .OrderByDescending(p => p.RBI).ToList();
                }
            }
            else if (sortField == "Singles")
            {
                if (sortDirection == "asc")
                {
                    playerStats = playerStats
                        .OrderBy(p => p.Singles).ToList();
                }
                else
                {
                    playerStats = playerStats
                        .OrderByDescending(p => p.Singles).ToList();
                }
            }
            else if (sortField == "Doubles")
            {
                if (sortDirection == "asc")
                {
                    playerStats = playerStats
                        .OrderBy(p => p.Doubles).ToList();
                }
                else
                {
                    playerStats = playerStats
                        .OrderByDescending(p => p.Doubles).ToList();
                }
            }
            else if (sortField == "Triples")
            {
                if (sortDirection == "asc")
                {
                    playerStats = playerStats
                        .OrderBy(p => p.Triples).ToList();
                }
                else
                {
                    playerStats = playerStats
                        .OrderByDescending(p => p.Triples).ToList();
                }
            }
            else if (sortField == "HR")
            {
                if (sortDirection == "asc")
                {
                    playerStats = playerStats
                        .OrderBy(p => p.HR).ToList();
                }
                else
                {
                    playerStats = playerStats
                        .OrderByDescending(p => p.HR).ToList();
                }
            }
            else if (sortField == "BB")
            {
                if (sortDirection == "asc")
                {
                    playerStats = playerStats
                        .OrderBy(p => p.BB).ToList();
                }
                else
                {
                    playerStats = playerStats
                        .OrderByDescending(p => p.BB).ToList();
                }
            }
            else if (sortField == "PA")
            {
                if (sortDirection == "asc")
                {
                    playerStats = playerStats
                        .OrderBy(p => p.PA).ToList();
                }
                else
                {
                    playerStats = playerStats
                        .OrderByDescending(p => p.PA).ToList();
                }
            }
            else if (sortField == "AB")
            {
                if (sortDirection == "asc")
                {
                    playerStats = playerStats
                        .OrderBy(p => p.AB).ToList();
                }
                else
                {
                    playerStats = playerStats
                        .OrderByDescending(p => p.AB).ToList();
                }
            }
            else if (sortField == "Run")
            {
                if (sortDirection == "asc")
                {
                    playerStats = playerStats
                        .OrderBy(p => p.Run).ToList();
                }
                else
                {
                    playerStats = playerStats
                        .OrderByDescending(p => p.Run).ToList();
                }
            }
            else if (sortField == "HBP")
            {
                if (sortDirection == "asc")
                {
                    playerStats = playerStats
                        .OrderBy(p => p.HBP).ToList();
                }
                else
                {
                    playerStats = playerStats
                        .OrderByDescending(p => p.HBP).ToList();
                }
            }
            else if (sortField == "SO")
            {
                if (sortDirection == "asc")
                {
                    playerStats = playerStats
                        .OrderBy(p => p.SO).ToList();
                }
                else
                {
                    playerStats = playerStats
                        .OrderByDescending(p => p.SO).ToList();
                }
            }
            else if (sortField == "Out")
            {
                if (sortDirection == "asc")
                {
                    playerStats = playerStats
                        .OrderBy(p => p.Out).ToList();
                }
                else
                {
                    playerStats = playerStats
                        .OrderByDescending(p => p.Out).ToList();
                }
            }
            else if (sortField == "AVG")
            {
                if (sortDirection == "asc")
                {
                    playerStats = playerStats
                        .OrderBy(p => p.AVG).ToList();
                }
                else
                {
                    playerStats = playerStats
                        .OrderByDescending(p => p.AVG).ToList();
                }
            }
            else if (sortField == "OBP")
            {
                if (sortDirection == "asc")
                {
                    playerStats = playerStats
                        .OrderBy(p => p.OBP).ToList();
                }
                else
                {
                    playerStats = playerStats
                        .OrderByDescending(p => p.OBP).ToList();
                }
            }
            else if (sortField == "SLG")
            {
                if (sortDirection == "asc")
                {
                    playerStats = playerStats
                        .OrderBy(p => p.SLG).ToList();
                }
                else
                {
                    playerStats = playerStats
                        .OrderByDescending(p => p.SLG).ToList();
                }
            }
            else if (sortField == "OPS")
            {
                if (sortDirection == "asc")
                {
                    playerStats = playerStats
                        .OrderBy(p => p.OPS).ToList();
                }
                else
                {
                    playerStats = playerStats
                        .OrderByDescending(p => p.OPS).ToList();
                }
            }


            //For adding player rankings
            if (playerRankings != null)
            {
                foreach (var playerRanking in playerRankings)
                {
                    var player = _context.Players.Find(playerRanking.Key);
                    if (player != null)
                    {
                        if (int.TryParse(playerRanking.Value, out int ranking))
                        {
                            player.Ranking = ranking;
                            _context.Update(player);
                        }
                    }
                }

                await _context.SaveChangesAsync();
            }

            //Filter by selected players for comparation

            if (selectedPlayers != null && selectedPlayers.Count > 0)
            {
                playerStats = playerStats.Where(p => selectedPlayers.Contains(p.ID)).ToList();
            }

            #endregion


            var model = new StatsVM
            {
                PlayerStats = playerStats,
                TeamStats = teamStats
            };


            return View(model);
        }

       
    }
}
