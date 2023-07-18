
using Entity.Common;

namespace Interface.Certificate
{
    public interface ICertificateEntry
    {
        JsonResponse GetShHolderInformation(string ShHolderNo, string CompCode, string SelectedAction, string UserName, string IPAddress);
        JsonResponse SearchCertificate(string ShHolderNoFrom, string ShHolderNoTo, string CertificateNoFrom, string CertificateNoTo, string SerialNoFrom, string SerialNoTo, string ShareKitttaFrom, string ShareKitttaTo, string CompCode, string UserName, string IPAddress);
        JsonResponse SaveCertificate(string CompCode, string SelectedAction, string ShHolderNo, string CertificateNo, string ShareType, string ShareKitta,
            string CertificateIssuedDate, string CertificateType, string StartSerialNo, string EndSerialNo, string ShOwnerType, string UserName, string IPAddress);
    }
}
