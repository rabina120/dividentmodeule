using CDSMODULE.Helper;
using Entity.Common;
using Entity.ShareHolder;
using Interface.Common;
using Interface.Security;
using Interface.ShareHolder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CDSMODULE.Areas.HolderManagement.Controllers
{
    [Area("HolderManagement")]
    [Authorize]
    [AutoValidateAntiforgeryToken]
    public class UpdateApplicationPostingController : Controller
    {
        private readonly ILoggedinUser _IloggedinUser;
        private readonly ICheckUserAccess _checkUserAccess;
        public readonly IAudit _audit;
        public readonly IPostingUpdateApplication _postingUpdateApplication;

        public UpdateApplicationPostingController(ILoggedinUser _loggedInUser, ICheckUserAccess checkUserAccess, IAudit audit, IPostingUpdateApplication postingUpdateApplication)
        {
            this._IloggedinUser = _loggedInUser;
            this._checkUserAccess = checkUserAccess;
            _audit = audit;
            _postingUpdateApplication = postingUpdateApplication;
        }
        public IActionResult Index()
        {
            ViewBag.UserName = _IloggedinUser.GetUserNameToDisplay();
            string UserId = _IloggedinUser.GetUserId();
            JsonResponse res = _audit.auditSave(_IloggedinUser.GetUserNameToDisplay(), "Opened " + this.ControllerContext.RouteData.Values["controller"].ToString() + " Form", this.ControllerContext.RouteData.Values["controller"].ToString(), _IloggedinUser.GetUserIPAddress());
            if (res.IsSuccess)
            {
                if (_checkUserAccess.CheckIfAccessible(UserId, this.ControllerContext))
                    return View();
                return RedirectToAction("Index", "Dashboard", new { area = "Common" });
            }
            return RedirectToAction("Index", "Dashboard", new { area = "Common" });
        }
        [HttpPost]
        public JsonResponse GetAllApplicationList(string CompCode, string fromDate, string toDate)
        {
            return _postingUpdateApplication.GetAllApplicationList(_IloggedinUser.GetUserNameToDisplay(), CompCode, fromDate, toDate, _IloggedinUser.GetUserIPAddress());
        }
        [HttpPost]
        public JsonResponse SaveApplication(List<ATTShHolderForUpdate> aTTShHolders, string CompCode, string PostingDate, string SelectedAction, string PostingRemarks)
        {
            return _postingUpdateApplication.SaveApplication(aTTShHolders, _IloggedinUser.GetUserNameToDisplay(), CompCode, PostingDate, SelectedAction, PostingRemarks, _IloggedinUser.GetUserIPAddress());
        }

    }
}
