
using Entity.Common;

namespace Interface.Common
{
    public interface IDividendTable
    {
        JsonResponse GetAllDividendTableLists(string CompCode);
    }
}
