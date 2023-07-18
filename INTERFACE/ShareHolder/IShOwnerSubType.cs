
using Entity.Common;

namespace Interface.ShareHolder
{
    public interface IShOwnerSubType
    {
        public JsonResponse GetAllShOwnerSubType(string shownertype);
        public JsonResponse GetShownerSubType(string shownertype, string shownersubtype);


    }
}
