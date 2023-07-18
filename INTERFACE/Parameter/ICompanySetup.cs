
using Entity.Common;
using Entity.Parameter;

namespace Interface.Parameter
{
    public interface ICompanySetup
    {
        public JsonResponse GetCompanyCode();

        public JsonResponse SaveCompanyDetails(ATTCompanySetup aTTCompanySetup, string ActionType, string UserName);

        public JsonResponse GetCompanyDetails(string CompCode);
    }
}
