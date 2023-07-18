
using Entity.Common;

namespace Interface.Security
{
    public interface ICompanyDetails
    {
        JsonResponse GetCompanyDetails(string CompCode);

    }
}
