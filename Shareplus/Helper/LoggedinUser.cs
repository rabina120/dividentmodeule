using Entity.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace CDSMODULE.Helper
{
    public interface ILoggedinUser
    {
        string GetUserName();
        string GetUserType();
        string GetUserRole();

        string GetUserId();

        string GetUserIPAddress();
        string GetUserNameToDisplay();
        bool IsLDAP();
        ATTSession GetSessionStorage();
        ATTCompany GetConnectedCompany();
    }
    public class _loggedInUser : ILoggedinUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;
        public _loggedInUser(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
        }

        public string GetUserName()
        {
            try
            {
                return _httpContextAccessor.HttpContext.User.Identity.Name;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        public string GetUserType()
        {
            try
            {

                return _httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(x => x.Type == "userType").Value;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        public string GetUserRole()
        {
            try
            {
                return _httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(x => x.Type == "userRole").Value;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        public string GetUserId()
        {
            try
            {
                return _httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(x => x.Type == "userId").Value;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }

        public string GetUserIPAddress()
        {
            try
            {
                return _httpContextAccessor.HttpContext.User.Claims.SingleOrDefault(x => x.Type == "userIpAddress").Value;
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }
        }
        public string GetUserNameToDisplay()

        {
            try
            {

                if (IsLDAP())
                {
                    return GetUserId();
                }
                else
                {
                    return GetUserName();
                }
            }
            catch (Exception ex)
            {
                return GetUserName();
            }

        }
        public ATTSession GetSessionStorage()
        {
            return _httpContextAccessor.HttpContext.Session.GetSessionStorage();
        }
        public ATTCompany GetConnectedCompany()
        {
            return _httpContextAccessor.HttpContext.Session.GetConnectedCompany();

        }
        public bool IsLDAP()
        {
            try
            {
                string isLDAP = _configuration.GetSection("LDAPAuthentication").GetSection("Enabled").Value;
                if (isLDAP.ToLower() == "true")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
