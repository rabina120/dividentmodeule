using Entity.Certificate;
using Entity.Common;
using ENTITY.Certificate;
using System.Collections.Generic;

namespace Interface.Certificate
{
    public interface IPrint
    {
        JsonResponse PrintCertificates(List<ATTDuplicateCertificate> list, string CompCode, string Username, string IPAddress);
        JsonResponse GetAllPrintFields();
        JsonResponse GetAllPrintFieldsWithCoordinates(string CompCode);
        JsonResponse SaveCompanyCoordinates(List<ATTCompanyCoordinates> cordList, string CompCode);
    }
}
