using Entity.Common;
using ENTITY.FundTransfer;
using System.Collections.Generic;
using System.Data;

namespace INTERFACE.FundTransfer
{
    public interface IBatchProcessing
    {
        //TO GET ALL ACTIVE BATCH LIST
        JsonResponse GetAllActiveBatch(string CompCode, string DivCode, string UserName, string IPAddress);
        //TO CHECK THE STATUS OF THE BATCH
        JsonResponse CheckBatchStatus(string CompCode, string DivCode, string BatchID, string UserName, string IPAddress);
        //TO CREATE THE BATCH
        JsonResponse CreateBatch(string CompCode, string DivCode, string UserName, string IPAddress);
        //TO UPLOAD THE DATA FROM CDS
        JsonResponse UploadCDSData(DataTable CdsTable, string UserName, string IPAddress, string CompCode, string DivCode, string BatchID);
        //VALIDATE THE CDS DATA
        JsonResponse ValidateCDSData(string CompCode, string DivCode, string BatchID, string UserName, string IPAddress);
        //UPDATE BANK DETAILS USING THE  API
        JsonResponse UpdateBankDetailsFromAPI(string username, string ipaddress);
        //Update Bank Details in Database
        JsonResponse UpdateBankDetails(List<ATTBank> banks, string UserName, string IPAddress);
        //Update BankCode into System
        JsonResponse UpdateBankDetailsToSystem(string swifcode, string bankcode, string UserName, string IPAddress);
        //Get Bank List From Esewa Api
        JsonResponse GetBankDetailsFromAPI();
        //Get Bank List from Database
        public JsonResponse GetBanks(string username, string ipaddress, string controllerName);
        //ACCOUNT VALIDATION USING ESEWA API
        JsonResponse ValidateAccountDetailsBatch(string DivCode, string CompCode, string BatchID, string BankUserName, string BankPassword, string UserName, string IPAddress);
        JsonResponse ValidateAccountDetailsAsync(string DivCode, string CompCode, string BatchID, string BankUserName, string BankPassword, string UserName, string IPAddress);

        //transacaiton processing
        JsonResponse TransactionProcessing(string DivCode, string CompCode, string BatchID,  string SourceBankName, string SourceBankNumber, string sourceAccountName, string UserName, string IPAddress);

        //batch closed on no data/error
        JsonResponse CloseBatch(string DivCode, string CompCode, string BatchID, string UserName, string IPAddress, string Remarks);

    }
}
