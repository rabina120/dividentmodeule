

using Dapper;
using Entity.Common;
using Entity.Dividend;
using Interface.Parameter;
using Microsoft.Extensions.Options;
using Repository.Security;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.Parameter
{
    public class DividendParameterPosting : IDividendParameterPosting
    {
        IOptions<ReadConfig> _connectionString;

        public DividendParameterPosting(IOptions<ReadConfig> connectionString)
        {
            this._connectionString = connectionString;
        }

        public JsonResponse GetDividendForApproval(string CompCode)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@CompCode", CompCode);
                    List<ATTDividend> aTTDividends = connection.Query<ATTDividend>("Get_Dividend_For_Approval", param, commandType: CommandType.StoredProcedure).ToList();
                    jsonResponse.IsSuccess = true;
                    jsonResponse.ResponseData = aTTDividends;


                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.HasError = true;
                    jsonResponse.ResponseData = ex;
                }
                return jsonResponse;
            }
        }

        public JsonResponse DividendRequestPosting(List<ATTDividend> aTTDividend, string CompCode, string UserName, string ActionType)
        {
            JsonResponse jsonResponse = new JsonResponse();
            SqlTransaction trans;
            AuditRepo audit = new AuditRepo(_connectionString);
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();


                    using (trans = connection.BeginTransaction())
                    {
                        DataTable dataTable = new DataTable();
                        dataTable.Columns.Add("CompCode", typeof(string));
                        dataTable.Columns.Add("Divcode", typeof(string));
                        dataTable.Columns.Add("UserName", typeof(string));

                        var query = from i in aTTDividend
                                    orderby i.Divcode
                                    select new { CompCode, i.Divcode, UserName };

                        aTTDividend.ForEach(att => dataTable.Rows.Add(CompCode, att.Divcode, UserName));
                        using (SqlCommand cmd = new SqlCommand("DIVIDEND_REQUEST_POSTING"))
                        {
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Connection = connection;
                            cmd.Transaction = trans;
                            cmd.Parameters.AddWithValue("@DivTable", dataTable);
                            cmd.Parameters.AddWithValue("@ActionType", ActionType);

                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    jsonResponse.Message = reader.GetString(0);
                                    jsonResponse.IsSuccess = reader.GetBoolean(1);
                                }
                            }
                        }
                        if (jsonResponse.IsSuccess)
                        {
                            if (ActionType == "A")
                            {
                                foreach (ATTDividend aTT in aTTDividend)
                                {
                                    if (aTT.DivType == 1)
                                    {
                                        connection.Query<int>("Select * into " + aTT.tablename1 + " from divmaster where compcode = '000'", null, trans, commandType: null);
                                        connection.Query<int>("Select * into " + aTT.tablename2 + " from divmasterCDS where compcode = '000'", null, trans, commandType: null);
                                    }
                                    else if (aTT.DivType == 2)
                                    {
                                        connection.Query<int>("select * into " + aTT.tablename1 + " from ShBonus where compcode = '000'", null, trans, commandType: null);
                                        connection.Query<int>("select * into " + aTT.tablename2 + " from ShbonusCDS where compcode = '000'", null, trans, commandType: null);
                                    }

                                    audit.auditSave(UserName, "DIVIDEND Parameter Authorized of Company " + CompCode + " Code:" + aTT.Divcode, "Dividend Posting " + ActionType);

                                }
                            }
                            else if (ActionType == "R")
                            {
                                foreach (ATTDividend aTT in aTTDividend)
                                {
                                    audit.auditSave(UserName, "DIVIDEND Parameter Rejected of Company " + CompCode + " Code:" + aTT.Divcode, "Dividend Posting " + ActionType);

                                }
                            }
                            trans.Commit();
                        }
                        else
                        {
                            jsonResponse.Message = "Failed to Post Dividend Request";
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
