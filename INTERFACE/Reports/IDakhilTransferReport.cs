using Entity.Common;

namespace INTERFACE.Reports
{
    public interface IDakhilTransferReport
    {
        JsonResponse GetReportDataForPDF(string Compcode, string SelectedAction,string ReportType,
			string FromDate,string ToDate,string RegnoFrom,string RegnoTo,
			string TranKittaFrom,string TranKittaTo, string BHolderNoFrom,
           string BHolderNoTo, string SHolderNoFrom, string SHolderNoTo,
            string Broker, string username,string Ipaddress);
		JsonResponse GetReportDataForExcel(string Compcode, string SelectedAction, string ReportType,
			string FromDate, string ToDate, string RegnoFrom, string RegnoTo,
			string TranKittaFrom, string TranKittaTo, string BHolderNoFrom,
           string BHolderNoTo, string SHolderNoFrom, string SHolderNoTo,
            string Broker, string username, string Ipaddress);



	}
}
