


using Entity.Common;
using Entity.HolderInfo;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Interface.ShareHolder
{
    public interface IInformationFromSystem
    {
        Task<JsonResponse> GetInformationDetails(string ComCode, string DivCode, string ShareType);
        Task<List<ATTDirtyInfromationFromSystem>> LoadDataTable(ATTDataListRequest request);
        JsonResponse GetAllData(string ComCode, string DivCode, string ShareType, string UserName);
        Task UpdateDivTable(string tableName, string UserName, string ActionType, string LotNo, string ShHolderNo, string WarrantNo, string BankAddress, string bankname, string bankaccno);
        JsonResponse DeleteDirtyTableData();
    }
}
