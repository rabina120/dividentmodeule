using Dapper;
using Entity.Common;
using Entity.HolderInfo;
using Entity.ShareHolder;
using Interface.ShareHolder;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.ShareHolder
{
    public class UploadFromExcel : IUploadFromExcel
    {
        IOptions<ReadConfig> connectionString;

        public UploadFromExcel(IOptions<ReadConfig> _connectionString)
        {
            this.connectionString = _connectionString;
        }

        public JsonResponse DeleteFromTemp(String username)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection con = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    con.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@username", username);
                    con.Execute("DeleteFromDirtyDemateInfo", param, commandType: CommandType.StoredProcedure);

                    con.Close();
                    response.IsSuccess = true;
                    response.Message = "Data Cleared.";
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.ResponseData = ex;
                    response.HasError = true;
                }
                return response;
            }
        }

        public List<ATTShHolder> GetHolderDetails()
        {
            List<ATTShHolder> aTTShHolders;
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    aTTShHolders = connection.Query<ATTShHolder>("Get_Details_FROM_DEMATE_info", param: null, null, commandType: CommandType.StoredProcedure)?.ToList();

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return aTTShHolders;
        }

        public JsonResponse Get_DemateHolderInfo_temp(int? pageno, int? pagesize, out int? TotalRecords)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
                {
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_PageSize", pagesize);
                    param.Add("@P_PageNo", pageno);
                    param.Add("@P_Count", dbType: DbType.Int32, direction: ParameterDirection.Output);

                    List<ATTHolderDemateInfo> lst = SqlMapper.Query<ATTHolderDemateInfo>(
                                      connection, "Get_DirtyDemateHolderInfo", param, commandType: CommandType.StoredProcedure).ToList();
                    TotalRecords = param.Get<Int32>("@P_Count");

                    if (lst.Count > 0)
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = lst;
                        jsonResponse.TotalRecords = TotalRecords;

                    }
                    else
                    {
                        jsonResponse.Message = "No data found!";
                    }
                    return jsonResponse;
                }
            }
            catch (Exception ex)
            {
                jsonResponse.IsSuccess = false;
                jsonResponse.ResponseData = ex;
                jsonResponse.HasError = true;
                TotalRecords = 0;
                return jsonResponse;
            }

        }


        public JsonResponse UploadHolderDetails(string userName)
        {
            JsonResponse jsonResponse = new JsonResponse();

            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@date", DateTime.Now.ToString("yyyy-MM-dd"));
                    param.Add("@username", userName);
                    //connection.Query("Get_Details_FROM_DEMATE_info", param: null, null, commandType: CommandType.StoredProcedure)?.ToList();
                    jsonResponse = connection.Query<JsonResponse>("Upload_demateholderinfo", param, null, commandType: CommandType.StoredProcedure).FirstOrDefault();
                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.Message = ex.Message;
                }
            }

            return jsonResponse;
        }
    }
}
