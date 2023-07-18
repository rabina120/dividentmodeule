using Entity.Certificate;
using Entity.Common;
using System.Collections.Generic;

namespace Interface.Certificate

{
    public interface IPSLEntryPosting
    {
        JsonResponse GetPSLEntryCompanyData(string CompCode, string UserName, string IP);
        JsonResponse GetCertificate(string CompCode, string PSLNO, string ShholderNo);
        JsonResponse PostPSLEntryPosting(List<ATTPSLEntryPosting> aTTpSLEntryPostings, ATTPSLEntryPosting recorddetails, string SelectedAction, string UserName, string IP);
        JsonResponse ViewReport(string CompCode, string ReportType, string UserName, string IP);

    }
}
