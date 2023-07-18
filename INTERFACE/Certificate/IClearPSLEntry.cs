



using Entity.Certificate;
using Entity.Common;
using System.Collections.Generic;

namespace Interface.Certificate
{
    public interface IClearPSLEntry
    {
        JsonResponse SearchHolderPSL(string compCode, string shholderNo, string SelectedAction, string UserName, string IP);
        JsonResponse GetPSLInformation(string compCode, string shholderNo, string selectedAction, string PSLNo, string UserName, string IP);
        JsonResponse SavePSLClearEntry(List<ATTPSLEntryClear> PSLEntry, string CompCode, int ShholderNo, int pslno, int PSL_clear_No, string Charge, string ClearedDt, string Remark, string UserName, string Issuedup, string SelectedAction, string IP);
    }
}
