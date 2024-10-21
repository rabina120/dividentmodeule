using Entity.Common;

namespace INTERFACE.FundTransfer
{
    public interface ITransactionProcessing
    {
        JsonResponse GetSourceBanks(string Compcode, string UserName, string IPAddress, string BankID = null);
        JsonResponse GetAllActiveBatch(string CompCode, string DivCode, string UserName, string IPAddress);
        JsonResponse GetDividendList(string CompCode, string UserName, string IPAddress);
        //TO CHECK THE STATUS OF THE BATCH
        JsonResponse CheckBatchStatus(string CompCode, string DivCode, string BatchID, string UserName, string IPAddress);
        //transacaiton processing
        JsonResponse TransactionProcessing(string DivCode, string CompCode, string BatchID, string BankID, string UserName, string IPAddress);
        //public JsonResponse TransactionProcessingTask(string DivCode, string CompCode, string BatchID, string BankUserName, string BankPassword, string SourceBankName, string SourceBankNumber, string UserName, string IPAddress);
        //public JsonResponse TransactionProcessingT(string DivCode, string CompCode, string BatchID, string BankUserName, string BankPassword, string SourceBankName, string SourceBankNumber, string UserName, string IPAddress);

    }
}
