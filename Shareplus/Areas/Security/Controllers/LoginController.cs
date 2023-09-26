using CDSMODULE.Helper;
using Entity.Common;
using Entity.Security;
using Interface.Security;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.DirectoryServices.Protocols;
using System.IO;
using System.Net;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CDSMODULE.Areas.Security.Controllers
{
    [Area("Security")]
    [AutoValidateAntiforgeryToken]
    public class LoginController : Controller
    {
        private readonly ILoginUser _login;
        private readonly ILoggedinUser _IloggedinUser;
        private readonly IConfiguration _congifuration;
        private readonly ICheckDatabaseConnection _checkDatabaseConnection;

        public LoginController(ILoginUser login, ILoggedinUser IloggedinUser, IConfiguration configuration, ICheckDatabaseConnection checkDatabaseConnection)
        {
            this._login = login;
            this._IloggedinUser = IloggedinUser;
            this._congifuration = configuration;
            _checkDatabaseConnection = checkDatabaseConnection;

        }
        public IActionResult Index()
        {

            if (TempData["loggedOut"] != null)
            {
                ViewBag.Message = TempData["loggedOut"];
                TempData.Remove("loggedOut");

            }
            if (_IloggedinUser.GetUserName() != null)
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Common" });
            }
            return View();
        }

        #region Login
        [HttpPost]

        public async Task<Entity.Common.JsonResponse> Index(string UserId, string Password)
        {
            Entity.Common.JsonResponse jsonResponse = new Entity.Common.JsonResponse();
            jsonResponse.IsSuccess = false;

            ATTUserProfile user = new ATTUserProfile();
            if (UserId != null && Password != null)
            {
                try
                {
                    string nonEncryptedPassword = Password;
                    Password = Crypto.OneWayEncryter(Password);

                    bool isLDAP = _IloggedinUser.IsLDAP();
                    jsonResponse.Token = "LDAP";
                    if (isLDAP)
                    {
                        jsonResponse = LdapAuthentication(UserId, nonEncryptedPassword);
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
                        jsonResponse = _login.Login(UserId, Password, ip);
                        jsonResponse.Token = "login after";
                        if (jsonResponse.IsSuccess)
                        {
                            user = (ATTUserProfile)jsonResponse.ResponseData;
                            jsonResponse.Token = "attuser";
                            if (user.isLoginSucess)
                            {
                                if (DateTime.Compare(user.CookieExpireTime, user.CurrentTime) > 0)
                                {
                                    if (isLDAP) user.HasPswChanged = true;

                                    jsonResponse.IsSuccess = false;
                                    jsonResponse.HasError = true;
                                    jsonResponse.Message = "Logged In From Another Device !!!";
                                    return jsonResponse;
                                }
                                else
                                {
                                    if (jsonResponse.IsSuccess)
                                    {
                                        jsonResponse.Token = "login claim";
                                        var claims = new List<Claim>();
                                        claims.Add(new Claim(ClaimTypes.Name, user.UserName.ToUpper(), ClaimValueTypes.String));
                                        claims.Add(new Claim("userType", user.UserType, ClaimValueTypes.String));
                                        claims.Add(new Claim("userRole", user.UserRole, ClaimValueTypes.Integer));
                                        claims.Add(new Claim("userId", user.UserId, ClaimValueTypes.String));
                                        claims.Add(new Claim("userIpAddress", Request.HttpContext.Connection.RemoteIpAddress.ToString(), ClaimValueTypes.String));
                                        jsonResponse.Token = "login identity";
                                        //new
                                        var userIdentity = new ClaimsIdentity(claims, "SecureLogin");


                                        //old
                                        //var userIdentity = new ClaimsIdentity("isAdmin");
                                        //userIdentity.AddClaims(claims);

                                        jsonResponse.Token = "login principal";
                                        var userPrincipal = new ClaimsPrincipal(userIdentity);

                                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal,
                                            //await HttpContext.SignInAsync("SHAREPLUSCDSCookie", userPrincipal,
                                            new AuthenticationProperties
                                            {
                                                ExpiresUtc = DateTime.UtcNow.AddHours(1),
                                                IsPersistent = true,
                                                AllowRefresh = true,

                                            });
                                        //var userId = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;
                                        //return RedirectToAction("Index", "Dashboard", new { area = "Common" });
                                        jsonResponse.IsSuccess = true;
                                        jsonResponse.HasPasswordChanged = user.HasPswChanged;
                                        jsonResponse.Message = "Last Logged In On \n Date : "
                                            + user.LastLoggedInDateTime.ToString("yyyy-MM-dd") + "\n Time : "
                                            + user.LastLoggedInDateTime.ToString("t") + "\n From IP : "
                                            + user.LastLoggedInIPAddress;
                                        jsonResponse.Token = "login finish";
                                    }
                                }
                            }
                            else
                            {
                                jsonResponse.IsSuccess = false;
                                jsonResponse.HasError = true;
                            }
                        }
                        else
                        {
                            jsonResponse.Token = "login failed";
                            _login.FailedLogin(UserId, Request.HttpContext.Connection.RemoteIpAddress.ToString());
                        }
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
        #endregion

        private Entity.Common.JsonResponse CheckLDAPCredentials(string LDAPServer)
        {
            Entity.Common.JsonResponse jsonResponse = new Entity.Common.JsonResponse();
            try
            {
                using (var connection = new LdapConnection(LDAPServer))
                {
                    connection.Bind();
                    jsonResponse.IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                jsonResponse.IsSuccess = false;
                jsonResponse.Message = ex.Message;
            }
            return jsonResponse;

        }
        public Entity.Common.JsonResponse LdapAuthentication(string username, string password)
        {
            Entity.Common.JsonResponse response = new Entity.Common.JsonResponse();

            try
            {
                string server = _congifuration.GetSection("LDAPAuthentication").GetSection("Server").Value;
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

        [Authorize]
        #region Logout
        [Area("Security")]
       
        public async Task<IActionResult> Logout()
        {
            Entity.Common.JsonResponse response = new Entity.Common.JsonResponse();
            try
            {
                response = _login.Logout(_IloggedinUser.GetUserId(), _IloggedinUser.GetUserIPAddress());
                if (response.IsSuccess)
                {
                    await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    TempData["loggedOut"] = "You have logged out.";
                    return RedirectToAction("Index", "Login");
                }
                else
                {
                    return RedirectToAction("Index");
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        [HttpPost]
        //[IgnoreAntiforgeryToken]
        public Entity.Common.JsonResponse CheckConnectionString()
        {
            Entity.Common.JsonResponse response = new Entity.Common.JsonResponse();


            //create instanace of database connection



            try
            {
                SqlConnection conn = new SqlConnection(Crypto.Decrypt(_congifuration.GetConnectionString("DefaultConnection")));
                response.Message = "Openning Connection ...";

                //open connection
                conn.Open();
                response.IsSuccess = true;
                response.Message = "Connection successful!";
            }
            catch (Exception e)
            {
                response.Message = "Error: " + e.Message;
            }




            //if (_congifuration.GetConnectionString("DefaultConnection") == null || _congifuration.GetConnectionString("DefaultConnection") == "")
            //{

            //}
            //else
            //{
            //    response.IsSuccess = true;
            //}
            return response;
        }

        [HttpPost]
        public Entity.Common.JsonResponse SetConnectionString(string ServerName, string Port, string DatabaseName, string ServerUserName, string ServerPassword, string LDAPServer)
        {
            Entity.Common.JsonResponse response = new Entity.Common.JsonResponse();
            try
            {
                string DBConn;
                if (Port != null)
                {
                    DBConn = "Data Source={0},{1};Initial Catalog={2};User ID={3};Password={4}";
                    DBConn = string.Format(DBConn, ServerName, Port, DatabaseName, ServerUserName, ServerPassword);
                }
                else
                {
                    DBConn = "Data Source={0};Initial Catalog={1};User ID={2};Password={3}";
                    DBConn = string.Format(DBConn, ServerName, DatabaseName, ServerUserName, ServerPassword);
                }
                response = _checkDatabaseConnection.CheckDatabaseConnection(DBConn);
                if (response.IsSuccess)
                {
                    if (LDAPServer != null)
                    {
                        response = CheckLDAPCredentials(LDAPServer);
                        if (!response.IsSuccess)
                        {
                            return response;
                        }
                    }
                    var config = new ConfigurationBuilder()
               .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
               .AddJsonFile("appsettings.json")
               .Build()
               .Get<RootConfig>();
                    config.ConnectionStrings.DefaultConnection = Crypto.Encrypt(DBConn);
                    config.EsewaKeys = new EsewaKeys();
                    config.LDAPAuthentication = new LDAPAuthentication();

                    if (LDAPServer != null)
                    {
                        config.LDAPAuthentication.Enabled = "True";
                        config.LDAPAuthentication.Server = LDAPServer;
                    }


                    var jsonWriteOptions = new JsonSerializerOptions()
                    {
                        WriteIndented = true
                    };
                    jsonWriteOptions.Converters.Add(new JsonStringEnumConverter());

                    var newJson = JsonSerializer.Serialize(config, jsonWriteOptions);

                    var appSettingsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.json");
                    System.IO.File.WriteAllText(appSettingsPath, newJson);
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
            }
            return response;

        }

    }
}


