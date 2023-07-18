
using Entity.Common;
using Entity.ShareHolder;
using System.Collections.Generic;

namespace Interface.ShareHolder
{
    public interface IShareHolderPosting
    {
        JsonResponse GetHolderForApproval(string CompCode, string SelectedAction,string FromDate,string ToDate, string UserName, string IP);

        JsonResponse PostShholderInfo(List<ATTShHolder> aTTShHolder, string CompCode, string Username, string SelectedRecordType, string IP);
    }
}
