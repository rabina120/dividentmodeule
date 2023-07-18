using Entity.Certificate;
using Entity.Common;
using System.Collections.Generic;

namespace Interface.Certificate
{
    public interface ICertificateSplit
    {
        JsonResponse GetCertificateDetailsByCertificateNo(string CompCode, string CertificateNo, string ActionType, string UserName, string IP);

        JsonResponse CheckCertificateNo(string CompCode, int? CertificateNo);

        JsonResponse SaveCertificateSplit(string CompCode, string CertificateNo, string srfrom, string srnoto, List<ATTCertificateSplit> aTTCertificates, string shholderno, string Splitdate, int shownertype, int sharetype, string remarks, string SelectedAction, string PageNo, string SplitNo, string UserName, string IP);
        JsonResponse CreateReport(string CompCode, string CertificateNo, string UserName);
    }
}
