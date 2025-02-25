﻿using Entity.Common;
using Entity.Esewa;
using System.Threading.Tasks;

namespace INTERFACE.FundTransfer
{
    public interface IEService
    {
        public Task<ATTDataTableResponse<ATTBatchProcessing>> GetBatchProcessingAsync(ATTDataTableRequest request, string CompCode, string DivCode, string BatchNo, string BatchStatus,string Username);
        Task<ATTDataTableResponse<ATTBatchProcessing>> GetBatchProcessingForFTransferAsync(ATTDataTableRequest request, string CompCode, string DivCode, string BatchNo, string BatchStatus, string Username);
        public Task<ATTDataTableResponse<ATTBatchProcessing>> GetBatchReportAsync(ATTDataTableRequest request, string CompCode, string DivCode, string BatchNo, string ReportType, string ReportSubType);
        //public Task<ATTDataTableResponse<ATTDirtyInfromationFromSystem>> LoadDataTable(ATTDataTableRequest request);
    }
}
