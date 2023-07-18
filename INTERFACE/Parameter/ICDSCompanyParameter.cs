

using Entity.Common;
using ENTITY.Parameter;
using System.Collections.Generic;

namespace INTERFACE.Parameter
{
    public interface ICDSCompanyParameter
    {
        public JsonResponse GetCDSCompanyParameter(string compCode);
        public JsonResponse SaveCDSCompanyParameterList(string compCode,List<ATTCDSCompanyParameter> companyParameterLists,string userName,string ipAddress);
        public JsonResponse UpdateCDSCompanyParameter(string compCode,ATTCDSCompanyParameter cdsParameter);
    }
}
