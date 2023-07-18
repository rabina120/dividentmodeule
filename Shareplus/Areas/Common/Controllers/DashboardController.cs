using CDSMODULE.Helper;
using Entity.Common;
using Interface.Reports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CDSMODULE.Areas.Common.Controllers
{
    [Authorize]
    [Area("Common")]
    [AutoValidateAntiforgeryToken]
    public class DashboardController : Controller
    {
        private readonly ILoggedinUser IloggedinUser;
        private readonly IGenerateReport Ireport;

        public DashboardController(ILoggedinUser _IloggedinUser, IGenerateReport _Ireport)
        {
            this.IloggedinUser = _IloggedinUser;
            this.Ireport = _Ireport;
        }
        public IActionResult Index()
        {
            ViewBag.UserName = IloggedinUser.GetUserNameToDisplay();
            return View();
        }
        [HttpPost]
        public object GetDashBoardItems()
        {
            JsonResponse response = new JsonResponse();
            string compcode = IloggedinUser.GetConnectedCompany().CompCode;
            if (compcode != null)
            {
                if (IloggedinUser.GetUserType().ToLower() == "checker")
                    response = Ireport.GenerateReport(compcode, "S", IloggedinUser.GetUserNameToDisplay(), IloggedinUser.GetUserIPAddress());
                else
                {
                    response.Message = "You Dont Have Access to See Charts .";
                }
            }

            return response;

        }
    }
}