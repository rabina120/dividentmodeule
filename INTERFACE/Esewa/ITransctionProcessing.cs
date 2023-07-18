using Entity.Common;
using Entity.Esewa;

namespace Interface.Esewa
{
    public interface ITransctionProcessing
    {
        ATTAccountDetails ProcessTransction(string CompCode, string BatchNo, string DivCode);

        JsonResponse SaveOutputRemarks(ATTEsewaResponse EsewaData, string CompCode, string BatchNo, string Token);
        JsonResponse SaveErrorRemarks(string BatchId, ATTEncryptedDetails output = null, ATTEsewaResponse esewaResponse = null);
    }
}
