

using Entity.Common;
using Entity.Dividend;
using System.Collections.Generic;

namespace Interface.DividendProcessing
{
    public interface IDividentPaymentEntryPosting
    {
        JsonResponse GetDividentPaymentForApproval(string CompCode,string FromDate, string ToDate, string Divcode, string UserName, string IP);

        JsonResponse PostDividentPaymentRequest(List<ATTDividentPaymentEntry> aTTDividentPaymentEntrys, ATTDividentPaymentEntry RecordDetails, string ActionType, string UserName, string IP);
    }
}
