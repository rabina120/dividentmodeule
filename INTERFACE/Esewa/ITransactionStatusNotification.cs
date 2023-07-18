using Entity.Common;
using Entity.Esewa;

namespace Interface.Esewa
{
    public interface ITransactionStatusNotification
    {
        JsonResponse SaveTransactionNotification(ATTEsewaResponse aTTEsewaResponse);
        void SaveErrorRemarks(ATTEncryptedDetails output, ATTEsewaResponse aTTEsewaResponse);

        void SaveData(string data);
    }
}
