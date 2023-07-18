
using Entity.Common;
using Entity.ShareHolder;

namespace Interface.ShareHolder
{
    public interface IHolderMergeEntry
    {
        JsonResponse GetMaxMergeNo(string CompCode);
        JsonResponse GetHolderForMerge(string CompCode, string ShHolderNo, string selectedAction, string Username, string IP, string MergeNo = null);
        JsonResponse GetMergeHolderList(string CompCode, string MergeNo);
        JsonResponse SaveHolderForMerge(string CompCode, ATTShHolder shholder, ATTShHolder shHolderForMerge, string selectedAction, string Username, string IP, string Remarks, string MergeDate, string MergeNo = null);
    }
}
