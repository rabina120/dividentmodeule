using CDSMODULE.Helper;
using Entity.Common;
using Entity.Esewa;
using Entity.Security;
using Interface.Common;
using Interface.Security;
using INTERFACE.FundTransfer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.DirectoryServices.Protocols;
using System.Net;

namespace CDSMODULE.Areas.FundTransfer.Controllers
{
    [Authorize]
    [Area("FundTransfer")]
    [AutoValidateAntiforgeryToken]
    public class FundTransferProcessingController : Controller
    {
        private readonly ILoggedinUser _loggedInUser;
        private readonly ILogDetails _logDetails;
        private readonly IAudit _audit;
        private readonly ICheckUserAccess _checkUserAccess;
        private readonly ITransactionProcessing _batchProcessing;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IEService _eService;
        private IConfiguration _configuration;
        private readonly ILoginUser _login;

        public FundTransferProcessingController(ILoggedinUser _loggedInUser, ILogDetails logDetails, IConfiguration configuration,
            IAudit audit, ICheckUserAccess checkUserAccess, ITransactionProcessing batchProcessing, IWebHostEnvironment hostingEnvironment, IEService eService, ILoginUser login)
        {
            this._loggedInUser = _loggedInUser;
            _logDetails = logDetails;
            _audit = audit;
            _checkUserAccess = checkUserAccess;
            _batchProcessing = batchProcessing;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
            _eService = eService;
            _login = login;
        }
        public IActionResult Index()
        {
            ViewBag.UserName = _loggedInUser.GetUserNameToDisplay();
            ViewBag.UserRole = _loggedInUser.GetUserType();
            string UserId = _loggedInUser.GetUserId();
            JsonResponse res = _audit.auditSave(_loggedInUser.GetUserName(), "Opened " + this.ControllerContext.RouteData.Values["controller"].ToString() + " Form", this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress());
            if (res.IsSuccess)
            {
                if (_checkUserAccess.CheckIfAccessible(UserId, this.ControllerContext))
                    return View();
                return RedirectToAction("Index", "Dashboard", new { area = "Common" });
            }
            return RedirectToAction("Index", "Dashboard", new { area = "Common" });
        }

        [HttpPost]
        public JsonResponse VerifyLogin(string UserName, string Password, string Pin)
        {


            Entity.Common.JsonResponse jsonResponse = new Entity.Common.JsonResponse();
            jsonResponse.IsSuccess = false;

            ATTUserProfile user = new ATTUserProfile();
            if (UserName != null && Password != null)
            {
                try
                {
                    string nonEncryptedPassword = Password;
                    Password = Crypto.OneWayEncryter(Password);

                    bool isLDAP = _loggedInUser.IsLDAP();
                    jsonResponse.Token = "LDAP";
                    if (isLDAP)
                    {
                        jsonResponse = LdapAuthentication(UserName, nonEncryptedPassword);
                        //bool isUserID = int.TryParse(UserId, out int id);
                        //if (!isUserID) throw new Exception("Enter UserID only!!");
                    }
                    else jsonResponse.IsSuccess = true;
                    //if (Convert.ToInt32(UserId) == 0 || Convert.ToInt32(UserId)==75)
                    //{
                    //    jsonResponse.IsSuccess = true;
                    //}
                    if (jsonResponse.IsSuccess)
                    {
                        jsonResponse.Token = "login before";
                        string ip = Request.HttpContext.Connection.RemoteIpAddress.ToString();
                        jsonResponse = _login.LoginForSpecialPurpose(UserName, Password,Pin ,ip);
                        jsonResponse.Token = "login after";
                        //if (jsonResponse.IsSuccess)
                        //{
                        //    user = (ATTUserProfile)jsonResponse.ResponseData;
                        //    jsonResponse.Token = "attuser";
                        //    if (user.isLoginSucess)
                        //    {
                        //        if (DateTime.Compare(user.CookieExpireTime, user.CurrentTime) > 0)
                        //        {
                        //            if (isLDAP) user.HasPswChanged = true;

                        //            jsonResponse.IsSuccess = false;
                        //            jsonResponse.HasError = true;
                        //            jsonResponse.Message = "Logged In From Another Device !!!";
                        //            return jsonResponse;
                        //        }
                        //        else
                        //        {
                        //            if (jsonResponse.IsSuccess)
                        //            {
                        //                jsonResponse.Token = "login claim";
                        //                var claims = new List<Claim>();
                        //                claims.Add(new Claim(ClaimTypes.Name, user.UserName.ToUpper(), ClaimValueTypes.String));
                        //                claims.Add(new Claim("userType", user.UserType, ClaimValueTypes.String));
                        //                claims.Add(new Claim("userRole", user.UserRole, ClaimValueTypes.Integer));
                        //                claims.Add(new Claim("userId", user.UserId, ClaimValueTypes.String));
                        //                claims.Add(new Claim("userIpAddress", Request.HttpContext.Connection.RemoteIpAddress.ToString(), ClaimValueTypes.String));
                        //                jsonResponse.Token = "login identity";
                        //                //new
                        //                var userIdentity = new ClaimsIdentity(claims, "SecureLogin");


                        //                //old
                        //                //var userIdentity = new ClaimsIdentity("isAdmin");
                        //                //userIdentity.AddClaims(claims);

                        //                jsonResponse.Token = "login principal";
                        //                var userPrincipal = new ClaimsPrincipal(userIdentity);


                        //                 HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal,
                        //                    //await HttpContext.SignInAsync("SHAREPLUSCDSCookie", userPrincipal,
                        //                    new AuthenticationProperties
                        //                    {
                        //                        ExpiresUtc = DateTime.UtcNow.AddHours(1),
                        //                        IsPersistent = true,
                        //                        AllowRefresh = true,

                        //                    });
                        //                //var userId = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
                        //                //return RedirectToAction("Index", "Dashboard", new { area = "Common" });
                        //                jsonResponse.IsSuccess = true;
                        //                jsonResponse.HasPasswordChanged = user.HasPswChanged;
                        //                jsonResponse.Message = "Last Logged In On \n Date : "
                        //                    + user.LastLoggedInDateTime.ToString("yyyy-MM-dd") + "\n Time : "
                        //                    + user.LastLoggedInDateTime.ToString("t") + "\n From IP : "
                        //                    + user.LastLoggedInIPAddress;
                        //                jsonResponse.Token = "login finish";
                        //            }
                        //        }
                        //    }
                        //    else
                        //    {
                        //        jsonResponse.IsSuccess = false;
                        //        jsonResponse.HasError = true;
                        //    }
                        //}
                        //else
                        //{
                        //    jsonResponse.Token = "login failed";
                        //    _login.FailedLogin(UserId, Request.HttpContext.Connection.RemoteIpAddress.ToString());
                        //}
                    }
                }
                catch (Exception ex)
                {
                    jsonResponse.Message = ex.Message;
                    jsonResponse.IsSuccess = false;
                }
            }
            else
            {
                jsonResponse.Message = "UserName and Password Cant Be Empty ";
            }
            return jsonResponse;
        }

        public Entity.Common.JsonResponse LdapAuthentication(string username, string password)
        {
            Entity.Common.JsonResponse response = new Entity.Common.JsonResponse();

            try
            {
                string server = _configuration.GetSection("LDAPAuthentication").GetSection("Server").Value;
                LdapConnection lcon = new LdapConnection(server);
                NetworkCredential nc = new NetworkCredential(username, password, server);
                lcon.Credential = nc;
                lcon.AuthType = AuthType.Negotiate;
                lcon.Bind(nc);
                response.IsSuccess = true;

            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }
       
        //    if (username == "admin" && password == "password123" && pin == "1234")
        //    {
        //        jsonResponse.IsSuccess = true;
        //    }

        //    return jsonResponse;
        //}
        [HttpPost]
        public JsonResponse GetDividendList(string CompCode)
        {
            JsonResponse response = _batchProcessing.GetDividendList(CompCode, _loggedInUser.GetUserName(), _loggedInUser.GetUserIPAddress());
            if (response.HasError)
                _audit.errorSave(_loggedInUser.GetUserName(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)response.ResponseData);
            return response;
        }
        //for get list of active batch
        [HttpPost]
        public JsonResponse GetAllActiveBatch(string CompCode, String DivCode)
        {
            JsonResponse res = _batchProcessing.GetAllActiveBatch(CompCode, DivCode, _loggedInUser.GetUserName(), _loggedInUser.GetUserIPAddress());
            if (res.HasError)
                res = _audit.errorSave(_loggedInUser.GetUserName(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), (Exception)res.ResponseData);
            return res;
        }
        //checking the status of the batch
        [HttpPost]
        public JsonResponse CheckBatchStatus(string CompCode, string DivCode,string BatchID)
        {
            JsonResponse res = _batchProcessing.CheckBatchStatus(CompCode, DivCode, BatchID, _loggedInUser.GetUserName(), _loggedInUser.GetUserIPAddress());
            if (res.HasError)
                res = _audit.errorSave(_loggedInUser.GetUserName(), this.ControllerContext.RouteData.Values["controller"].ToString(), _loggedInUser.GetUserIPAddress(), new Exception(res.Message));
            return res;
        }
        ///Transcation Batch Processing
        [HttpPost]
        public JsonResponse TransactionProcessing(string CompCode, string DivCode, string BatchID, string BankID)
        {
            JsonResponse jsonResponse = new JsonResponse();
            jsonResponse = _batchProcessing.TransactionProcessing(DivCode, CompCode, BatchID, BankID, _loggedInUser.GetUserName(), _loggedInUser.GetUserIPAddress());
            return jsonResponse;
        }

        [HttpPost]
        public JsonResponse GetSourceBankList(string Compcode)
        {
            JsonResponse jsonResponse = new JsonResponse();
            jsonResponse = _batchProcessing.GetSourceBanks( Compcode,  _loggedInUser.GetUserName(), _loggedInUser.GetUserIPAddress());
            return jsonResponse;
        }
        [HttpPost]
        public async Task<IActionResult> GetData(string CompCode, string DivCode, string BatchID, string BatchStatus)
        {
            var request = new ATTDataTableRequest();

            request.Draw = Convert.ToInt32(Request.Form["draw"].FirstOrDefault());
            request.Start = Convert.ToInt32(Request.Form["start"].FirstOrDefault());
            request.Length = Convert.ToInt32(Request.Form["length"].FirstOrDefault());
            request.Search = new ATTDataTableSearch()
            {
                Value = Request.Form["search[value]"].FirstOrDefault()
            };
            request.Order = new ATTDataTableOrder[] {
                new ATTDataTableOrder()
                {
                    Dir = Request.Form["order[0][dir]"].FirstOrDefault(),
                    Column = Convert.ToInt32(Request.Form["order[0][column]"].FirstOrDefault()),
                    ColumnName =Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault(),

                    }
                };
            ATTDataTableResponse<ATTBatchProcessing> returnData = new ATTDataTableResponse<ATTBatchProcessing>();

            returnData = await _eService.GetBatchProcessingAsync(request, CompCode, DivCode, BatchID, BatchStatus, _loggedInUser.GetUserName());
            var jsonData = new { draw = returnData.Draw, recordsFiltered = returnData.RecordsFiltered, recordsTotal = returnData.RecordsTotal, data = returnData.Data };
            return Ok(jsonData);
        }

        public async Task<IActionResult> GetDataForFTransfer(string CompCode, string DivCode, string BatchID, string BatchStatus)
        {
            var request = new ATTDataTableRequest();

            request.Draw = Convert.ToInt32(Request.Form["draw"].FirstOrDefault());
            request.Start = Convert.ToInt32(Request.Form["start"].FirstOrDefault());
            request.Length = Convert.ToInt32(Request.Form["length"].FirstOrDefault());
            request.Search = new ATTDataTableSearch()
            {
                Value = Request.Form["search[value]"].FirstOrDefault()
            };
            request.Order = new ATTDataTableOrder[] {
                new ATTDataTableOrder()
                {
                    Dir = Request.Form["order[0][dir]"].FirstOrDefault(),
                    Column = Convert.ToInt32(Request.Form["order[0][column]"].FirstOrDefault()),
                    ColumnName =Request.Form["columns[" + Request.Form["order[0][column]"].FirstOrDefault() + "][name]"].FirstOrDefault(),

                    }
                };
            ATTDataTableResponse<ATTBatchProcessing> returnData = new ATTDataTableResponse<ATTBatchProcessing>();

            returnData = await _eService.GetBatchProcessingForFTransferAsync(request, CompCode, DivCode, BatchID, BatchStatus, _loggedInUser.GetUserName());
            var jsonData = new { draw = returnData.Draw, recordsFiltered = returnData.RecordsFiltered, recordsTotal = returnData.RecordsTotal, data = returnData.Data };
            return Ok(jsonData);
        }

    }
}
