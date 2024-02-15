
using Dapper;
using DocumentFormat.OpenXml.Drawing.Charts;
using Entity.Common;
using Entity.Security;
using Interface.Reports;

using Microsoft.Extensions.Options;

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net.NetworkInformation;
using System.Transactions;

namespace Repository.Reports
{
    public class SignatureReportRepo : ISignatureReport
    {
        IOptions<ReadConfig> _connectionString;

        public SignatureReportRepo(IOptions<ReadConfig> connectionString)
        {
            this._connectionString = connectionString;
        }

        public JsonResponse GenerateReport(string CompCode, string UserName, string SelectedAction, string IPAddress, string DateFrom = null, string DateTo = null, string HolderFrom = null, string HolderTo = null)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@COMPCODE", CompCode);
                        parameters.Add("@USERNAME", UserName);
                        parameters.Add("@SELECTEDACTION", SelectedAction);
                        parameters.Add("@IPADDRESS", IPAddress);
                        parameters.Add("@DATEFROM", DateFrom);
                        parameters.Add("@DATETO", DateTo);
                        parameters.Add("@HOLDERFROM", HolderFrom);
                        parameters.Add("@HOLDERTO", HolderTo);
                        List<dynamic> reportData = connection.Query<dynamic>("SIGNATURE_REPORT", parameters, transaction, commandType: CommandType.StoredProcedure).ToList();
                        if (reportData.Count > 0)
                        {
                            response.IsSuccess = true;
                            response.ResponseData = reportData;
                            transaction.Commit();
                        }
                        else
                        {
                            response.Message = ATTMessages.NO_RECORDS_FOUND;
                            transaction.Rollback();
                        }
                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.Message = ex.Message;
                }
            }
            return response;
        }

        public JsonResponse GenerateReportExcel(string CompCode, string UserName, string SelectedAction, string IPAddress, string DateFrom = null, string DateTo = null, string HolderFrom = null, string HolderTo = null)
        { 
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("SIGNATURE_REPORT", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter param = new SqlParameter();
                    connection.Open();

                    param = cmd.Parameters.AddWithValue("@COMPCODE", CompCode);
                    param = cmd.Parameters.AddWithValue("@USERNAME", UserName);
                    param = cmd.Parameters.AddWithValue("@SELECTEDACTION", SelectedAction);
                    param = cmd.Parameters.AddWithValue("@IPADDRESS", IPAddress);
                    param = cmd.Parameters.AddWithValue("@DATEFROM", DateFrom);
                    param = cmd.Parameters.AddWithValue("@DATETO", DateTo);
                    param = cmd.Parameters.AddWithValue("@HOLDERFROM", HolderFrom);
                    param = cmd.Parameters.AddWithValue("@HOLDERTO", HolderTo);
                   

                    param.Direction = ParameterDirection.Input;
                    DataSet ds = new DataSet("Data");
                    SqlDataAdapter da = new SqlDataAdapter();
                    da.SelectCommand = cmd;

                    da.Fill(ds);

                    if (ds.Tables[0].Rows.Count == 0)
                    {
                        response.IsSuccess = false;
                        response.Message = ATTMessages.NO_RECORDS_FOUND;
                    }
                    else
                    {
                        response.IsSuccess = true;
                        response.ResponseData = ds;
                    }
                }
                catch (Exception ex)
                {
                    response.HasError = true;
                    response.Message = ex.Message;
                    response.ResponseData = ex;

                }

            }
            return response;
        }

        //JsonResponse ISignatureReport.GenerateReportExcel(string CompCode, string UserName, string SelectedAction, string IPAddress, string DateFrom, string DateTo, string HolderFrom, string HolderTo)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
