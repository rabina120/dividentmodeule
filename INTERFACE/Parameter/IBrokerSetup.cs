
using Entity.Common;
using Entity.Parameter;

namespace Interface.Parameter
{
    public interface IBrokerSetup
    {
        public JsonResponse GetBrokerCode();

        public JsonResponse GetBrokerDetail(string Bcode, string UserName, string IPAddress);

        public JsonResponse SaveBrokerDetails(ATTBroker aTTBroker, string ActionType, string UserName, string IPAddress);
    }
}
