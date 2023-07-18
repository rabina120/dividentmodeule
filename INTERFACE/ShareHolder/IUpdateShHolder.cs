
using Entity.Common;
using Entity.ShareHolder;

namespace Interface.ShareHolder
{
    public interface IUpdateShHolder
    {
        JsonResponse GetApplicationInformation(string CompCode, string ApplicationNo, string UserName, string IPAddress);
        JsonResponse SaveApplicationUpdate(ATTShHolder shholder, string ApplicaitonNo, string UserName, string IPAddress);
    }
}
