using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CDSMODULE.Areas.UserLogReport.Controllers
{
    [Authorize]
    [Area("UserLogReport")]
    [AutoValidateAntiforgeryToken]
    public class UserLogReportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

