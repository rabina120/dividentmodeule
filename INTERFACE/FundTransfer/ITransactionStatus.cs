using Entity.Common;
using Entity.Esewa;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace INTERFACE.FundTransfer
{
    public interface ITransactionStatus
    {
        JsonResponse GetDividendList(string CompCode, string UserName, string IPAddress);
        JsonResponse GetBatchList(string CompCode, string DivCode, string UserName, string IPAddress);
        Task<List<ATTBatchProcessing>> GetAccountValidatedData(ATTDataListRequest request, string CompCode, string DivCode, string BatchID, string BatchStatus,string Username);
        Task<List<ATTBatchProcessing>> GetAccountValidatedDataForFTransfer(ATTDataListRequest request, string CompCode, string DivCode, string BatchID, string BatchStatus, string Username);

        JsonResponse UpdateTransactionStatus(string CompCode, string BatchNo, string DivCode, string BankUserName, string BankPassword, string UserName, string IPAddress);

    }
}
