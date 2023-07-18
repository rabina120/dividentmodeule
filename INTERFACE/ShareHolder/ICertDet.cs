

using Entity.Common;
using Entity.ShareHolder;
using System.Collections.Generic;

namespace Interface.ShareHolder
{
    public interface ICertDet
    {
        public JsonResponse GetCertDet(int shholderno, string compcode);
        public JsonResponse GetCertStatuses();
        public JsonResponse UpdateCertificate(int shholderno, List<ATTCertDet> lisOfCertificates);
    }
}
