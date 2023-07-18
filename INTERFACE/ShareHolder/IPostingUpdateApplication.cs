
using Entity.Common;
using Entity.ShareHolder;
using System.Collections.Generic;

namespace Interface.ShareHolder
{
    public interface IPostingUpdateApplication
    {
        JsonResponse GetAllApplicationList(string UserName, string CompCode, string fromDate, string toDate, string IPAddress);
        JsonResponse SaveApplication(List<ATTShHolderForUpdate> aTTShHolders, string UserName, string CompCode, string PostingDate, string SelectedAction, string PostingRemarks, string IPAddress);
    }
}
