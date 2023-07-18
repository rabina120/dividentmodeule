using Dapper;
using Entity.CDS;
using Entity.Common;
using Interface.CDS;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.CDS
{
    public class ReversalPostingRepo : IReversalPosting
    {
        IOptions<ReadConfig> _connectionString;

        public ReversalPostingRepo(IOptions<ReadConfig> connectionString)
        {
            this._connectionString = connectionString;
        }


        public JsonResponse GetDataForPosting(string CompCode, string FromDate, string ToDate, string UserName, string IP)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                connection.Open();
                try
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@P_COMPCODE", CompCode);
                    parameters.Add("@P_FromDate", FromDate);
                    parameters.Add("@P_ToDate", ToDate);
                    parameters.Add("@P_USERNAME", IP);
                    parameters.Add("@P_IP_ADDRESS", UserName);
                    parameters.Add("@P_DATE_NOW", DateTime.Now);

                    List<ATTReversalCertificate> revCertificates = connection.Query<ATTReversalCertificate>("GET_DATA_FOR_REVERSAL_POSTING", parameters, null, commandType: CommandType.StoredProcedure)?.ToList();
                    if (revCertificates.Count > 0)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = revCertificates;
                    }
                    else
                    {
                        response.Message = "Cannot Find Any Certificates !!!";
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

        public JsonResponse PostData(string Compcode, string SelectedAction, string Remarks, string PostingDate, List<ATTReversalCertificate> ReversalCertificates, string UserName, string IP)
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

                        dt.Columns.Add("rev_tran_no");
                        dt.Columns.Add("regno");
                        dt.Columns.Add("drnno");
                        dt.Columns.Add("shholderno");
                        //dt.Columns.Add("shholderno");
                        //dt.Columns.Add("pcs_demate_holderno");
                        //dt.Columns.Add("certno");
                        //dt.Columns.Add("SrNoFrom");
                        //dt.Columns.Add("srnoto");
                        //dt.Columns.Add("SHkitta");
                        //dt.Columns.Add("rev_tran_no");

                        ReversalCertificates.ForEach(x => dt.Rows.Add(x.rev_tran_no, x.regno, x.drn_no, x.shholderno));
                        //ReversalCertificates.ForEach(x => dt.Rows.Add(x.shholderno, x.pcs_demate_holderno, x.certno, x.srnofrom, x.srnoto, x.shkitta, x.rev_tran_no));

                        SqlCommand cmd = new SqlCommand("POST_REVERSAL_CERTIFICATE", connection);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Transaction = tran;
                        SqlParameter param = cmd.Parameters.AddWithValue("@udt", dt);
                        param = cmd.Parameters.AddWithValue("@P_COMPCODE", Compcode);
                        param = cmd.Parameters.AddWithValue("@P_REMARKS", Remarks);
                        param = cmd.Parameters.AddWithValue("@P_USERNAME", UserName);
                        param = cmd.Parameters.AddWithValue("@P_SELECTED_ACTION", SelectedAction);
                        param = cmd.Parameters.AddWithValue("@P_POSTINGDATE", PostingDate);
                        param = cmd.Parameters.AddWithValue("@P_IP_ADDRESS", IP);
                        param = cmd.Parameters.AddWithValue("@P_DATE_NOW", DateTime.Now);
                        param.Direction = ParameterDirection.Input;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (reader.GetString(0) == "1")
                                {
                                    response.IsSuccess = true;
                                    string msg = SelectedAction == "A" ? "Approved" : "Rejected";
                                    response.Message = "Certificates " + msg + " Successfully !!!";
                                }
                                else
                                {
                                    string msg = SelectedAction == "A" ? "Approve" : "Reject";
                                    response.Message = "Failed To " + msg + "Records !!!";
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
                }
            }
            return response;
        }

        public JsonResponse ViewSingleRematerializeDetail(string CompCode, string RevTranNo, string RegNo, string DrnNo, string UserName, string IP)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                connection.Open();
                try
                {
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@P_COMPCODE", CompCode);
                    parameters.Add("@P_REVTRANNO", RevTranNo);
                    parameters.Add("@P_REGNO", RegNo);
                    parameters.Add("@P_DRNNO", DrnNo);
                    parameters.Add("@P_USERNAME", IP);
                    parameters.Add("@P_IP_ADDRESS", UserName);
                    parameters.Add("@P_DATE_NOW", DateTime.Now);

                    List<ATTReversalCertificate> revCertificates = connection.Query<ATTReversalCertificate>("GET_SINGLE_DATA_FOR_REVERSAL_POSTING", parameters, null, commandType: CommandType.StoredProcedure)?.ToList();
                    if (revCertificates.Count > 0)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = revCertificates;
                    }
                    else
                    {
                        response.Message = "Cannot Find Any Certificates !!!";
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
    }
}
