using Entity.Common;
using System.Data;

namespace Interface.ShareHolder
{
    public interface IUpdateDemateHolder
    {
        JsonResponse UploadHolderDetails(DataTable dt, string user);
        JsonResponse SaveHolderDetails(string user);
    }
}
