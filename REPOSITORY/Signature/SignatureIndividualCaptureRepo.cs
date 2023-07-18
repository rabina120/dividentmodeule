
using Dapper;
using Entity.Common;
using Entity.Signature;
using Interface.Signature;
using Microsoft.Extensions.Options;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Repository.Signature
{
    public class SignatureIndividualCaptureRepo : ISignatureIndividualCapture
    {
        IOptions<ReadConfig> _connectionString;

        public SignatureIndividualCaptureRepo(IOptions<ReadConfig> connectionString)
        {
            this._connectionString = connectionString;
        }

        public JsonResponse GetShHolderInformation(string CompCode, string ShHolderNo, string SelectedAction, string UserName, string IPAddress)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
                {
                    connection.Open();
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@P_CompCode", CompCode);
                    parameters.Add("@P_ShHolderNo", ShHolderNo);
                    parameters.Add("@P_UserName", UserName);
                    parameters.Add("@P_IP_Address", IPAddress);
                    parameters.Add("@P_ENTRY_DATE", DateTime.Now);

                    ATTShHolderSignature shHolderSignature = connection.Query<ATTShHolderSignature>(sql: "GET_SIGNATURE_DETAILS", param: parameters, transaction: null, commandType: System.Data.CommandType.StoredProcedure)?.FirstOrDefault();
                    if (shHolderSignature != null)
                    {
                        if (shHolderSignature.signature != null)
                        {
                            Image image = null;
                            using (MemoryStream stream = new MemoryStream(shHolderSignature.signature))
                            {
                                image = Image.FromStream(stream);
                            }
                            shHolderSignature.ImageType = image.RawFormat.ToString();
                        }
                        shHolderSignature.UserName = UserName;
                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = shHolderSignature;

                    }
                    else
                    {
                        jsonResponse.Message = ATTMessages.SHARE_HOLDER.NOT_FOUND;
                    }
                }
            }
            catch (Exception ex)
            {
                jsonResponse.HasError = true;
                jsonResponse.ResponseData = ex;
                jsonResponse.IsSuccess = false;
            }
            return jsonResponse;
        }

        public JsonResponse SaveSignatureInformation(string CompCode, string ShHolderNo, string ScannedBy, byte[] Signature, int? fileLength, string SelectedAction, string UserName, string ip)
        {
            JsonResponse jsonResponse = new JsonResponse();
            try
            {
                using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
                {
                    connection.Open();

                    try
                    {
                        using (SqlTransaction tran = connection.BeginTransaction())
                        {
                            DynamicParameters parameters = new DynamicParameters();
                            parameters.Add("@P_CompCode", CompCode);
                            parameters.Add("@P_ShHolderNo", ShHolderNo);
                            parameters.Add("@P_ScannedBy", ScannedBy);
                            parameters.Add("@P_Signature", Signature == null ? new byte[0] : Signature);
                            parameters.Add("@P_FileLength", fileLength);
                            parameters.Add("@P_UserName", UserName);
                            parameters.Add("@P_SelectedAction", SelectedAction);
                            parameters.Add("@P_IP_ADDRESS", ip);
                            parameters.Add("@P_ENTRY_DATE", DateTime.Now);

                            jsonResponse = connection.Query<JsonResponse>(sql: "SAVE_SIGNATURE_SHHOLDER", parameters, tran, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                            if (jsonResponse.IsSuccess)
                                tran.Commit();
                            else
                                tran.Rollback();
                        }
                    }

                    catch (Exception ex)
                    {
                        jsonResponse.HasError = true;
                        jsonResponse.ResponseData = ex;
                        jsonResponse.IsSuccess = false;

                    }
                }
            }
            catch (Exception ex)
            {
                jsonResponse.Message = ex.Message;
            }
            return jsonResponse;
        }
    }
}
