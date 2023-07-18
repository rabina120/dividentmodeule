
using Dapper;
using Entity.Common;
using Entity.ShareHolder;
using Interface.ShareHolder;
using Microsoft.Extensions.Options;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.ShareHolder
{
    public class EntryUpdateApplicationRepo : IEntryUpdateApplication
    {
        IOptions<ReadConfig> _connectionString;
        public EntryUpdateApplicationRepo(IOptions<ReadConfig> connectionString)
        {
            _connectionString = connectionString;
        }
        public JsonResponse GetInformationForApplication(string CompCode, string ShHolderNo, string SelectedAction, string UserName, string IP, string ApplicationNo)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_CompCode", CompCode);
                    param.Add("@P_SelectedAction", SelectedAction);
                    param.Add("@P_ShHolderNo", ShHolderNo);

                    param.Add("@P_USERNAME", UserName);
                    param.Add("@P_IP_ADDRESS", IP);
                    param.Add("@P_ENTRY_DATE", DateTime.Now);
                    param.Add("@P_APPNO", ApplicationNo);
                    ATTShHolder shHolder = connection.Query<ATTShHolder>(sql: "GET_SHHOLDER_INFO_APPENTRY", param: param, null, commandType: CommandType.StoredProcedure)?.FirstOrDefault();

                    if (shHolder != null)
                    {
                        if (shHolder.ExistingUpdate)
                        {
                            response.IsSuccess = false;
                            response.Message = "SHHOLDER ALREADY ADDED FOR UPDATE";
                        }
                        else
                        {
                            response.IsSuccess = true;
                            response.ResponseData = shHolder;
                        }
                    }
                    else
                    {
                        response.Message = ATTMessages.SHARE_HOLDER.NOT_FOUND;
                    }
                }
                catch (Exception ex)
                {
                    response.HasError = true;
                    response.IsSuccess = false;
                    response.ResponseData = ex;
                }
                return response;
            }
        }

        public JsonResponse GetInformationFromApplicationNo(string CompCode, string SelectedAction, string UserName, string IP, string ApplicationNo)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_CompCode", CompCode);
                    param.Add("@P_SelectedAction", SelectedAction);
                    param.Add("@P_USERNAME", UserName);
                    param.Add("@P_IP_ADDRESS", IP);
                    param.Add("@P_ENTRY_DATE", DateTime.Now);
                    param.Add("@P_APPNO", ApplicationNo);
                    response.ResponseData = connection.Query<ATTShHolderForUpdate>(sql: "GET_SHHOLDER_INFO_APPENTRY_FROM_APPLICATIONNO", param: param, null, commandType: CommandType.StoredProcedure)?.FirstOrDefault();

                    if (response.ResponseData != null)
                    {
                        response.IsSuccess = true;
                    }
                    else
                    {
                        response.Message = ATTMessages.SHARE_HOLDER.NOT_FOUND;
                    }
                }
                catch (Exception ex)
                {
                    response.HasError = true;
                    response.IsSuccess = false;
                    response.ResponseData = ex;
                }
            }
            return response;
        }

        public JsonResponse SaveApplicationForUpdate(string CompCode, string ShHolderNo, string ApplicationDate, string Action, string SelectedAction, string UserName, string IPAddress, string ApplicationNo = null)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        DynamicParameters param = new DynamicParameters();
                        param.Add("@P_CompCode", CompCode);
                        param.Add("@P_SelectedAction", SelectedAction);
                        param.Add("@P_ShHolderNo", ShHolderNo);
                        param.Add("@P_USERNAME", UserName);
                        param.Add("@P_IP_ADDRESS", IPAddress);
                        param.Add("@P_ENTRY_DATE", DateTime.Now);
                        param.Add("@P_APPNO", ApplicationNo);
                        param.Add("@P_Action", Action);
                        param.Add("@P_Application_Date", ApplicationDate);
                        response = connection.Query<JsonResponse>(sql: "SAVE_SHHOLDER_INFO_APPENTRY", param: param, transaction, commandType: CommandType.StoredProcedure)?.FirstOrDefault();

                        if (response.IsSuccess)
                        {
                            transaction.Commit();
                        }
                        else
                        {
                            transaction.Rollback();
                            response.Message = ATTMessages.CANNOT_SAVE;
                        }
                    }
                }


                catch (Exception ex)
                {
                    response.HasError = true;
                    response.IsSuccess = false;
                    response.ResponseData = ex;
                }
                return response;
            }
        }
    }
}
