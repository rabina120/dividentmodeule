using CDSMODULE.Helper;
using Entity.Common;
using Entity.Signature;
using Interface.Common;
using Interface.Security;
using Interface.Signature;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Compression;

namespace CDSMODULE.Areas.SignatureHandling.Controllers
{
    [Authorize]
    [Area("SignatureHandling")]
    [AutoValidateAntiforgeryToken]
    public class SignatureBatchCaptureController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private IWebHostEnvironment _webHostEnvironment;
        private readonly IOptions<ReadConfig> _connectionString;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly ISignatureBatchCapture _signatureBatchCapture;
        private readonly IAudit _audit;

        public SignatureBatchCaptureController(IOptions<ReadConfig> connectionString, IWebHostEnvironment webHostEnvironment,
            ILoggedinUser _loggedInUser, ICheckUserAccess checkUserAccess, ISignatureBatchCapture signatureBatchCapture, IAudit audit)
        {
            this._connectionString = connectionString;
            this._webHostEnvironment = webHostEnvironment;
            this._loggedInUser = _loggedInUser;
            this._checkUserAccess = checkUserAccess;
            this._signatureBatchCapture = signatureBatchCapture;
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
        public JsonResponse SaveBatchSignature(string CompCode, List<ATTShHolderSignature> Signatures)
        {

            JsonResponse response = new JsonResponse();
            byte[] signatureByte = null;
            bool pass = false;
            try
            {
                foreach (ATTShHolderSignature signature in Signatures)
                {
                    if (signature.base64SignatureString != null)
                    {
                        signatureByte = imageToByteConverter.dataURL2byte(signature.base64SignatureString);
                        signature.signature = signatureByte;
                    }

                    Image image = null;
                    using (MemoryStream stream = new MemoryStream(signatureByte))
                    {
                        image = Image.FromStream(stream);
                    }
                    if (image.RawFormat.ToString().ToLower() == "jpeg" || image.RawFormat.ToString().ToLower() == "png" || image.RawFormat.ToString().ToLower() == "bmp")
                        pass = true;
                    else
                        pass = false;
                    //-------------------------------------------
                    //  Try to instantiate new Bitmap, if .NET will throw exception
                    //  we can assume that it's not a valid image
                    //-------------------------------------------

                    try
                    {
                        using (var bitmap = new System.Drawing.Bitmap(image))
                        {
                            pass = true;
                        }
                    }
                    catch (Exception)
                    {
                        pass = false;
                        response.Message = "Cannot Open File";
                    }


                }
                if (pass)
                {
                    response = _signatureBatchCapture.SaveBatchSignature(CompCode, Signatures, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
                    if (response.HasError)
                    {
                        response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
                    }
                    return response;
                }
                else
                {
                    response.Message = "Some Files Are Not In JPEG/PNG/BMP Format !!!";
                }
            }
            catch(Exception ex)
            {
                response.Message = ATTMessages.CANNOT_SAVE;

           }
            return response;


        }

        [HttpPost]
        public JsonResponse ExportBatchSignature(string CompCode, string StartShHolderNo, string EndShHolderNo)
        {
            JsonResponse response = _signatureBatchCapture.ExportBatchSignature(CompCode, StartShHolderNo, EndShHolderNo, _loggedInUser.GetUserNameToDisplay(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
            {
                response = _audit.errorSave(_loggedInUser.GetUserNameToDisplay(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            }
            else
            {
                if (response.IsSuccess)
                {
                    List<ATTShHolderSignature> signatures = (List<ATTShHolderSignature>)response.ResponseData;
                    try
                    {
                        byte[] zipthis = null;
                        var compressedFileStream = new MemoryStream();

                        using (compressedFileStream)
                        {
                            compressedFileStream.Seek(0, SeekOrigin.Begin);
                            using (var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Create, false))
                            {
                                foreach (ATTShHolderSignature s in signatures)
                                {
                                    //Create a zip entry for each attachment
                                    var zipEntry = zipArchive.CreateEntry(s.FileName + ".jpeg");
                                    //Get the stream of the attachment
                                    using (var originalFileStream = new MemoryStream(s.signature))
                                    {
                                        using (var zipEntryStream = zipEntry.Open())
                                        {
                                            //Copy the attachment stream to the zip entry stream
                                            originalFileStream.CopyTo(zipEntryStream);
                                        }
                                    }
                                }
                            }
                        }
                        zipthis = compressedFileStream.ToArray();
                        response.IsSuccess = true;
                        response.ResponseData = zipthis;
                        response.Message = "Company: " + CompCode + " Signatures" + StartShHolderNo + "-" + EndShHolderNo + ".zip";
                    }
                    catch (Exception ex)
                    {
                        response.Message = ex.Message;
                    }

                }
            }
            return response;
        }
    }
}
