using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WMBA_4.Data;
using WMBA_4.Models;

namespace WMBA_4.Controllers
{
    public class HomeController : Controller
    {
        private readonly WMBA_4_Context _context;

        public HomeController(WMBA_4_Context context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            IQueryable<Game> games = _context.Games
           .Include(g => g.GameType)
           .Include(g => g.Location)
           .Include(g => g.Season)
           .Include(t => t.TeamGames)
               .ThenInclude(t => t.Team)
                   .ThenInclude(d => d.Division);

            var divisionCount = await _context.Divisions
            .Where(d => d.Status == true)
            .CountAsync();
            var playerCount = await _context.Players
            .Where(p => p.Status == true)
            .CountAsync();
            var gameCount = await _context.Games
            .Where(g => g.Status == true)
            .CountAsync();
            var teamCount = await _context.Teams
           .Where(t => t.Status == true)
           .CountAsync();

            ViewBag.DivisionCount = divisionCount;
            ViewBag.PlayerCount = playerCount;
            ViewBag.GameCount = gameCount;
            ViewBag.TeamCount = teamCount;
            ViewBag.TeamName= _context.Teams.FirstOrDefault()?.Name;

            return View(games);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}