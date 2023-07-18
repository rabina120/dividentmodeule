using CDSMODULE.Helper;
using Entity.Common;
using ENTITY.Parameter;
using INTERFACE.Parameter;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Shareplus.Areas.ParameterSetup.Controllers
{
    [Authorize]
    [Area("ParameterSetup")]
    [AutoValidateAntiforgeryToken]
    public class OwnerCategorySetupController : Controller
    {
        private readonly IOwnerCategorySetup _ownerCategorySetup;
        private readonly ILoggedinUser _loggedinUser;
        public OwnerCategorySetupController(IOwnerCategorySetup ownerCategorySetup,ILoggedinUser loggedinUser)
        {
            _ownerCategorySetup = ownerCategorySetup;
            _loggedinUser = loggedinUser;
        }



        public IActionResult Index()
        {
            return View();
        }

       [HttpPost]

       public object SaveOwnerCategory(List<ATTOwnerCategory> ShownerType)
        {

            JsonResponse response = new JsonResponse();
            response = _ownerCategorySetup.SaveOwnerCategory(ShownerType, _loggedinUser.GetUserNameToDisplay(),_loggedinUser.GetUserIPAddress());


            return JsonConvert.SerializeObject(response);
        }

        [HttpPost]

        public object GetOwnerCategory(List<ATTOwnerCategory> ShownerType)
        {

            JsonResponse response = new JsonResponse();
            response = _ownerCategorySetup.GetOwnerCategory( _loggedinUser.GetUserNameToDisplay(), _loggedinUser.GetUserIPAddress());


            return JsonConvert.SerializeObject(response);
        }
    }

}
