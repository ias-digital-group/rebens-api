using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ias.Rebens.Helper;
using ias.Rebens.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace ias.Rebens.api.Controllers
{
    public class UnicsulController : Controller
    {
        private IReportRepository _repo;
        private IHostingEnvironment _hostingEnvironment;
        private ExcelHelper _excelHelper;

        public UnicsulController(IHostingEnvironment hostingEnvironment, IReportRepository reportRepository)
        {
            _repo = reportRepository;
            _hostingEnvironment = hostingEnvironment;
            _excelHelper = new ExcelHelper();

        }

        public async Task<IActionResult> WeeklyReport()
        {
            var report = _repo.UnicsulWeekly();

            string webRootPath = _hostingEnvironment.WebRootPath;
            string newPath = Path.Combine(webRootPath, "files");
            if (!Directory.Exists(newPath))
                Directory.CreateDirectory(newPath);
            string fileName = $"unicsul_{DateTime.UtcNow.ToString("yyyyMMddHHmmss")}.xlsx";
            if(_excelHelper.UnicsulReport(report, Path.Combine(newPath, fileName)))
            {
                MemoryStream memory = new MemoryStream();
                using (var stream = new FileStream(Path.Combine(newPath, fileName), FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                return File(memory, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }

            return null;
        }
    }
}
