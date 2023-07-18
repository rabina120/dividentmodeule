using Entity.CDS;
using Entity.Common;
using System.Collections.Generic;

namespace Interface.CDS
{
    public interface IDematerializePosting
    {
        JsonResponse GetParaCompChildList(string CompCode);
        JsonResponse GetDeMaterializeData(string CompCode, string FromDate, string ToDate, string RegNoFrom, string RegNoTo, string ISINNO, string CheckCA, string UserName, string IP);
        JsonResponse GetSingleDeMaterializeData(string CompCode, string DemateRegno, string RegNo, string ISINNo, string Remarks, string DRNNo, string UserName, string IPAddress);
        JsonResponse PostDeMaterializeEntry(List<CertificateDemateDetails> certificateDemate, CertificateDemateDetails recordDetails, string ActionType, string UserName, string IP);
    }
}
