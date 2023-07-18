using CDSMODULE.Helper;
using Entity.Common;
using Entity.ShareHolder;
using Interface.Common;
using Interface.Security;
using Interface.ShareHolder;
using Interface.Signature;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;

namespace CDSMODULE.Areas.HolderManagement.Controllers
{
    [Area("HolderManagement")]
    [Authorize]
    [AutoValidateAntiforgeryToken]
    public class ShareHolderController : Controller
    {
        private readonly ILoggedinUser _IloggedinUser;
        private readonly IHolderInformation _holderInformation;
        private readonly IDistrict _district;
        private readonly IOccupation _occupation;
        private readonly IShOwnerType _shOwnerType;
        private readonly IShOwnerSubType _shOwnerSubType;
        private readonly ISignature _signature;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly IAudit _audit;
        public ShareHolderController(ILoggedinUser _loggedInUser, IHolderInformation holderInformation, IDistrict district,
            IOccupation occupation, IShOwnerType shOwnerType, IShOwnerSubType shOwnerSubType, ISignature signature, ICheckUserAccess checkUserAccess, IAudit audit)
        {
            _IloggedinUser = _loggedInUser;
            _holderInformation = holderInformation;
            _district = district;
            _occupation = occupation;
            _shOwnerSubType = shOwnerSubType;
            _shOwnerType = shOwnerType;
            _signature = signature;
            _checkUserAccess = checkUserAccess;
            _audit = audit;

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
        public JsonResponse GetHolderInformation(string ShHolderNo, string CompCode, string selectedAction)
        {
            JsonResponse res = _holderInformation.GetSHholder(ShHolderNo, CompCode, selectedAction, _IloggedinUser.GetUserNameToDisplay(), _IloggedinUser.GetUserIPAddress());
            if (res.HasError)
            {
                res = _audit.errorSave(_IloggedinUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _IloggedinUser.GetUserIPAddress(), (Exception)res.ResponseData);
            }
            return res;
        }

        [HttpPost]
        public JsonResponse GetAllDistrict()
        {
            JsonResponse res = _district.GetAllDistrict();
            if (res.HasError)
            {
                res = _audit.errorSave(_IloggedinUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _IloggedinUser.GetUserIPAddress(), (Exception)res.ResponseData);
            }
            return res;
        }

        [HttpPost]
        public JsonResponse GetDistrict(string distcode)
        {
            JsonResponse res = _district.GetDistrict(distcode);
            if (res.HasError)
            {
                res = _audit.errorSave(_IloggedinUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _IloggedinUser.GetUserIPAddress(), (Exception)res.ResponseData);
            }
            return res;
        }
        [HttpPost]
        public JsonResponse GetAllOccupation(string shownertype)
        {
            JsonResponse res = _occupation.GetAllOccupation(shownertype);
            if (res.HasError)
            {
                res = _audit.errorSave(_IloggedinUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _IloggedinUser.GetUserIPAddress(), (Exception)res.ResponseData);
            }
            return res;
        }

        [HttpPost]
        public JsonResponse GetAllShOwnerType()
        {
            JsonResponse res = _shOwnerType.GetAllShOwnerType();
            if (res.HasError)
            {
                res = _audit.errorSave(_IloggedinUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _IloggedinUser.GetUserIPAddress(), (Exception)res.ResponseData);
            }
            return res;
        }
        [HttpPost]
        public JsonResponse GetShOwnerType(string shOwnerType)
        {
            JsonResponse res = _shOwnerType.GetShownerType(shOwnerType);
            if (res.HasError)
            {
                res = _audit.errorSave(_IloggedinUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _IloggedinUser.GetUserIPAddress(), (Exception)res.ResponseData);
            }
            return res;

        }

        [HttpPost]
        public JsonResponse GetAllShOwnerSubType(string shOwnerType)
        {
            JsonResponse res = _shOwnerSubType.GetAllShOwnerSubType(shOwnerType);
            if (res.HasError)
            {
                res = _audit.errorSave(_IloggedinUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _IloggedinUser.GetUserIPAddress(), (Exception)res.ResponseData);
            }
            return res;
        }

        [HttpPost]
        public JsonResponse GetShOwnerSubType(string shOwnerType, string shOwnerSubType)
        {
            JsonResponse res = _shOwnerSubType.GetShownerSubType(shOwnerType, shOwnerSubType);
            if (res.HasError)
            {
                res = _audit.errorSave(_IloggedinUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _IloggedinUser.GetUserIPAddress(), (Exception)res.ResponseData);
            }
            return res;
        }

        [HttpPost]
        public JsonResponse GetSignature(string compcode, string holderno)
        {
            JsonResponse res = _signature.GetSignature(compcode, holderno);
            if (res.HasError)
            {
                res = _audit.errorSave(_IloggedinUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _IloggedinUser.GetUserIPAddress(), (Exception)res.ResponseData);
            }
            return res;
        }
        [HttpPost]
        public JsonResponse GetNewShHolderNo(string compcode)
        {
            JsonResponse res = _holderInformation.GetNewShHolderNo(compcode);
            if (res.HasError)
            {
                res = _audit.errorSave(_IloggedinUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _IloggedinUser.GetUserIPAddress(), (Exception)res.ResponseData);
            }
            return res;
        }

        //[HttpPost]
        //public string SaveShHolder(ATTShHolder aTTShHolder, string signature, string signFileLength, string filename, string updateRemarks, string selectedAction)
        //{
        //    aTTShHolder.UserName = _IloggedinUser.GetUserNameToDisplay();
        //    byte[]? signatures = null;
        //    if (signature != null)
        //    {
        //        signatures = imageToByteConverter.dataURL2byte(signature);
        //    }

        //    return JsonConvert.SerializeObject(_holderInformation.SaveShHolder(aTTShHolder, signatures, signFileLength, filename, updateRemarks, selectedAction));
        //}

        //Wiothout Signature
        [HttpPost]
        public string SaveShHolder(ATTShHolder aTTShHolder, string updateRemarks, string selectedAction)
        {
            aTTShHolder.UserName = _IloggedinUser.GetUserNameToDisplay();
            JsonResponse res = _holderInformation.SaveShHolder(aTTShHolder, updateRemarks, selectedAction, _IloggedinUser.GetUserNameToDisplay(), _IloggedinUser.GetUserIPAddress());
            if (res.HasError)
            {
                res = _audit.errorSave(_IloggedinUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _IloggedinUser.GetUserIPAddress(), (Exception)res.ResponseData);
            }
            return JsonConvert.SerializeObject(res);
        }


        [HttpPost]
        public JsonResponse GetHolderByQuery(string CompCode, string FirstName, string LastName, string FatherName, string GrandFatherName)
        {
            JsonResponse res = _holderInformation.GetHolderByQuery(CompCode, FirstName, LastName, FatherName, GrandFatherName, _IloggedinUser.GetUserNameToDisplay(), Request.HttpContext.Connection.RemoteIpAddress.ToString());
            if (res.HasError)
            {
                res = _audit.errorSave(_IloggedinUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _IloggedinUser.GetUserIPAddress(), (Exception)res.ResponseData);
            }
            return res;
        }
    }
}
