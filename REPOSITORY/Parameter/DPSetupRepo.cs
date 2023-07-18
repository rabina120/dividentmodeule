using Dapper;
using Entity.Common;
using Entity.Parameter;
using Interface.Parameter;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.Parameter
{
    public class DPSetupRepo : IDPSetup
    {
        IOptions<ReadConfig> connectionstring;
        public DPSetupRepo(IOptions<ReadConfig> _connectionstring)
        {
            this.connectionstring = _connectionstring;
        }

        public JsonResponse GetDPCode()
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionstring.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    string sqlquery = "GetDPCode";
                    var data = connection.Query<string>(sql: sqlquery, param: null, null, commandType: null)?.FirstOrDefault();
                    jsonResponse.Message = data != null ? data.ToString() : "001";
                    if (jsonResponse.Message != null)
                    {
                        jsonResponse.IsSuccess = true;
                    }
                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.ResponseData = ex;
                    jsonResponse.HasError = true;

                }
            }
            return jsonResponse;
        }

        public JsonResponse GetDPDetails(string DPCode, string UserName, string IPAddress)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionstring.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@DPCode", DPCode);
                    param.Add("@USERNAME", UserName);
                    param.Add("@IPADDRESS", IPAddress);
                    param.Add("@ENTRYDATE", DateTime.Now);
                    ATTDPSetup aTTDPSetup = connection.Query<ATTDPSetup>("GetDPDetail", param, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                    if (aTTDPSetup != null)
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = aTTDPSetup;
                    }
                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.HasError = true;
                    jsonResponse.ResponseData = ex;
                }
            }
            return jsonResponse;
        }

        public JsonResponse LoadDPDetailList(string UserName, string IPAddress)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection con = new SqlConnection(Crypto.Decrypt(connectionstring.Value.DefaultConnection)))
            {
                try
                {
                    con.Open();
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@UserName", UserName);
                    parameters.Add("@IPAddress", IPAddress);
                    parameters.Add("@EntryDate", DateTime.Now);

                    List<ATTDPSetup> aTTDPSetup = con.Query<ATTDPSetup>("LoadDPDetailLis", parameters, commandType: CommandType.StoredProcedure)?.ToList();
                    if (aTTDPSetup != null)
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = aTTDPSetup;
                    }
                    else
                    {
                        jsonResponse.Message = ATTMessages.CANNOT_SAVE;
                    }
                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.HasError = true;
                    jsonResponse.ResponseData = ex;

                }
            }
            return jsonResponse;
        }
        public JsonResponse LoadDPDetailList()
        {
            return LoadDPDetailList(null, null);
        }

        public JsonResponse SaveDPDetails(ATTDPSetup aTTDPSetup, string Actiontype, string UserName, string IPAddress)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionstring.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (SqlTransaction trans = connection.BeginTransaction())
                    {
                        DynamicParameters param = new DynamicParameters();
                        param.Add("@dpcode", aTTDPSetup.DP_CODE);
                        param.Add("@dpName", aTTDPSetup.DP_NAME);
                        param.Add("@cds_dp_id", aTTDPSetup.Dp_Id_cds);
                        param.Add("@ActionType", Actiontype);
                        param.Add("@USERNAME", UserName);
                        param.Add("@IPADDRESS", IPAddress);
                        param.Add("@ENTRYDATE", DateTime.Now);

                        jsonResponse = connection.Query<JsonResponse>("Insert_Update_DP", param, trans, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                        if (jsonResponse.IsSuccess)
                            trans.Commit();
                        else
                        {
                            trans.Rollback();
                            jsonResponse.Message = ATTMessages.CANNOT_SAVE;
                        }

                    }

                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.HasError = true;
                    jsonResponse.ResponseData = ex;
                }
            }
            return jsonResponse;
        }
    }
}
