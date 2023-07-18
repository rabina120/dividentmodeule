
using Entity.Common;

namespace Interface.ShareHolder
{
    public interface IShHolderLockUnlockEntry
    {
        JsonResponse GetHolderForLockUnlock(string CompCode, string ShHolderNo, string RecordType, string SelectedAction, string Username, string IP);
        JsonResponse GetHolderByLockId(string CompCode, string LockId, string RecordType, string SelectedAction, string UserName, string IPAddress);
        JsonResponse GetMaxLockId(string CompCode);
        JsonResponse GetRecordShHolderLuckDetail(string CompCode, string UserName, string IPAddress, string SelectedAction = null, string RecordType = null);
        JsonResponse SaveHolderLockUnlock(string CompCode, string ShHolderNo, string RecordType, string SelectedAction,
            string LockId, string LockDate, string LockRemarks, string UserName, string IP, string UnlockDate = null, string UnlockRemarks = null);
    }
}
