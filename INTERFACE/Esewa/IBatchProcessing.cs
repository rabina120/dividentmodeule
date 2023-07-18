using Entity.Common;
using Entity.Esewa;
using System;
using System.Data;

namespace Interface.Esewa
{
    public interface IBatchProcessing
    {
        //TO GET ALL ACTIVE BATCH LIST
        JsonResponse GetAllActiveBatch(string CompCode, String DivCode, string UserName, string IPAddress);
        //TO CHECK THE STATUS OF THE BATCH
        JsonResponse CheckBatchStatus(string CompCode, string DivCode, string BatchID, string UserName, string IPAddress);
        //TO CREATE THE BATCH
        JsonResponse CreateBatch(string CompCode, string DivCode, string UserName, string IPAddress);
        //TO UPLOAD THE DATA FROM CDS
        JsonResponse UploadCDSData(DataTable CdsTable, string UserName, string IPAddress, string CompCode, string DivCode, string BatchID);
        //VALIDATE THE CDS DATA
        JsonResponse ValidateCDSData(string CompCode, string DivCode, string BatchID, string UserName, string IPAddress);
        //UPDATE BANK DETAILS USING THE ESEWA API
        JsonResponse UpdateBankDetailsFromEsewa(string username, string ipaddress);
        //Update Bank Details in Database
        JsonResponse UpdateBankDetails(ATTBanks banks, string UserName, string IPAddress);
        //Update BankCode into System
        JsonResponse UpdateBankDetailsToSystem(string swifcode, string bankcode, string UserName, string IPAddress);
        //Get Bank List From Esewa Api
        JsonResponse GetBankDetailsEsewa();
        //Get Bank List from Database
        public JsonResponse GetBanks(string username, string ipaddress, string controllerName);
        //ACCOUNT VALIDATION USING ESEWA API
        JsonResponse ValidateAccountDetails(string DivCode, string CompCode, string BatchID, string BankUserName, string BankPassword, string UserName, string IPAddress);
        JsonResponse ValidateAccountDetailsBatch(string DivCode, string CompCode, string BatchID, string BankUserName, string BankPassword, string UserName, string IPAddress);

        //transacaiton processing
        JsonResponse TransactionProcessing(string DivCode, string CompCode, string BatchID, string BankUserName, string BankPassword, string SourceBankName, string SourceBankNumber, string UserName, string IPAddress);

        //batch closed on no data/error
        JsonResponse CloseBatch(string DivCode, string CompCode, string BatchID, string UserName, string IPAddress, string Remarks);

    }
}
