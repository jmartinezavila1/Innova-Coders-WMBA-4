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
            var playerCount = await _context.Players
            .Where(p => p.Status == true)
            .CountAsync();
            var gameCount = await _context.Games
            .Where(g => g.Status == true)
            .CountAsync();
            var teamCount = await _context.Teams
           .Where(t => t.Status == true)
           .CountAsync();

            ViewBag.PlayerCount = playerCount;
            ViewBag.GameCount = gameCount;
            ViewBag.TeamCount = teamCount;
            return View();
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