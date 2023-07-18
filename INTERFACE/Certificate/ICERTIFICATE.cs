using Entity.Common;

namespace Interface.Certificate
{
    public interface ICERTIFICATE
    {
        JsonResponse GetCertInformation(string CertNo, string compcode);
        JsonResponse LoadCertificateTable(string CertNo, string compcode);
    }
}
