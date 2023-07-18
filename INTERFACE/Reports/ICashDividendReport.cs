
using Entity.Common;

namespace Interface.Reports
{
    public interface ICashDividendReport
    {
        JsonResponse GenerateDataForReport(string CompCode, string DivCode, string SelectedReportType,string undoType, string HolderNoFrom, string HolderNoTo, string KittaFrom, string KittaTo, string DateFrom, string DateTo, string PaymentType, string Posted, string PaymentCenter, string BatchNo, bool WithBankDetails, string ShareType,int? Occupation, string ExportFileType, string UserName, string IPAddress, string SelectedReportName);
    }
}
