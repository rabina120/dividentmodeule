using CDSMODULE.Helper;
using Entity.Common;
using Interface.Common;
using Interface.Security;
using Interface.Signature;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace CDSMODULE.Areas.SignatureHandling.Controllers
{
    [Authorize]
    [Area("SignatureHandling")]
    [AutoValidateAntiforgeryToken]
    public class SignatureVerificationPostingController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private IWebHostEnvironment _webHostEnvironment;
        private readonly IOptions<ReadConfig> _connectionString;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly ISignatureVerificationPosting _signatureVerificationPosting;
        private readonly IAudit _audit;

        public SignatureVerificationPostingController(IOptions<ReadConfig> connectionString, IWebHostEnvironment webHostEnvironment,
            ILoggedinUser _loggedInUser, ICheckUserAccess checkUserAccess, ISignatureVerificationPosting signatureVerificationPosting, IAudit audit)
        {
            this._connectionString = connectionString;
            this._webHostEnvironment = webHostEnvironment;
            this._loggedInUser = _loggedInUser;
            this._checkUserAccess = checkUserAccess;
            this._signatureVerificationPosting = signatureVerificationPosting;
            _audit = audit;
        }
        public IActionResult Index()
        {
            ViewBag.UserName = _loggedInUser.GetUserNameToDisplay();
            string UserId = _loggedInUser.GetUserId(); 
            JsonResponse res = _audit.auditSave(_loggedInUser.GetUserNameToDisplay(), "Opened " + this.ControllerContext.RouteData.Values["controller"].ToString() + " Form", this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress());
            if (res.IsSuccess)
            {
                if (_checkUserAccess.CheckIfAccessible(UserId, this.ControllerContext))
                {
                    return View();
                }

                return RedirectToAction("Index", "Dashboard", new { area = "Common" });
            }
            return RedirectToAction("Index", "Dashboard", new { area = "Common" });
        }
    }
}
