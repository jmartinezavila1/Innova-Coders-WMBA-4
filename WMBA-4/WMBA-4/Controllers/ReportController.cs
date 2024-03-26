using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMBA_4.CustomControllers;
using WMBA_4.Data;
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
        public async Task<IActionResult> Index()
        {
            var playerStats = await _context.PlayerStats.ToListAsync();

            var teamStats = await _context.TeamStats.ToListAsync();

            var model = new StatsVM
            {
                PlayerStats = playerStats,
                TeamStats = teamStats
            };

            return View(model);
        }

    }
}
