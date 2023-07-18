
using Entity.Common;
using Entity.ShareHolder;
using System.Collections.Generic;

namespace Interface.ShareHolder
{
    public interface IShHolderLockUnlockPosting
    {
        JsonResponse GetLockUnlockData(string CompCode, string FromDate, string ToDate, string RecordType, string Username, string IP);
        JsonResponse PostLockUnlockData(string CompCode, string RecordType, List<ATTShHolderLockUnlock> ShHolderLocks,
            string Remarks, string SelectedAction, string PostingDate, string UserName, string IP);
    }
}
