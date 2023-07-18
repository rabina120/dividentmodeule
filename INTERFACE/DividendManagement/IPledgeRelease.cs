using Entity.Common;
using Entity.DemateDividend;
using System.Collections.Generic;

namespace INTERFACE.DividendManagement
{
    public interface IPledgeRelease
    {
        JsonResponse SavePRList(string CompCode, string DivCode, List<ATTPledgeReleaseDataList> PRList, string Status, string SelectedAction, string Username, string IPAddress);
    }
}
