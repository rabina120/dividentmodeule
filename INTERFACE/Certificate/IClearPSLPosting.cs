

using Entity.Certificate;
using Entity.Common;
using System.Collections.Generic;

namespace Interface.Certificate
{
    public interface IClearPSLPosting
    {
        public JsonResponse GetClearPSLPostingCompanyData(string CompCode, string UserName, string IP);
        public JsonResponse PostPSLClearPosting(List<ATTClearPSLPosting> aTTpSLClearPostings, ATTClearPSLPosting recorddetails, string SelectedAction, string UserName, string IP);
        public JsonResponse GetSingleClearPSLData(string CompCode, string PSL_CLEAR_NO, int ShholderNo);
        public JsonResponse ViewReport(string CompCode, string ReportType, string UserName, string IP);
    }
}
