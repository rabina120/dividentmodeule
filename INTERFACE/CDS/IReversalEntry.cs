
using Entity.CDS;
using Entity.Common;
using System.Collections.Generic;

namespace Interface.CDS
{
    public interface IReversalEntry
    {
        JsonResponse GetDataFromDRN(string CompCode, string UserName, string ShholderNo, string DRNNO, string IP);
        JsonResponse GenerateReport(string CompCode, string UserName, string DateFrom, string DateTo);
        JsonResponse GetShholderInformation(string CompCode, string UserName, string ShholderNo, string IP);
        JsonResponse GetReversalCertificateList(string CompCode, string UserName, string SelectedAction, string CertNo = null);
        JsonResponse SaveReversalCertificate(string CompCode, string UserName, string SelectedAction, string Remarks, string ApprovalDate, List<CertificateDemateDetails> certificates, string DRNNO, string ShHolderNo, string IP);
    }
}
