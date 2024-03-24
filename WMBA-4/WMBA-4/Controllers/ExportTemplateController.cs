using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Drawing;
using WMBA_4.CustomControllers;
using WMBA_4.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace WMBA_4.Controllers
{
    [Authorize(Roles = "Admin,RookieConvenor, IntermediateConvenor, SeniorConvenor")]
    public class ExportTemplateController : CognizantController
    {
        private readonly WMBA_4_Context _context;

        public ExportTemplateController(WMBA_4_Context context)
        {
            _context = context;
        }
        public IActionResult DownloadTemplate()
        {
            // Define headers
            var headers = new List<string> { "ID", "First Name", "Last Name", "Member ID", "Season", "Division", "Club", "Team" };

            using (ExcelPackage excel = new ExcelPackage())
            {
                var workSheet = excel.Workbook.Worksheets.Add("Template");

                // Add headers
                for (int i = 0; i < headers.Count; i++)
                {
                    workSheet.Cells[1, i + 1].Value = headers[i];
                }

                // Style headers
                using (ExcelRange headings = workSheet.Cells[1, 1, 1, headers.Count])
                {
                    headings.Style.Font.Bold = true;
                    var fill = headings.Style.Fill;
                    fill.PatternType = ExcelFillStyle.Solid;
                    fill.BackgroundColor.SetColor(Color.LightBlue);
                }

                // Autofit columns
                workSheet.Cells.AutoFitColumns();

                // Download the Excel
                try
                {
                    Byte[] theData = excel.GetAsByteArray();
                    string filename = "Template.xlsx";
                    string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    return File(theData, mimeType, filename);
                }
                catch (Exception)
                {
                    return BadRequest("Could not build and download the file.");
                }
            }
        }
    }
}
