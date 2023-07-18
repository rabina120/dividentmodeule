

using Entity.Certificate;
using Entity.Common;

namespace Interface.Certificate
{
    public interface ICertificateList
    {
        JsonResponse DisplayCertificateList(string CompCode, string UserName, string OrderBy, string Listtype, string IP);
        JsonResponse ExporttoExcel(string CompCode, string UserName, string OrderBy);
        JsonResponse DistributedUnDistributedList(ATTDuplicateCertificate ReportData, string OrderBy, string Listtype, string sharetype, string SelectedAction);
        JsonResponse DistributedUnDistributedListForPDF(ATTDuplicateCertificate ReportData, string OrderBy, string Listtype, string sharetype, string SelectedAction);

        JsonResponse AllCertificateList(ATTDuplicateCertificate ReportData, string OrderBy, string ShareOwnerType);
        JsonResponse AllCertificateListForPDF(ATTDuplicateCertificate ReportData, string OrderBy, string ShareOwnerType);
    }
}
