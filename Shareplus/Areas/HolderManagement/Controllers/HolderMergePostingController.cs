using CDSMODULE.Helper;
using Entity.Common;
using Entity.ShareHolder;
using Interface.Common;
using Interface.ShareHolder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CDSMODULE.Areas.HolderManagement.Controllers
{
    [Authorize]
    [Area("HolderManagement")]
    [AutoValidateAntiforgeryToken]
    public class HolderMergePostingController : Controller
    {
        private readonly ILoggedinUser _IloggedinUser;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IHolderMergePosting _holderMergePosting;

        public HolderMergePostingController(ILoggedinUser _loggedInUser, ICheckUserAccess checkUserAccess, IHolderMergePosting holderMergePosting)
        {
            this._IloggedinUser = _loggedInUser;
            this._checkUserAccess = checkUserAccess;
            this._holderMergePosting = holderMergePosting;
        }


        public IActionResult Index()
        {
            ViewBag.UserName = _IloggedinUser.GetUserNameToDisplay();
            string UserId = _IloggedinUser.GetUserId();
            if (_checkUserAccess.CheckIfAccessible(UserId, this.ControllerContext))
                return View();
            return RedirectToAction("Index", "Dashboard", new { area = "Common" });
        }

        [HttpPost]
        public JsonResponse GetAllMergeHoldersList(string CompCode, string FromDate, string ToDate)
        {
            return _holderMergePosting.GetHolderForPosting(CompCode, FromDate, ToDate, _IloggedinUser.GetUserNameToDisplay());
        }

        [HttpPost]
        public JsonResponse SaveHolderPosting(string CompCode, List<ATTMergeDetail> aTTMergeDetails, string SelectedAction, string PostingDate, string Remarks)
        {
            return _holderMergePosting.SaveHolderPosting(CompCode, _IloggedinUser.GetUserNameToDisplay(), aTTMergeDetails, SelectedAction, PostingDate, Remarks);
        }
    }
}
