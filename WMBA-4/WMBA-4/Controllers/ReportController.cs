using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WMBA_4.CustomControllers;
using WMBA_4.Data;

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

            if (playerStats == null)
            {
                return NotFound();
            }
            return View(playerStats);
        }

    }
}
