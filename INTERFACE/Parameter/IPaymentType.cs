

using Entity.Common;
using Entity.Parameter;

namespace Interface.Parameter
{
    public interface IPaymentType
    {
        public JsonResponse GetPaymentCode();

        public JsonResponse GetPaymentDetails(string CenterId, string UserName, string IPAddress);

        public JsonResponse SavePaymentDetails(ATTPamentType aTTPamentType, string ActionType, string UserName, string IPAddress);
    }
}
