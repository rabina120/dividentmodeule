


using Dapper;
using Entity.Certificate;
using Entity.Common;
using Interface.Certificate;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.Certificate
{
    public class CertificateSplitPostingRepo : ICertificateSplitPosting
    {
        IOptions<ReadConfig> connectionString;
        public CertificateSplitPostingRepo(IOptions<ReadConfig> connectionString)
        {
            this.connectionString = connectionString;
        }


        public JsonResponse GetCertificateSplitCompanyData(string CompCode)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                //try
                //{
                //    connection.Open();
                //    DynamicParameters param = new DynamicParameters();
                //    param.Add("@P_COMPCODE", CompCode);
                //    param.Add("@TransType", "01");


                //    //List<ATTCERTIFICATE> paraCertificateSplit= connection.Query<ATTCERTIFICATE>("GET_CERTIFICATE_SPLIT_DATA", param, commandType: CommandType.StoredProcedure)?.AsList();
                //    //if (paraCertificateSplit.Count > 0)
                //    //{
                //    //    jsonResponse.IsSuccess = true;
                //    //    jsonResponse.ResponseData = paraCertificateSplit;
                //    //}

                //    //param.Add("@TransType", "01");



                //    List<ATTCERTIFICATE> paraCertificateSplit = SqlMapper.QueryAsync<ATTCERTIFICATE, ATTShHolder, ATTCERTIFICATE>(connection, "GET_CERTIFICATE_SPLIT_DATA",
                //       (certificate, holder) =>
                //       {
                //           certificate.aTTShHolder = holder;
                //           return certificate;
                //       }, param, null, splitOn: "null", commandType: CommandType.StoredProcedure)?.Result.AsList();

                //    if (paraCertificateSplit.Count > 0)
                //    {
                //        jsonResponse.IsSuccess = true;
                //        jsonResponse.ResponseData = paraCertificateSplit;
                //    }
                //    else
                //    {
                //        jsonResponse.Message = "Cannot Find Any Data !!!";
                //    }
                //}
                //catch (Exception ex)
                //{
                //    jsonResponse.IsSuccess = false;
                //    jsonResponse.Message = ex.Message;
                //}
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_COMPCODE", CompCode);
                    param.Add("@TransType", "01");
                    List<ATTCERTIFICATE> certificate = connection.Query<ATTCERTIFICATE>("GET_CERTIFICATE_SPLIT_DATA", param, commandType: CommandType.StoredProcedure)?.ToList();
                    if (certificate.Count > 0)
                    {
                        jsonResponse.ResponseData = certificate;
                        jsonResponse.IsSuccess = true;
                    }



                    else
                    {
                        jsonResponse.Message = ATTMessages.NO_RECORDS_FOUND;
                    }
                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.Message = ex.Message;
                }
            }
            return jsonResponse;
        }

        public JsonResponse PostCertificateSplitEntry(List<ATTCERTIFICATE> certificateDemate, ATTCERTIFICATE recordDetails, string ActionType, string UserName, string IP)
        {

            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {

                //old format
                //try
                //{
                //    connection.Open();
                //    using (trans = connection.BeginTransaction())
                //    {
                //        try
                //        {
                //            foreach (ATTCERTIFICATE certificateSplitDetails in certificateDemate)
                //            {
                //                param = new DynamicParameters();
                //                param.Add("@CompCode", recordDetails.compcode);
                //                param.Add("@Split_No", certificateSplitDetails.split_no);
                //                param.Add("@App_Remarks", recordDetails.App_remarks);
                //                param.Add("@App_Date", recordDetails.App_date);
                //                param.Add("@ActionType", ActionType);
                //                param.Add("@UserName", UserName);
                //                //param.Add("@BHolderNo", certificateSplitDetails.ShholderNo);
                //                param.Add("@CertNo", certificateSplitDetails.CertNo);
                //                param.Add("@SrNoFrom", certificateSplitDetails.SrNoFrom);
                //                param.Add("@SrNoTo", certificateSplitDetails.SrNoTo);
                //                param.Add("@ShKitta", certificateSplitDetails.kitta);
                //                param.Add("@SplitDt", certificateSplitDetails.split_dt);
                //                //param.Add("@TrDate", certificateSplitDetails.split_dt);

                //                connection.Query("CERTIFICATE_SPLIT_DATA_POSTING", param, trans, commandType: CommandType.StoredProcedure);

                //            }

                //            trans.Commit();
                //            jsonResponse.IsSuccess = true;

                //            if (ActionType.Trim() == "A") action = " Authorized";
                //            else if (ActionType.Trim() == "R") action = " Rejected";
                //            else if (ActionType.Trim() == "D") action = " Deleted";
                //            jsonResponse.Message = "Certificate Split Data Have Been " + action;
                //        }
                //        catch (Exception ex)
                //        {
                //            trans.Rollback();
                //            jsonResponse.IsSuccess = false;
                //            jsonResponse.Message = ex.Message;
                //        }
                //    }

                //}
                //catch (Exception ex)
                //{
                //    jsonResponse.IsSuccess = false;
                //    jsonResponse.Message = ex.Message;
                //}

                //new format
                try
                {
                    connection.Open();
                    using (SqlTransaction tran = connection.BeginTransaction())
                    {
                        try
                        {
                            DataTable dt = new DataTable();

                            dt.Columns.Add("split_no");
                            dt.Columns.Add("Shholderno");
                            dt.Columns.Add("Certno");

                            certificateDemate.ForEach(x => dt.Rows.Add(x.split_no, x.ShholderNo,x.CertNo));

                            SqlCommand cmd = new SqlCommand("CERTIFICATE_SPLIT_DATA_POSTING", connection);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Transaction = tran;

                            SqlParameter sqlParam = cmd.Parameters.AddWithValue("@udt", dt);
                            sqlParam = cmd.Parameters.AddWithValue("@CompCode", recordDetails.compcode);
                            sqlParam = cmd.Parameters.AddWithValue("@UserName", UserName);
                            sqlParam = cmd.Parameters.AddWithValue("@ActionType", ActionType);
                            sqlParam = cmd.Parameters.AddWithValue("@App_Remarks", recordDetails.App_remarks);
                            sqlParam = cmd.Parameters.AddWithValue("@P_IP_ADDRESS", IP);
                            sqlParam = cmd.Parameters.AddWithValue("@P_DATE_NOW", DateTime.Now);

                            sqlParam.Direction = ParameterDirection.Input;
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    if (reader.GetString(0) == "1")
                                    {
                                        jsonResponse.IsSuccess = true;
                                        string msg = "";
                                        switch (ActionType)
                                        {
                                            case "A":
                                                msg = "Approved";
                                                break;

                                            case "D":
                                                msg = "Deleted";
                                                break;
                                        }

                                        jsonResponse.Message = "Certificate Split Posted " + msg + " Successfully !!!";
                                    }
                                    else
                                    {
                                        string msg = "";
                                        switch (ActionType)
                                        {
                                            case "A":
                                                msg = "Approved";
                                                break;

                                            case "D":
                                                msg = "Delete";
                                                break;
                                        }
                                        jsonResponse.Message = "Failed To " + msg + " Records !!!";
                                    }

                                }
                            }
                            if (jsonResponse.IsSuccess)
                                tran.Commit();
                            else
                                tran.Rollback();
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            jsonResponse.Message = ex.Message;
                        }
                    }
                }
                catch (Exception ex)
                {
                    jsonResponse.Message = ex.Message;
                }

            }
            return jsonResponse;
        }
    }
}
