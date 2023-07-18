using Entity.Common;
using Entity.Esewa;
using System.Collections.Generic;

namespace Interface.Esewa

{
    public interface IAccountValidation
    {

        ATTAccountDetails ProcessAccountValidation(string CompCode, string DivCode, string BatchNo);
        JsonResponse SaveOutputRemarks(List<ATTAccountValidationResponse> response, string CompCode, string BatchNo, string Token);
        JsonResponse SaveErrorRemarks(ATTEncryptedDetails output = null, ATTAccountValidationResponse response = null);
        void SaveBankDetails(ATTBanks aTTBanks);
    }
}
