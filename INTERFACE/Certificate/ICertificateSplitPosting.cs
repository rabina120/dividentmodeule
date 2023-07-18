

using Entity.Certificate;
using Entity.Common;
using System.Collections.Generic;

namespace Interface.Certificate
{
    public interface ICertificateSplitPosting
    {
        JsonResponse GetCertificateSplitCompanyData(string CompCode);

        JsonResponse PostCertificateSplitEntry(List<ATTCERTIFICATE> certificateDemate, ATTCERTIFICATE recordDetails, string ActionType, string UserName, string IP);
    }
}
