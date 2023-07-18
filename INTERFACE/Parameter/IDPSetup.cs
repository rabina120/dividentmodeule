
using Entity.Common;
using Entity.Parameter;

namespace Interface.Parameter
{
    public interface IDPSetup
    {
        public JsonResponse GetDPCode();

        public JsonResponse LoadDPDetailList();
        public JsonResponse LoadDPDetailList(string UserName, string IPAddress);
        public JsonResponse GetDPDetails(string DPCode, string UserName, string IPAddress);
        public JsonResponse SaveDPDetails(ATTDPSetup aTTDPSetup, string Actiontype, string UserName, string IPAddress);
    }
}
