
using Dapper;
using Entity.Common;
using Entity.Signature;
using Interface.Signature;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.Signature
{
    public class SignatureBatchCaptureRepo : ISignatureBatchCapture
    {
        IOptions<ReadConfig> _connectionString;

        public SignatureBatchCaptureRepo(IOptions<ReadConfig> connectionString)
        {
            this._connectionString = connectionString;
        }

        public JsonResponse ExportBatchSignature(string CompCode, string StartShHolderNo, string EndShHolderNo, string UserName, string ip)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@P_CompCode", CompCode);
                    parameters.Add("@P_StartShHolderNo", StartShHolderNo);
                    parameters.Add("@P_EndShHolderNo", EndShHolderNo);
                    parameters.Add("@P_UserName", UserName);
                    parameters.Add("@P_ENTRY_DATE", DateTime.Now);
                    parameters.Add("@P_IP_ADDRESS", ip);
                    List<ATTShHolderSignature> signatures = connection.Query<ATTShHolderSignature>("GET_SHHOLDER_SIGNATURE", parameters, commandType: CommandType.StoredProcedure)?.ToList();
                    if (signatures.Count > 0)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = signatures;
                    }
                    else
                    {
                        response.Message = "Cannot Find Any Signatures To Export !!!";
                    }
                }
                catch (Exception ex)
                {
                    response.ResponseData = ex;
                    response.IsSuccess = false;
                    response.HasError = true;
                }
            }
            return response;
        }

        public JsonResponse SaveBatchSignature(string CompCode, List<ATTShHolderSignature> Signatures, string UserName, string ip)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (SqlTransaction tran = connection.BeginTransaction())
                    {
                        DataTable dt = new DataTable();

                        dt.Columns.Add("ShHolderNo", typeof(string));
                        dt.Columns.Add("FileLength", typeof(int));
                        dt.Columns.Add("Signature", typeof(byte[]));

                        Signatures.ForEach(x => dt.Rows.Add(x.FileName, x.FileLength, x.signature));
                        SqlCommand cmd = new SqlCommand("POST_BATCH_SIGNATURE", connection);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Transaction = tran;
                        SqlParameter param = cmd.Parameters.AddWithValue("@udt", dt);
                        param = cmd.Parameters.AddWithValue("@P_COMPCODE", CompCode);
                        param = cmd.Parameters.AddWithValue("@P_USERNAME", UserName);
                        param = cmd.Parameters.AddWithValue("@P_IP_ADDRESS", ip);
                        param = cmd.Parameters.AddWithValue("@P_ENTRY_DATE", DateTime.Now);
                        param.Direction = ParameterDirection.Input;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (reader.GetString(0) == "1")
                                {
                                    response.IsSuccess = true;
                                    response.Message = ATTMessages.SAVED_SUCCESS;
                                }
                                else
                                {
                                    response.Message = ATTMessages.CANNOT_SAVE;
                                }

                            }
                        }
                        if (response.IsSuccess)
                            tran.Commit();
                        else
                            tran.Rollback();

                    }
                }
                catch (Exception ex)
                {
                    response.ResponseData = ex;
                    response.IsSuccess = false;
                    response.HasError = true;
                    response.Message = ATTMessages.CANNOT_SAVE;
                }
            }
            return response;
        }


    }
}
