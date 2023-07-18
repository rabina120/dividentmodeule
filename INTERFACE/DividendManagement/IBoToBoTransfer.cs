using Entity.Common;
using Entity.Dividend;

namespace Interface.DividendManagement
{
    public interface IBoToBoTransfer
    {
        JsonResponse GetHolderInformation(string CompCode, string BOID, string UserName, string IPAddress, string Action);
        JsonResponse SaveHolderForBoidChange(ATTHolderForBoidChange HolderInfo);
        JsonResponse GetHolderChangelistForPosting(ATTHolderForBoidChange postingData);
        JsonResponse VerifyRejectHolderList(ATTHolderForBoidChange postingData);
    }
}
