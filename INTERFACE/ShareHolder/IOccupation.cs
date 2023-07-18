
using Entity.Common;

namespace Interface.ShareHolder
{
    public interface IOccupation
    {
        public JsonResponse GetAllOccupation(string shownertype);
        public JsonResponse GetOccupation(string occupationId);


    }
}
