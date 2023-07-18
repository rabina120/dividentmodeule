

using Entity.Certificate;
using Entity.Common;
using System.Collections.Generic;

namespace Interface.Certificate
{
    public interface ICertificateDistributionPosting
    {
        JsonResponse GetCertificateDistributionCompanyData(string CompCode, string startdate, string enddate, string UserName, string IP);

        JsonResponse PostCertificateDistributionEntry(List<ATTCERTIFICATE> certificateDemate, ATTCERTIFICATE recordDetails, string ActionType, string UserName, string IP);


    }
}
