using Entity.CDS;
using Entity.Common;
using System.Collections.Generic;

namespace Interface.CDS
{
    public interface IRematerializePosting
    {
        JsonResponse GetReMaterializeData(string CompCode);

        JsonResponse PostReMaterializeEntry(List<CertificateDemateDetails> certificateDemate, CertificateDemateDetails recordDetails, string ActionType, string UserName);

    }
}
