
using Entity.Common;

namespace Interface.Common
{
    public interface ICommon
    {
        JsonResponse GetShareOwnerType();
        JsonResponse GetOccupations(string shownertype);
        JsonResponse GetOccupations();
        JsonResponse GetSelectedOccupation(string occupationId);
        JsonResponse GetDistricts();
        JsonResponse GetSelectedDistrict(string distcode);
        JsonResponse GetShOwnerTypes(bool isLookUp = false);
        JsonResponse GetShOwnerSubTypes(string shOwnerTypeID);
        JsonResponse GetShOwnerSubTypes();

        JsonResponse GetCertificateStatus(string DependOn);
        JsonResponse GetAllShareTypes();

        JsonResponse SaveGetPdfReport(object json);

    }
}
