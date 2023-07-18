
using Entity.Common;

namespace Interface.Security
{
    public interface IRoleDefine
    {
        JsonResponse SaveRole(string RoleName, string UserName, string IPAddress);

    }
}
