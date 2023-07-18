

using Entity.Common;

namespace Interface.Security
{
    public interface IPaymentCenter
    {
        JsonResponse GetPaymentCenter();
        JsonResponse GetPaymentCenterHolder(string compcode, string shholderno);
    }
}
