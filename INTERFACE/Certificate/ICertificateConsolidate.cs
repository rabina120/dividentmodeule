using Entity.Certificate;
using Entity.Common;
using System.Collections.Generic;

namespace Interface.Certificate
{
    public interface ICertificateConsolidate
    {
        JsonResponse GetShholderDetailsByShHolderNo(string CompCode, string ShholderNo, string SelectedAction, string UserName, string IP);
        JsonResponse GetCertificateDetails(string CompCode, string ShholderNo, string CertificateNo, string SelectedAction, string UserName, string IP);
        JsonResponse SaveCertificateConsolidate(string CompCode, string ShholderNo, List<ATTCertificateConsolidate> aTTCertificateConsolidate, string CertificateNo, string Splitdate, string remarks, string SelectedAction, string UserName, string IP);
    }
}
