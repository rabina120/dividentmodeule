using Entity.Common;
using Entity.ShareHolder;
using System.Collections.Generic;

namespace Interface.CDS
{
    public interface IReMaterializeEntry
    {
        JsonResponse GetParaCompChild(string CompCode);

        JsonResponse GetHolderDetails(string Compcode, int Occupation, string ShHolderNo);

        JsonResponse GetMaxRemageRegNo(string Compcode);

        JsonResponse GetCertificate(string Compcode, int Certno, int Shownertype, int ShHolderno);
        JsonResponse SaveRematerializeCertificate(List<ATTCertDet> certificateList, string compCode, string demateRegNo, string shHolderNo, string TranDate, string bOID, string drnNo, string dPCode, string remarks, string regNO, string iSINNo, string bonusCode, string selectedAction, string v);
    }
}
