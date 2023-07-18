
using Entity.Common;
using Entity.ShareHolder;

namespace Interface.ShareHolder
{
    public interface IShareHolderRelativeEntry
    {
        JsonResponse GetShHolder(string CompCode, string SelectedAction, string ShHolderNo, string UserName, string IP);
        JsonResponse GetMaxSN(string CompCode, string SelectedAction, string ShHolderNo, string UserName);
        JsonResponse GetRelativeShHolder(string CompCode, string SelectedAction, string ShHolderNo, string UserName, string IP);
        JsonResponse GetRelativeShHolderForUpdateDelete(string CompCode, string SelectedAction, string ShHolderNo, string UserName, string IP);
        JsonResponse SaveRelativeEntry(string CompCode, int ShHolderNo, string SN, ATTShHolder relativeShholder, string SelectedAction, string UserName, string IP);
    }
}
