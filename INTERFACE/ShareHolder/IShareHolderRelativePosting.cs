
using Entity.Common;
using Entity.ShareHolder;
using System.Collections.Generic;

namespace Interface.ShareHolder
{
    public interface IShareHolderRelativePosting
    {
        JsonResponse GetHolderForPosting(string CompCode, string fromDate, string toDate, string UserName, string IP);
        JsonResponse SaveHolderPosting(string CompCode, List<ATTShHolderForRelative> attShHolderForRelatives, string SelectedAction, string ApprovedDate, string UserName, string IP);
    }
}
