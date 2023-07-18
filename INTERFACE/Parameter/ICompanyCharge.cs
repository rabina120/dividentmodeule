
using Entity.Common;
using Entity.Parameter;

namespace Interface.Parameter
{
    public interface ICompanyCharge
    {
        public JsonResponse GetChargeCode(string CompCode);

        public JsonResponse GetCompanyChargeDetail(string CompCode, string UserName, string IPAddress);

        public JsonResponse GetCompanyCharge(string Compcode, string ChargeCode, string UserName, string IPAddress);

        public JsonResponse SaveCompanyCharge(ATTCompanyCharge aTTCompanyCharge, string ActionType, string UserName, string IPAddress);
    }
}
