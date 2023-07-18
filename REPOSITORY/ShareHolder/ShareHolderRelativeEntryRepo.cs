
using Dapper;
using Entity.Common;
using Entity.ShareHolder;
using Interface.ShareHolder;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.ShareHolder
{

    public class ShareHolderRelativeEntryRepo : IShareHolderRelativeEntry
    {
        IOptions<ReadConfig> connectionString;
        public ShareHolderRelativeEntryRepo(IOptions<ReadConfig> _connectionString)
        {
            connectionString = _connectionString;
        }

        public JsonResponse GetMaxSN(string CompCode, string SelectedAction, string ShHolderNo, string UserName)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@P_COMPCODE", CompCode);
                    parameters.Add("@P_SHHOLDERNO", ShHolderNo);
                    parameters.Add("@P_SELECTEDACTION", SelectedAction);
                    int? sn = connection.Query<int?>(sql: "GET_MAX_SN", param: parameters, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                    if (sn == null)
                    {
                        sn = 1;
                    }
                    else
                    {
                        sn++;
                    }
                    response.IsSuccess = true;
                    response.ResponseData = sn;
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.ResponseData = ex;
                    response.HasError = true;
                }
            }
            return response;
        }

        public JsonResponse GetRelativeShHolder(string CompCode, string SelectedAction, string ShHolderNo, string UserName, string IP)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@P_COMPCODE", CompCode);
                    parameters.Add("@P_SHHOLDERNO", ShHolderNo);
                    parameters.Add("@P_SELECTEDACTION", SelectedAction);
                    parameters.Add("@P_USERNAME", UserName);
                    parameters.Add("@P_IP_ADDRESS", IP);
                    parameters.Add("@P_DATE_NOW", DateTime.Now);

                    ATTShHolder shHolder = connection.Query<ATTShHolder>(sql: "GET_RELATIVE_SHHOLDER_RELATIVE_ENTRYRK", param: parameters, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                    if (shHolder != null)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = shHolder;
                    }
                    else
                    {
                        response.Message = "Cannot Find ShHolder !!!";
                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.ResponseData = ex;
                    response.HasError = true;
                }
            }
            return response;
        }

        public JsonResponse GetRelativeShHolderForUpdateDelete(string CompCode, string SelectedAction, string ShHolderNo, string UserName, string IP)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@P_COMPCODE", CompCode);
                    parameters.Add("@P_SHHOLDERNO", ShHolderNo);
                    parameters.Add("@P_SELECTEDACTION", SelectedAction);
                    parameters.Add("@username", UserName);
                    parameters.Add("@P_IP_ADDRESS", IP);
                    parameters.Add("@P_DATE_NOW", DateTime.Now);

                    List<ATTShHolderForRelative> shHolder = connection.Query<ATTShHolderForRelative>(sql: "GET_SHHOLDER_RELATIVE_DETAILRK", param: parameters, commandType: System.Data.CommandType.StoredProcedure).ToList();
                    if (shHolder.Count > 0)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = shHolder;
                    }
                    else
                    {
                        response.Message = "Cannot Find ShHolder !!!";
                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.ResponseData = ex;
                    response.HasError = true;
                }
            }
            return response;
        }

        public JsonResponse GetShHolder(string CompCode, string SelectedAction, string ShHolderNo, string UserName, string IP)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@P_COMPCODE", CompCode);
                    parameters.Add("@P_SHHOLDERNO", ShHolderNo);
                    parameters.Add("@P_SELECTEDACTION", SelectedAction);
                    parameters.Add("@P_USERNAME", UserName);
                    parameters.Add("@P_IP_ADDRESS", IP);
                    parameters.Add("@P_DATE_NOW", DateTime.Now);

                    ATTShHolder shHolder = connection.Query<ATTShHolder>(sql: "GET_SHHOLDER_RELATIVE_ENTRYRK", param: parameters, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                    if (shHolder != null)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = shHolder;
                    }
                    else
                    {
                        response.Message = "Cannot Find ShHolder !!!";
                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.ResponseData = ex;
                    response.HasError = true;
                }
            }
            return response;
        }

        public JsonResponse SaveRelativeEntry(string CompCode, int ShHolderNo, string SN, ATTShHolder relativeShholder, string SelectedAction, string UserName, string IP)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (SqlTransaction tran = connection.BeginTransaction())
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@compcode", CompCode);
                        parameters.Add("@shholderno", ShHolderNo);
                        parameters.Add("@rholderno", relativeShholder.ShholderNo);
                        parameters.Add("@sno", SN);
                        parameters.Add("@name", relativeShholder.FName);
                        parameters.Add("@faname", relativeShholder.FaName);
                        parameters.Add("@grfaname", relativeShholder.GrFaName);
                        parameters.Add("@address", relativeShholder.Address);
                        parameters.Add("@telno", relativeShholder.TelNo);
                        parameters.Add("@username", UserName);
                        parameters.Add("@P_IP_ADDRESS", IP);
                        parameters.Add("@P_DATE_NOW", DateTime.Now);
                        parameters.Add("@selectedaction", SelectedAction);

                        response = connection.Query<JsonResponse>(sql: "save_relative", parameters, tran, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();

                        if (response.IsSuccess)
                        {
                            tran.Commit();
                            response.Message = SelectedAction == "A" ? ATTMessages.SHARE_HOLDER_RELATIVE.ADD_SUCCESS : SelectedAction == "U" ? ATTMessages.SHARE_HOLDER_RELATIVE.UPDATE_SUCCESS : ATTMessages.SHARE_HOLDER_RELATIVE.DELETE_SUCCESS;
                        }
                        else
                        {
                            tran.Rollback();
                            response.Message = ATTMessages.CANNOT_SAVE;
                        }

                    }


                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.ResponseData = ex;
                    response.HasError = true;
                }
            }
            return response;
        }
    }
}
