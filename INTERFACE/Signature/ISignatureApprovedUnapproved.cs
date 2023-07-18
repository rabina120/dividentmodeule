using Entity.Common;
using System.Collections.Generic;

namespace Interface.Signature
{
    public interface ISignatureApprovedUnapproved
    {
        JsonResponse GetAllSignatureList(string CompCode, string UserName, string ip);
        JsonResponse GetSingleSignature(string CompCode, string ShHolderNo, string ip);
        JsonResponse GetUnApproveHolderDetail(string CompCode, string ShHolderNo, string ip, string username);
        JsonResponse SaveApprove(string CompCode, List<string> ShHolderNos, string ScannedBy, string ApprovedDate, string UserName, string SelectedAction, bool hasPassword, string ip, string Password = null);
        public JsonResponse SaveUnapprove(string CompCode, string ShHolderNo, string UserName, string SelectedAction, string ip);
    }
}
