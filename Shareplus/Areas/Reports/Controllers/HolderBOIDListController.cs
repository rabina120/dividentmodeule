using CDSMODULE.Helper;
using Entity.Common;

using Interface.Common;
using Interface.DividendProcessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CDSMODULE.Areas.Reports.Controllers
{

    [Authorize]
    [Area("Reports")]
    [AutoValidateAntiforgeryToken]
    public class HolderBOIDListController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly IHolderBOIDList _holderBOIDList;
        private readonly ICheckUserAccess _checkUserAccess;
        public HolderBOIDListController(ILoggedinUser _loggedInUser, IHolderBOIDList holderBOIDList
            , ICheckUserAccess checkUserAccess)
        {
            this._loggedInUser = _loggedInUser;
            this._holderBOIDList = holderBOIDList;
            this._checkUserAccess = checkUserAccess;
        }
        public IActionResult Index()
        {
            ViewBag.UserName = _loggedInUser.GetUserNameToDisplay();

            if (_checkUserAccess.CheckIfAccessible(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext))
                return View();
            return RedirectToAction("Index", "Dashboard", new { area = "Common" });
        }
        [HttpPost]
        public JsonResponse GetHolderDetails(string CompCode, string HolderNo, string ShareType, string DivType)
        {
            return _holderBOIDList.GetHolderDetails(CompCode, HolderNo, ShareType, DivType, _loggedInUser.GetUserNameToDisplay());
        }
    }
}
