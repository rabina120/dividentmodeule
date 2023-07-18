
using Dapper;
using Entity.Common;
using Entity.Dividend;
using Entity.ShareHolder;

using Interface.DividendManagement;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.DividendProcessing
{
    public class HoldersHistory : IHoldersHistory
    {
        IOptions<ReadConfig> _connectionString;
        public HoldersHistory(IOptions<ReadConfig> connectionString)
        {
            this._connectionString = connectionString;
        }

        public JsonResponse GetHolderInformation(string CompCode, string ShareType, string ShHolderNo, string UserName, string IPAddress)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@COMPCODE", CompCode);
                    param.Add("@SHHOLDERNO", ShareType == "P" ? ShHolderNo : null);
                    param.Add("@BOID", ShareType == "P" ? null : ShHolderNo);
                    param.Add("@USERNAME", UserName);
                    param.Add("@IP_ADDRESS", IPAddress);
                    param.Add("@ENTRY_DATE", DateTime.Now);
                    ATTShHolder shHolder = connection.Query<ATTShHolder>(sql: "HOLDERHISTORY_GETHOLDERINFORMATION", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    if (shHolder != null)
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = shHolder;

                    }
                    else
                    {
                        jsonResponse.Message = ATTMessages.NO_RECORDS_FOUND;
                    }
                }
                catch (Exception ex)
                {
                    jsonResponse.Message = ex.Message;
                }
                return jsonResponse;
            }
        }

        public JsonResponse GetHoldersHistory(string CompCode, string DivType, string ShareType, string ShHolderNo, int occupation, string UserName, string IPAddress)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@COMPCODE", CompCode);
                    String HOLDERNO = null;
                    String BOID = null;
                    if (ShareType == "P")
                    {
                        HOLDERNO = ShHolderNo;
                    }
                    else if (ShareType == "D")
                    {
                        BOID = ShHolderNo;
                    }
                    param.Add("@SHHOLDERNO", HOLDERNO);
                    param.Add("@BOID", BOID);
                    param.Add("@USERNAME", UserName);
                    param.Add("@IP_ADDRESS", IPAddress);
                    param.Add("@ENTRY_DATE", DateTime.Now);
                    param.Add("@divtype", DivType);
                    param.Add("@Occupation", occupation);
                    ATTShHolder shHolder = connection.Query<ATTShHolder>(sql: "HOLDERHISTORY_GETHOLDERINFORMATION", param, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    if (shHolder != null)
                    {
                        jsonResponse.IsSuccess = true;

                        DynamicParameters param2 = new DynamicParameters();
                        param2.Add("@COMPCODE", CompCode);
                        param2.Add("@SHHOLDERNO", ShHolderNo);
                        param2.Add("@OCCUPATION", occupation);
                        param2.Add("@USERNAME", UserName);
                        param2.Add("@IP_ADDRESS", IPAddress);
                        param2.Add("@ENTRY_DATE", DateTime.Now);
                        param2.Add("@DIVTYPE", DivType);                    
                        param2.Add("@HOLDERTYPE", ShareType);

                        List<ATTHolderHistory> aTTHolderHistories = connection.Query<ATTHolderHistory>(sql: "GET_HOLDER_BOID_HISTORY", param2, commandType: CommandType.StoredProcedure).ToList();
                        if (aTTHolderHistories.Count > 0)
                        {
                            jsonResponse.IsSuccess = true;
                            Object[] ArrayOfObjects = new Object[] { shHolder, aTTHolderHistories };
                            jsonResponse.ResponseData = ArrayOfObjects;
                        }
                        else
                        {
                            jsonResponse.IsSuccess = false;
                            jsonResponse.Message = ATTMessages.NO_RECORDS_FOUND;
                        }

                    }
                    else
                    {
                        jsonResponse.Message = ATTMessages.SHARE_HOLDER.NOT_FOUND;
                    }


                }
                catch (Exception ex)
                {
                    jsonResponse.Message = ex.Message;
                    jsonResponse.ResponseData = ex;
                    jsonResponse.HasError = true;
                }
                return jsonResponse;
            }
        }
    }
}
