using Entity.Common;
using ENTITY.Dividend;
using System.Collections.Generic;
using System.Data;


namespace INTERFACE.DividendManagement
{
    public interface ICalculationRepo
    {
        JsonResponse SaveCalculation(ATTCalculation data,string selectedOption);

        JsonResponse Calculate(/*List<ATTCalculation> data,*/ string selectedOption, string Bonus, string Divident,string compcode);

        JsonResponse SaveCalculationFromExcel(List<ATTCalculation> data, string selectedOption);
        // JsonResponse GetAllCalclationData(string selectedOption, int? pageNo, int? pageSize, out int TotalRecords);

        List<ATTCalculation> GetAllCalclationData(string selectedOption, string CompanyId,int? pageNo, int? pageSize, out int TotalRecords);


        JsonResponse BulkCopyCalculationData(DataSet ds, DataTable dt, string selectedOption);
    }
}
