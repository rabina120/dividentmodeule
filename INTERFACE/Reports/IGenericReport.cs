using Entity.Common;
using Entity.Reports;
using System.Collections.Generic;
using static Entity.Reports.ATTGenericReport;

namespace Interface.Reports
{
    public interface IGenericReport
    {
        JsonResponse GenerateReport(ReportName name, JsonResponse data, string[] ReportTitles, bool isNepali = false, bool isTotal = false, List<ATTShareHolderReportTotalBasedOn> aTTShareHolderReportTotalBasedOns = null,bool addSerialNo = false);
        public JsonResponse GenerateDemateRemateReport(JsonResponse jsonResponse, ATTReportTypeForDemateRemate ReportData);
    }
}
