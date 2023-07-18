
using Entity.Common;

namespace Interface.ShareHolder
{
    public interface IDistrict
    {
        public JsonResponse GetAllDistrict();
        public JsonResponse GetDistrict(string distcode);
    }
}
