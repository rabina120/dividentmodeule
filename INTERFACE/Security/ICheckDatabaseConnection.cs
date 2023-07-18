

using Entity.Common;

namespace Interface.Security
{
    public interface ICheckDatabaseConnection
    {
        JsonResponse CheckDatabaseConnection(string ConnectionString);
    }
}
