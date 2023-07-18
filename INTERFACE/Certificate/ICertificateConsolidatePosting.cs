using Entity.Certificate;
using Entity.Common;
using System.Collections.Generic;

namespace Interface.Certificate
{
    public interface ICertificateConsolidatePosting
    {
        JsonResponse GetCertificateConsolidateCompanyData(string CompCode, string UserName, string IP);

        JsonResponse GetCertificate(string CompCode, string SplitNo, string ShholderNo, string UserName, string IP);
        JsonResponse PostCertificateConsolidateEntry(List<ATTCertificateConsolidatePosting> aTTCertificateConsolidatePostings, ATTCertificateConsolidatePosting recorddetails, string SelectedAction, string UserName, string IP);
    }
}
