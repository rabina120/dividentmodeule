
using Dapper;
using Entity.Common;
using Entity.Dividend;
using Interface.DividendProcessing;
using Microsoft.Extensions.Options;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.DividendProcessing
{
    public class HolderBOIDListRepo : IHolderBOIDList
    {
        IOptions<ReadConfig> _connectionString;

        public HolderBOIDListRepo(IOptions<ReadConfig> connectionString)
        {
            this._connectionString = connectionString;
        }
        public JsonResponse GetHolderDetails(string CompCode, string HolderNo, string ShareType, string DivType, string UserName)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    HolderBOIDResponse HolderRepsonse = new HolderBOIDResponse();

                    #region Demate
                    if (ShareType == "2")
                    {
                        param.Add("@P_CompCode", CompCode);
                        param.Add("@P_ShareType", ShareType);
                        param.Add("@P_DivType", DivType);

                        HolderRepsonse.HolderBOIDs = connection.Query<HolderBOID>("Get_Dividend_List_HOLDERBOID", param, commandType: CommandType.StoredProcedure)?.ToList();
                        if (HolderRepsonse.HolderBOIDs.Count > 0)
                        {

                            param.Add("@P_HolderNo", HolderNo);
                            param.Add("@P_UserName", UserName);
                            foreach (HolderBOID holder in HolderRepsonse.HolderBOIDs)
                            {
                                param.Add("@P_TableName", holder.tablename);
                                holder.holderBOIDInfo = connection.Query<HolderBOIDTable>("GET_ALL_HOLDER_INFO", param, commandType: CommandType.StoredProcedure)?.ToList();
                            }
                            jsonResponse.ResponseData = HolderRepsonse;
                            jsonResponse.IsSuccess = true;
                        }
                        else
                        {
                            jsonResponse.Message = "No Records Found !!!.";
                        }
                    }
                    #endregion

                    #region Physical
                    if (ShareType == "1")
                    {

                        param.Add("@P_CompCode", CompCode);
                        param.Add("@P_HolderNo", HolderNo);
                        HolderRepsonse = connection.Query<HolderBOIDResponse>("GET_SHHOLDER_INFORMATION_FOR_HOLDERLIST", param, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                        if (HolderRepsonse != null)
                        {
                            param = new DynamicParameters();
                            param.Add("@P_CompCode", CompCode);
                            param.Add("@P_ShareType", ShareType);
                            param.Add("@P_DivType", DivType);



                            HolderRepsonse.HolderBOIDs = connection.Query<HolderBOID>("Get_Dividend_List_HOLDERBOID", param, commandType: CommandType.StoredProcedure)?.ToList();
                            if (HolderRepsonse.HolderBOIDs.Count > 0)
                            {
                                param.Add("@P_HolderNo", HolderNo);
                                param.Add("@P_UserName", UserName);
                                foreach (HolderBOID holder in HolderRepsonse.HolderBOIDs)
                                {
                                    param.Add("@P_TableName", holder.tablename);
                                    holder.holderBOIDInfo = connection.Query<HolderBOIDTable>("GET_ALL_HOLDER_INFO", param, commandType: CommandType.StoredProcedure)?.ToList();
                                }
                                jsonResponse.ResponseData = HolderRepsonse;
                                jsonResponse.IsSuccess = true;
                            }
                            else
                            {
                                jsonResponse.Message = "No Records Found !!!";
                            }
                        }
                        else
                        {
                            jsonResponse.Message = "No Such Holder Found !!!";
                        }

                    }
                    #endregion

                }
                catch (Exception ex)
                {
                    jsonResponse.Message = ex.Message;
                }
                return jsonResponse;
            }
        }
    }
}
