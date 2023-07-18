using CDSMODULE.Helper;
using Entity.Common;
using Interface.Certificate;
using Interface.Common;
using Interface.Security;
using Microsoft.AspNetCore.Mvc;

namespace CDSMODULE.Areas.Certificate.Controllers
{
    [Area("Certificate")]
    [AutoValidateAntiforgeryToken]
    public class CertificateHistoryController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly ICERTIFICATE _CERTIFICATE;
        private readonly IAudit _audit;
        public CertificateHistoryController(IAudit audit,ICheckUserAccess checkUserAccess, ILoggedinUser _loggedInUser, ICERTIFICATE CERTIFICATE)
        {
            _checkUserAccess = checkUserAccess;
            this._loggedInUser = _loggedInUser;
            _CERTIFICATE = CERTIFICATE;
            this._audit = audit;
        }
        public IActionResult Index()
        {
            JsonResponse res = _audit.auditSave(_loggedInUser.GetUserNameToDisplay(), "Opened " + this.ControllerContext.RouteData.Values["controller"].ToString() + " Form", this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress());

            ViewBag.UserName = _loggedInUser.GetUserNameToDisplay();
            if (_checkUserAccess.CheckIfAccessible(_loggedInUser.GetUserId(), this.ControllerContext))
                return View();
            return RedirectToAction("Index", "Dashboard", new { area = "Common" });

        }
        public JsonResponse GetCertInformation(string CertNo, string compcode)
        {
            return _CERTIFICATE.GetCertInformation(CertNo, compcode);
        }
        public JsonResponse LoadCertificateTable(string CertNo, string compcode)
        {
            return _CERTIFICATE.LoadCertificateTable(CertNo, compcode);
        }

    }
}
