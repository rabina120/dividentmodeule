
using Entity.Common;
using Entity.ShareHolder;

namespace Interface.Parameter
{
    public interface IOccupationSetup
    {
        public JsonResponse GetOccupationCode();

        public JsonResponse GetOccupationDetails(string OccupationId, string UserName, string IPAddress);

        public JsonResponse LoadOccupationList(string UserName, string IPAddress);

        public JsonResponse SaveOccupationDetails(ATTOccupation aTTOccupation, string ActionType, string UserName, string IPAddress);
    }
}
