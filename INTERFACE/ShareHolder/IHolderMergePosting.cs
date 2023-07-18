
using Entity.Common;
using Entity.ShareHolder;
using System.Collections.Generic;

namespace Interface.ShareHolder
{
    public interface IHolderMergePosting
    {
        JsonResponse GetHolderForPosting(string CompCode, string FromDate, string ToDate, string Username);
        JsonResponse SaveHolderPosting(string CompCode, string Username, List<ATTMergeDetail> attHolderMergeLists, string SelectedAction, string PostingDate, string Remarks);
    }
}
