using Dapper;
using Entity.Common;
using Entity.ShareHolder;
using Interface.Parameter;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.Parameter
{
    public class OccuptionSetupRepo : IOccupationSetup
    {
        IOptions<ReadConfig> connectionstring;

        public OccuptionSetupRepo(IOptions<ReadConfig> _connectionString)
        {
            this.connectionstring = _connectionString;
        }
        public JsonResponse GetOccupationCode()
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionstring.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    string sqlQuery = "GetOccupationCode";
                    var data = connection.Query<int>(sql: sqlQuery, param: null, null, commandType: null)?.FirstOrDefault();
                    jsonResponse.Message = Convert.ToString(data);
                    if (jsonResponse.Message != null)
                    {
                        jsonResponse.IsSuccess = true;
                    }
                }
                catch (Exception ex)
                {
                    jsonResponse.Message = ex.Message;
                }
            }
            return jsonResponse;
        }

        public JsonResponse GetOccupationDetails(string OccupationId, string UserName, string IPAddress)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionstring.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@OccupationId", OccupationId);
                    param.Add("@UserName", UserName);
                    param.Add("@IPAddress", IPAddress);
                    param.Add("@EntryDate", DateTime.Now);
                    ATTOccupation aTTOccupation = connection.Query<ATTOccupation>("GetOccupationDetails", param, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                    if (aTTOccupation != null)
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = aTTOccupation;
                    }
                    else
                    {
                        jsonResponse.IsSuccess = false;
                        jsonResponse.ResponseData = ATTMessages.NO_RECORDS_FOUND;
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

        public JsonResponse LoadOccupationList(string UserName, string IPAddress)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionstring.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@Username", UserName);
                    param.Add("@IPAddress", IPAddress);
                    param.Add("@EntryDate", DateTime.Now);
                    List<ATTOccupation> aTTOccupation = connection.Query<ATTOccupation>("LoadOccupationList ", param, commandType: CommandType.StoredProcedure)?.ToList();
                    if (aTTOccupation != null)
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = aTTOccupation;
                    }
                    else
                    {
                        jsonResponse.IsSuccess = false;
                        jsonResponse.ResponseData = ATTMessages.NO_RECORDS_FOUND;
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

        public JsonResponse SaveOccupationDetails(ATTOccupation aTTOccupation, string ActionType, string UserName, string IPAddress)
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
                        param.Add("@OccupationId", Convert.ToInt32(aTTOccupation.OccupationId));
                        param.Add("@OccupationN", aTTOccupation.OccupationN);
                        param.Add("@Shownertype", aTTOccupation.shownertype);
                        param.Add("@ActionType", ActionType);
                        param.Add("@UserName", UserName);
                        param.Add("@IPAddress", IPAddress);
                        param.Add("@EntryDate", DateTime.Now);

                        jsonResponse = connection.Query<JsonResponse>("INSERT_UPDATE_OCCUPATION", param, trans, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
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
