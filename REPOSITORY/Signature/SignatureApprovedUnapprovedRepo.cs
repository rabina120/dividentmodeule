
using Dapper;
using Entity.Common;
using Entity.Signature;
using Interface.Signature;
using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;

namespace Repository.Signature
{
    public class SignatureApprovedUnapprovedRepo : ISignatureApprovedUnapproved
    {
        IOptions<ReadConfig> _connectionString;

        public SignatureApprovedUnapprovedRepo(IOptions<ReadConfig> connectionString)
        {
            this._connectionString = connectionString;
        }

        public JsonResponse GetAllSignatureList(string CompCode, string UserName, string ip)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@P_CompCode", CompCode);
                    parameters.Add("@P_UserName", UserName);
                    parameters.Add("@P_IP_ADDRESS", ip);
                    parameters.Add("@P_ENTRY_DATE", DateTime.Now);
                    List<ATTShHolderSignature> signatures = connection.Query<ATTShHolderSignature>("GET_SHHOLDER_SIGNATURE_UNAPPROVED", parameters, commandType: CommandType.StoredProcedure)?.ToList();
                    if (signatures.Count > 0)
                    {
                        response.IsSuccess = true;
                        signatures[0].UserName = UserName;
                        response.ResponseData = signatures;
                    }
                    else
                    {
                        response.Message = "Cannot Find Any Signatures !!!";
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

        public JsonResponse GetUnApproveHolderDetail(string CompCode, string ShHolderNo, string ip,string username)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@P_CompCode", CompCode);
                    parameters.Add("@P_ShHolderNo", ShHolderNo);
                    parameters.Add("@IPADDRESS", ip);
                    parameters.Add("@USERNAME", username);
                    parameters.Add("@DATENOW", DateTime.Now);
                    ATTShHolderSignature signature = connection.Query<ATTShHolderSignature>("GET_APPROVED_SHHOLDER_DETAIL_FOR_UNAPPROVE", parameters, commandType: CommandType.StoredProcedure)?.FirstOrDefault();

                    if (signature != null)
                    {
                        if (signature.Is_Approved)
                        {
                            Image image = null;
                            using (MemoryStream stream = new MemoryStream(signature.signature))
                            {
                                image = Image.FromStream(stream);
                            }
                            signature.ImageType = image.RawFormat.ToString().ToLower();
                            response.IsSuccess = true;
                            response.ResponseData = signature;
                        }
                        else
                        {
                            response.Message = "Signature Is Already UnApproved !!!";
                        }
                    }
                    else
                    {
                        response.Message = "Signature Not Found or Is !!!";
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

        public JsonResponse GetSingleSignature(string CompCode, string ShHolderNo, string ip)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@P_CompCode", CompCode);
                    parameters.Add("@P_ShHolderNo", ShHolderNo);

                    byte[] signature = connection.Query<byte[]>("GET_SIGNATURE_UNAPPROVED", parameters, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                    if (signature != null)
                    {
                        ATTShHolderSignature signatureToReturn = new ATTShHolderSignature();
                        Image image = null;
                        using (MemoryStream stream = new MemoryStream(signature))
                        {
                            image = Image.FromStream(stream);
                        }
                        signatureToReturn.ImageType = image.RawFormat.ToString().ToLower();
                        signatureToReturn.signature = signature;
                        response.IsSuccess = true;
                        response.ResponseData = signatureToReturn;
                    }
                    else
                    {
                        response.Message = "Cannot Find Any Signature !!!";
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

        public JsonResponse SaveApprove(string CompCode, List<string> ShHolderNos, string ScannedBy, string ApprovedDate, string UserName, string SelectedAction, bool hasPassword, string ip, string Password = null)
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

                        ShHolderNos.ForEach(x => dt.Rows.Add(x));

                        SqlCommand cmd = new SqlCommand("APPROVE_UNAPPROVE_SIGNATURE_POSTING", connection);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Transaction = tran;
                        SqlParameter param = cmd.Parameters.AddWithValue("@udt", dt);
                        param = cmd.Parameters.AddWithValue("@P_COMPCODE", CompCode);
                        param = cmd.Parameters.AddWithValue("@P_USERNAME", UserName);
                        param = cmd.Parameters.AddWithValue("@P_APPROVED_DATE", ApprovedDate);
                        param = cmd.Parameters.AddWithValue("@P_HAS_PASSWORD", hasPassword);
                        param = cmd.Parameters.AddWithValue("@P_PASSWORD", Password);
                        param = cmd.Parameters.AddWithValue("@P_SELECTED_ACTION", SelectedAction);
                        param = cmd.Parameters.AddWithValue("@P_ScannedBy", ScannedBy);
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
                                    response.Message = SelectedAction == "A" ? "Signatures Approved Successfully !!!" : "Signatures Unapproved Successfully !!!";
                                }
                                else
                                {
                                    response.Message = SelectedAction == "A" ? "Failed to Approve Signature !!!" : "Failed to Unapprove Signature !!!";
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
                    response.IsSuccess = false;
                    response.ResponseData = ex;
                    response.HasError = true;

                }
            }
            return response;
        }
        public JsonResponse SaveUnapprove(string CompCode, string ShHolderNo, string UserName, string SelectedAction, string ip)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (SqlTransaction tran = connection.BeginTransaction())
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@P_CompCode", CompCode);
                        parameters.Add("@P_ShHolderNo", ShHolderNo);
                        parameters.Add("@P_UserName", UserName);
                        parameters.Add("@P_IP_ADDRESS", ip);
                        parameters.Add("P_ENTRY_DATE", DateTime.Now);

                        response = connection.Query<JsonResponse>(sql: "UNAPPROVE_SIGNATURE_POSTING", parameters, tran, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                        if (response.IsSuccess)
                            tran.Commit();
                        else
                            tran.Rollback();

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

