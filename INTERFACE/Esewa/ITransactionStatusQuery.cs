using Entity.Common;
using Entity.Esewa;

namespace Interface.Esewa
{
    public interface ITransactionStatusQuery
    {
        ATTAccountDetails GetBatchTokenNo(string CompCode, string BatchNo, string DivCode);
        JsonResponse SaveSubTokenOutputRemarks(ATTEsewaResponse aTTEsewaResponse, string CompCode, string BatchNo, string Token);
        JsonResponse SaveErrorRemarks(ATTEncryptedDetails output, ATTEsewaResponse esewaResponse, string BatchId);
    }
}
