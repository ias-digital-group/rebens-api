using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace ias.Rebens.api.Controllers
{
    public class UnicsulController : Controller
    {
        private IReportRepository _repo;
        private IHostingEnvironment _hostingEnvironment;

        public UnicsulController(IHostingEnvironment hostingEnvironment, IReportRepository reportRepository)
        {
            _repo = reportRepository;
            _hostingEnvironment = hostingEnvironment;

        }

        public IActionResult WeeklyReport()
        {
            var report = _repo.UnicsulWeekly();
            return View();
        }
    }
}
