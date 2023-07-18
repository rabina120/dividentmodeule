using Entity.Common;
using ENTITY.Parameter;
using System;
using System.Collections.Generic;
using System.Text;

namespace INTERFACE.Parameter
{
    public interface IOwnerCategorySetup
    {

        public JsonResponse SaveOwnerCategory(List<ATTOwnerCategory> ShownerType,string username,string ipaddress);
        JsonResponse GetOwnerCategory(string v1, string v2);
    }
}
