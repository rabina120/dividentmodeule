using Entity.Common;
using Entity.ShareHolder;
using System.Collections.Generic;


namespace Interface.ShareHolder
{
    public interface IUploadFromExcel
    {
        public List<ATTShHolder> GetHolderDetails();

        public JsonResponse Get_DemateHolderInfo_temp(int? pageno, int? pagesize, out int? TotalRecords);

        public JsonResponse DeleteFromTemp(string username);
        public JsonResponse UploadHolderDetails(string userName);

    }
}
