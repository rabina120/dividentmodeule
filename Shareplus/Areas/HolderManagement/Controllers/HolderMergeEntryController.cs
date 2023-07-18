using CDSMODULE.Helper;
using Entity.Common;
using Entity.ShareHolder;
using Interface.Common;
using Interface.ShareHolder;
using Interface.Signature;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CDSMODULE.Areas.HolderManagement.Controllers
{
    [Authorize]
    [Area("HolderManagement")]
    [AutoValidateAntiforgeryToken]
    public class HolderMergeEntryController : Controller
    {
        private readonly ILoggedinUser _IloggedinUser;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IHolderMergeEntry _holderMergeEntry;
        private readonly ISignature _signature;

        public HolderMergeEntryController(ILoggedinUser _loggedInUser, ICheckUserAccess checkUserAccess, IHolderMergeEntry holderMergeEntry, ISignature signature)
        {
            this._IloggedinUser = _loggedInUser;
            this._checkUserAccess = checkUserAccess;
            this._holderMergeEntry = holderMergeEntry;
            this._signature = signature;
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
        public JsonResponse GetMaxMergeNo(string CompCode)
        {
            return _holderMergeEntry.GetMaxMergeNo(CompCode);
        }

        [HttpPost]
        public JsonResponse GetHolderForMerge(string CompCode, string ShHolderNo, string selectedAction, string MergeNo = null)
        {
            return _holderMergeEntry.GetHolderForMerge(CompCode, ShHolderNo, selectedAction, _IloggedinUser.GetUserNameToDisplay(), MergeNo, Request.HttpContext.Connection.RemoteIpAddress.ToString());
        }

        [HttpPost]
        public JsonResponse SaveHolderForMerge(string CompCode, ATTShHolder shholder, ATTShHolder shHolderForMerge, string SelectedAction, string Remarks, string MergeDate, string MergeNo = null)
        {
            return _holderMergeEntry.SaveHolderForMerge(CompCode, shholder, shHolderForMerge, SelectedAction, _IloggedinUser.GetUserNameToDisplay(), Remarks, MergeDate, MergeNo, Request.HttpContext.Connection.RemoteIpAddress.ToString());
        }

        [HttpPost]
        public JsonResponse GetSignature(string CompCode, string ShHolderNo)
        {
            return _signature.GetSignature(CompCode, ShHolderNo);
        }
        [HttpPost]
        public JsonResponse GetMergeHolderList(string CompCode, string MergeNo = null)
        {
            return _holderMergeEntry.GetMergeHolderList(CompCode, MergeNo);
        }

    }
}
