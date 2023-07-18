using CDSMODULE.Helper;
using Entity.Common;
using Interface.Common;
using Interface.Security;
using Interface.Signature;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Drawing;
using System.IO;

namespace CDSMODULE.Areas.SignatureHandling.Controllers
{
    [Authorize]
    [Area("SignatureHandling")]
    [AutoValidateAntiforgeryToken]
    public class SignatureIndividualCaptureController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;

        private readonly ICheckUserAccess _checkUserAccess;
        private readonly ISignatureIndividualCapture _signatureIndividualCapture;
        private readonly IAudit _audit;

        public SignatureIndividualCaptureController(
            ILoggedinUser _loggedInUser, ICheckUserAccess checkUserAccess, ISignatureIndividualCapture signatureIndividualCapture, IAudit audit)
        {

            this._loggedInUser = _loggedInUser;
            this._checkUserAccess = checkUserAccess;
            this._signatureIndividualCapture = signatureIndividualCapture;
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
                    return View();
                return RedirectToAction("Index", "Dashboard", new { area = "Common" });
            }
            return RedirectToAction("Index", "Dashboard", new { area = "Common" });
        }

        [HttpPost]
        public JsonResponse GetShHolderInformation(string CompCode, string ShHolderNo, string SelectedAction)
        {
            JsonResponse response = _signatureIndividualCapture.GetShHolderInformation(CompCode, ShHolderNo, SelectedAction, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            return response;
        }
        [HttpPost]
        public JsonResponse SaveSignatureInformation(string CompCode, string ShHolderNo, string ScannedBy, string Signature, int? fileLength, string SelectedAction)
        {
            byte[] signatures = null;
            if (Signature != null)
            {
                signatures = imageToByteConverter.dataURL2byte(Signature);
            }
            if (SelectedAction != "D")
            {
                Image image = null;
                using (MemoryStream stream = new MemoryStream(signatures))
                {
                    image = Image.FromStream(stream);
                }
                if (image.RawFormat.ToString().ToLower() == "jpeg" || image.RawFormat.ToString().ToLower() == "png" || image.RawFormat.ToString().ToLower() == "bmp")
                {
                    JsonResponse response = _signatureIndividualCapture.SaveSignatureInformation(CompCode, ShHolderNo, ScannedBy, signatures, fileLength, SelectedAction, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
                    if (response.HasError)
                    {
                        response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
                    }
                    return response;
                }
                else
                {
                    JsonResponse response = new JsonResponse();
                    response.Message = "Please Input Image with JPEG/PNG/BMP data type !!!";
                    return response;
                }
            }
            else
            {
                JsonResponse response = _signatureIndividualCapture.SaveSignatureInformation(CompCode, ShHolderNo, ScannedBy, signatures, fileLength, SelectedAction, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
                if (response.HasError)
                {
                    response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
                }
                return response;
            }

        }
    }
}
