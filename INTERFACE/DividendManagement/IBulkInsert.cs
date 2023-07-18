
using Entity.Common;
using System.Data;

namespace Interface.DividendManagement

{
    public interface IBulkInsert
    {
        JsonResponse GetDividendTableList(string compcode, string ShareType);
        JsonResponse Issue(string compcode, string divcode, string tablename1, string ActionType, int SheetId, DataSet ds, DataTable table, string UserName, string IPAddress);
    }
}
