
using Entity.Common;
using Entity.Dividend;
using System.Collections.Generic;

namespace Interface.DividendProcessing
{
    public interface IDemateDividentPaymentPosting
    {
        public JsonResponse GetDemateDividentForApproval(string CompCode, string FromDate, string ToDate, string Divcode, string UserName, string IP);

        public JsonResponse PostDemateDividentPaymentPosting(List<ATTDemateDividentPaymentPosting> attDemateDividentPaymentPosting, string PostingRemarks,string PostingDate, string ActionType, string CompCode, string Divcode, string UserName, string IP);
    }
}
