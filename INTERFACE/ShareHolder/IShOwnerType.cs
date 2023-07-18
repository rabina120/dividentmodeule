
using Entity.Common;

namespace Interface.ShareHolder
{
    public interface IShOwnerType
    {
        public JsonResponse GetAllShOwnerType();
        public JsonResponse GetShownerType(string shownertype);
    }
}
