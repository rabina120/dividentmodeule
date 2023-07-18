using Entity.Certificate;
using Entity.Common;
using Entity.Reports;
using System.Collections.Generic;

namespace Interface.Reports
{
    public interface ICertificateReports
    {
        public JsonResponse CertificateSplitReport(ATTCERTIFICATEREPORT ReportData,JsonResponse data, string rootPath);
        public JsonResponse CertificateSplitReportForSingle(List<ATTCertificateSplit> CertificateList, string rootPath);
        public JsonResponse CertificateConsolidateReport(ATTConsolidateReport ReportData, List<ATTConsolidateReport> ConsolidateData, string rootPath);

    }
}

