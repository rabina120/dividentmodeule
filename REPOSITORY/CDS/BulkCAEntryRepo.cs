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
using System.Threading.Tasks;

namespace Repository.CDS
{
    public class BulkCAEntryRepo : IBulkCAEntry
    {
        IOptions<ReadConfig> _connectionString;

        public BulkCAEntryRepo(IOptions<ReadConfig> connectionString)
        {
            _connectionString = connectionString;
        }

        public JsonResponse GetCompleteDataFromExcel(List<ATTBulkCAEntry> aTTBulkCAEntries, string UserName, string CompCode, string CertDetailId, string ShOwnerType, string IPAddress)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (SqlTransaction trans = connection.BeginTransaction())
                    {
                        DataTable dt = new DataTable();
                        dt.Columns.Add("shholderno");
                        dt.Columns.Add("certno");
                        dt.Columns.Add("dp_id_cds");
                        dt.Columns.Add("boid");
                        aTTBulkCAEntries.ForEach(x => dt.Rows.Add(x.ShHolderNo, x.CertNo, x.DPID, x.BOID));
                        SqlCommand cmd = new SqlCommand("Bulk_CA_Entry_From_Excel", connection);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Transaction = trans;

                        SqlParameter param = cmd.Parameters.AddWithValue("@udt", dt);
                        param = cmd.Parameters.AddWithValue("@P_EntryDate", DateTime.Now);
                        param = cmd.Parameters.AddWithValue("@P_UserName", UserName);
                        param = cmd.Parameters.AddWithValue("@P_CompCode", CompCode);
                        param = cmd.Parameters.AddWithValue("@P_CertDetailId", CertDetailId);
                        param = cmd.Parameters.AddWithValue("@P_ShOwnerType", ShOwnerType);
                        param = cmd.Parameters.AddWithValue("@P_IPAddress", IPAddress);

                        param.Direction = ParameterDirection.Input;
                        List<ATTBulkCAEntry> responseDataBulkEntry = new List<ATTBulkCAEntry>();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            var parser = reader.GetRowParser<ATTBulkCAEntry>(typeof(ATTBulkCAEntry));

                            while (reader.Read())
                            {
                                ATTBulkCAEntry bulkCAEntry = parser(reader);
                                responseDataBulkEntry.Add(bulkCAEntry);
                            }
                        }
                        if (responseDataBulkEntry.Count > 0)
                        {
                            if (aTTBulkCAEntries.Count != responseDataBulkEntry.Count)
                            {
                                jsonResponse.Message = ATTMessages.CERTIFICATE.NOT_IN_CERT_BONUS_ISSUE + "\n Or ShOwnerType Data Is Not Correct. \n Please Check The Excel Data .";
                            }
                            else
                            {
                                jsonResponse.IsSuccess = true;
                                jsonResponse.ResponseData = responseDataBulkEntry;

                            }
                        }
                        else
                        {
                            jsonResponse.Message = ATTMessages.CERTIFICATE.NOT_IN_CERT_BONUS_ISSUE + "\n Or ShOwnerType Data Is Not Correct. \n Or No Records Found. \n Please Check The Excel Data .";
                        }

                    }

                }
                catch (Exception ex)
                {
                    jsonResponse.ResponseData = ex;
                    jsonResponse.IsSuccess = false;
                    jsonResponse.HasError = true;

                }
            }
            return jsonResponse;
        }

        public JsonResponse SaveBulkCAEntry(List<ATTBulkCAEntry> aTTBulkCAEntries, string CompCode, string TransactionDate, string CertDetail, string UserName, string IPAddress)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (SqlTransaction trans = connection.BeginTransaction())
                    {
                        DataTable dt = new DataTable();
                        dt.Columns.Add("seqno");
                        dt.Columns.Add("shholderno");
                        dt.Columns.Add("certno");
                        dt.Columns.Add("srnofrom");
                        dt.Columns.Add("srnoto");
                        dt.Columns.Add("kitta");
                        dt.Columns.Add("dp_code");
                        dt.Columns.Add("boid");
                        dt.Columns.Add("isin_no");
                        int i = 1;
                        aTTBulkCAEntries.ForEach(x => dt.Rows.Add(i++, x.ShHolderNo, x.CertNo, x.SrNoFrom, x.SrNoTo, x.Kitta, x.DPCode, x.BOID, x.ISIN_NO));
                        SqlCommand cmd = new SqlCommand("Bulk_CA_Entry_SAVE", connection);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Transaction = trans;

                        SqlParameter param = cmd.Parameters.AddWithValue("@udt", dt);
                        param = cmd.Parameters.AddWithValue("@P_EntryDate", DateTime.Now);
                        param = cmd.Parameters.AddWithValue("@P_UserName", UserName);
                        param = cmd.Parameters.AddWithValue("@P_CompCode", CompCode);
                        param = cmd.Parameters.AddWithValue("@P_TransactionDate", TransactionDate);
                        param = cmd.Parameters.AddWithValue("@P_CertDetail", CertDetail);
                        param = cmd.Parameters.AddWithValue("@P_IPAddress", IPAddress);

                        param.Direction = ParameterDirection.Input;
                        List<ATTBulkCAEntry> responseDataBulkEntry = new List<ATTBulkCAEntry>();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (reader.GetString(0) == "1")
                                {
                                    jsonResponse.IsSuccess = true;
                                    jsonResponse.Message = ATTMessages.CERTIFICATE.DEMATE_ENTRY + "\n With Demate Reg No From : " + reader.GetInt32(1) + "\n To : " + reader.GetInt32(2) + "\n With Reg No From : " + reader.GetInt32(3) + "\n To : " + reader.GetInt32(4);
                                }
                                else
                                {
                                    jsonResponse.Message = ATTMessages.CERTIFICATE.DEMATE_ENTRY_FALIED;
                                }

                            }

                        }
                        if (jsonResponse.IsSuccess)
                            trans.Commit();
                        else
                            trans.Rollback();


                    }

                }
                catch (Exception ex)
                {

                    jsonResponse.ResponseData = ex;
                    jsonResponse.IsSuccess = false;
                    jsonResponse.HasError = true;
                }
            }
            return jsonResponse;
        }



        //public JsonResponse GetDetails(string CompanyCode,int? Cret_Id,int? ShOwnerType)
        //{
        //    JsonResponse response = new JsonResponse();
        //    using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
        //    {
        //        try
        //        {
        //            DynamicParameters parma = new DynamicParameters();
        //            parma.Add("@SEARCHVALUE", null);
        //            parma.Add("@CompCode", CompanyCode);
        //            parma.Add("@Cret_Id", Cret_Id);
        //            parma.Add("@ShOwnerType", ShOwnerType);
        //            List<ATTBulkCAEntry> list = connection.Query<ATTBulkCAEntry>("dbo.CDS_MODULE_DETAILS", parma, commandType: System.Data.CommandType.StoredProcedure).ToList();
        //            response.ResponseData = list;
        //            response.IsSuccess = true;
        //        }
        //        catch (Exception ex)
        //        {
        //            response.Message = ex.Message;
        //            response.IsSuccess = false;
        //        }

        //    }
        //    return response;
        //}

        private async Task<List<ATTBulkCAEntry>> GetAllDetails(ATTDataListRequest request, string CompanyCode, int? Cret_Id, int? ShOwnerType)
        {
            try
            {
                using (var connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
                {
                    connection.Open();
                    var parameters = new DynamicParameters();
                    parameters.Add("SearchValue", request.SearchValue.Trim(), DbType.String);
                    parameters.Add("PageNo", request.PageNo, DbType.Int32);
                    parameters.Add("PageSize", request.PageSize, DbType.Int32);
                    parameters.Add("SortColumn", request.SortColumn, DbType.Int32);
                    parameters.Add("SortDirection", request.SortDirection, DbType.String);
                    parameters.Add("CompCode", CompanyCode.Trim(), DbType.String);
                    parameters.Add("Cret_Id", Cret_Id);
                    parameters.Add("ShOwnerType", ShOwnerType);
                    List<ATTBulkCAEntry> batchProcessings = (await connection.QueryAsync<ATTBulkCAEntry>("dbo.CDS_MODULE_DETAILS", parameters, commandType: CommandType.StoredProcedure)).ToList();
                    return batchProcessings;

                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }


        public async Task<ATTDataTableResponse<ATTBulkCAEntry>> GetData(ATTDataTableRequest request, string CompanyCode, int? Cret_Id, int? ShOwnerType)
        {
            var req = new ATTDataListRequest()
            {
                PageNo = request.Start,
                PageSize = request.Length,
                SortColumn = request.Order[0].Column,
                SortColumnName = request.Order[0].ColumnName,
                SortDirection = request.Order[0].Dir,
                SearchValue = request.Search != null ? request.Search.Value.Trim() : ""
            };

            var batchProcessings = await GetAllDetails(req, CompanyCode, Cret_Id, ShOwnerType);
            return new ATTDataTableResponse<ATTBulkCAEntry>()
            {
                Draw = request.Draw,
                RecordsTotal = batchProcessings.Count > 0 ? batchProcessings[0].TotalCount : 0,
                RecordsFiltered = batchProcessings.Count > 0 ? batchProcessings[0].FilteredCount : 0,
                Data = batchProcessings.ToArray(),
            };
        }

        public JsonResponse GenerateReport(string CompanyCode, int? Cret_Id, int? ShOwnerType, string ExportReportType)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("CompCode", CompanyCode.Trim(), DbType.String);
                    param.Add("Cret_Id", Cret_Id);
                    param.Add("ShOwnerType", ShOwnerType);
                    if (ExportReportType == "P")
                    {
                        List<dynamic> reportData = connection.Query<dynamic>("[dbo].[CDS_MODULE_DETAILS_REPORT]", param, null, commandType: CommandType.StoredProcedure).ToList();
                        if (reportData.Count > 0)
                        {
                            jsonResponse.IsSuccess = true;
                            jsonResponse.ResponseData = reportData;
                        }
                        else
                        {
                            jsonResponse.Message = ATTMessages.NO_RECORDS_FOUND;
                        }
                    }
                    else
                    {
                        SqlCommand cmd = new SqlCommand("[dbo].[CDS_MODULE_DETAILS_REPORT]", connection);
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter excelParam = new SqlParameter();
                        excelParam = cmd.Parameters.AddWithValue("@CompCode", CompanyCode);
                        excelParam = cmd.Parameters.AddWithValue("@Cret_Id", Cret_Id);
                        excelParam = cmd.Parameters.AddWithValue("@ShOwnerType", ShOwnerType);
                        excelParam.Direction = ParameterDirection.Input;
                        DataSet ds = new DataSet("Data");
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = cmd;

                        da.Fill(ds);

                        if (ds.Tables[0].Rows.Count == 0)
                        {
                            jsonResponse.IsSuccess = false;
                            jsonResponse.Message = ATTMessages.NO_RECORDS_FOUND;
                        }
                        else
                        {
                            jsonResponse.IsSuccess = true;
                            jsonResponse.ResponseData = ds;
                        }
                    }
                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.ResponseData = ex;
                    jsonResponse.HasError = true;
                }

            }
            return jsonResponse;
        }

    }
}
