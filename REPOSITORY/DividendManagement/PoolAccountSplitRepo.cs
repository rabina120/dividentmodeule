using Dapper;
using Entity.Common;
using Entity.DemateDividend;
using ENTITY.DemateDividend;
using INTERFACE.DividendManagement;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace REPOSITORY.DividendManagement
{
    public class PoolAccountSplitRepo : IPoolAccountSplit
    {
        IOptions<ReadConfig> _connectionString;

        public PoolAccountSplitRepo(IOptions<ReadConfig> connectionString)
        {
            this._connectionString = connectionString;
        }

        public JsonResponse GetHolderInfoForSplit(string BOID, string UserName, string CompCode, string IPAddress, string DivCode, string Action)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                connection.Open();
                try
                {
                    var SP = "dbo.GET_DATA_FOR_DIVIDEND_SPLIT";
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_CompCode", CompCode);
                    param.Add("@P_BOID", BOID);
                    param.Add("@p_divcode", DivCode);
                    param.Add("@P_UserName", UserName);
                    param.Add("@P_IPADdress", IPAddress);
                    param.Add("@p_Entry_Date", DateTime.Now);
                    param.Add("@P_SelectedAction", Action);
                    ATTDemateDividend resp = connection.Query<ATTDemateDividend>(SP, param, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                    if (resp != null)
                    {


                        if (resp.isSplit)
                        {
                            response.Message = "Boid already listed for split!!";
                            response.IsSuccess = false;
                        }
                        else
                        {
                            if (resp.fullname != null)
                            {
                                response.ResponseData = resp;
                                response.IsSuccess = true;
                            }
                            else
                            {
                                response.IsSuccess = false;
                                response.Message = "Data not found!!!!!";
                            }
                        }
                    }
                    else
                    {
                        resp = new ATTDemateDividend();
                        response.IsSuccess = false;
                        response.Message = "Data not found!!!!!";

                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.Message = ex.Message;
                }
                finally
                {
                    connection.Close();
                }
            }
            return response;
        }

        public JsonResponse GetCompleteDataFromExcel(List<ATTPoolAccountList> poolentries, string UserName, string CompCode, string IPAddress)
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
                        dt.Columns.Add("Boid");
                        dt.Columns.Add("name");
                        dt.Columns.Add("fathername");
                        dt.Columns.Add("grandfathername");
                        dt.Columns.Add("totalkitta");
                        dt.Columns.Add("fraction");
                        dt.Columns.Add("actualbonus");
                        dt.Columns.Add("remfrac");
                        dt.Columns.Add("divamount");
                        dt.Columns.Add("divtax");
                        dt.Columns.Add("bonustax");
                        poolentries.ForEach(x => dt.Rows.Add(x.Boid, x.name, x.fathername, x.grandfathername, x.totalkitta, x.fraction, x.actualbonus, x.remfrac, x.divamount, x.bonustax, x.bonustax));
                        SqlCommand cmd = new SqlCommand("procedure", connection);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Transaction = trans;

                        SqlParameter param = cmd.Parameters.AddWithValue("@udt", dt);
                        param = cmd.Parameters.AddWithValue("@P_EntryDate", DateTime.Now);
                        param = cmd.Parameters.AddWithValue("@P_UserName", UserName);
                        param = cmd.Parameters.AddWithValue("@P_CompCode", CompCode);
                        param = cmd.Parameters.AddWithValue("@P_IPAddress", IPAddress);

                        param.Direction = ParameterDirection.Input;
                        List<ATTPoolAccountList> poolBulkEntries = new List<ATTPoolAccountList>();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            var parser = reader.GetRowParser<ATTPoolAccountList>(typeof(ATTPoolAccountList));

                            while (reader.Read())
                            {
                                ATTPoolAccountList poolaccounts = parser(reader);
                                poolBulkEntries.Add(poolaccounts);
                            }
                        }
                        if (poolBulkEntries.Count > 0)
                        {
                            if (poolentries.Count != poolBulkEntries.Count)
                            {
                                jsonResponse.Message = ATTMessages.CERTIFICATE.NOT_IN_CERT_BONUS_ISSUE + "\n Or ShOwnerType Data Is Not Correct. \n Please Check The Excel Data .";
                            }
                            else
                            {
                                jsonResponse.IsSuccess = true;
                                jsonResponse.ResponseData = poolBulkEntries;

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

        public JsonResponse UploadPASData(DataTable PasTable, string UserName, string IPAddress, string CompCode, string DivCode, string BOID, string Action)
        {
            JsonResponse jsonResponse = new JsonResponse();


            using (SqlConnection con = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                con.Open();
                using (SqlTransaction trans = con.BeginTransaction())
                {
                    try
                    {
                        DataTable tabletosend = new DataTable();
                            tabletosend.Columns.Add(new DataColumn("boid", typeof(string)));
                            tabletosend.Columns.Add(new DataColumn("name", typeof(string)));
                            tabletosend.Columns.Add(new DataColumn("faname", typeof(string)));
                            tabletosend.Columns.Add(new DataColumn("grfaname", typeof(string)));
                            tabletosend.Columns.Add(new DataColumn("totalkitta", typeof(int)));
                            tabletosend.Columns.Add(new DataColumn("tfrackitta", typeof(float)));
                            tabletosend.Columns.Add(new DataColumn("actualbonus", typeof(float)));
                            tabletosend.Columns.Add(new DataColumn("remfrac", typeof(float)));
                            tabletosend.Columns.Add(new DataColumn("divamount", typeof(float)));
                            tabletosend.Columns.Add(new DataColumn("divtax", typeof(float)));
                            tabletosend.Columns.Add(new DataColumn("bonustax", typeof(float)));

                    
                        if(Action == "A")
                        {
                            foreach (DataRow drow in PasTable.Rows)
                            {
                                //drow["name"] = Regex.Replace(drow["name"].ToString(), @"\s+", "").ToString();
                                //drow["faname"] = Regex.Replace(drow["faname"].ToString(), @"\s+", "").ToString();
                                //drow["grfaname"] = Regex.Replace(drow["grfaname"].ToString(), @"\s+", "").ToString();
                                var row = tabletosend.NewRow();
                                row[0] = drow["boid"].ToString();
                                row[1] = drow["name"].ToString();
                                row[2] = drow["faname"].ToString();
                                row[3] = drow["grfaname"].ToString();
                                row[4] =int.Parse( drow["totalkitta"].ToString());
                                row[5] = float.Parse(drow["tfrackitta"].ToString());
                                row[6] = float.Parse(drow["actualbonus"].ToString());
                                row[7] =float.Parse(drow["remfrac"].ToString());
                                row[8] = float.Parse(drow["divamount"].ToString());
                                row[9] =float.Parse(drow["divtax"].ToString());
                                row[10] =float.Parse(drow["bonustax"].ToString());
                                tabletosend.Rows.Add(row );

                            }
                        }
                        else
                        {
                            tabletosend.Rows.Add("","","","",0,0,0,0,0,0,0);
                        }
                            DynamicParameters param = new DynamicParameters();
                            param.Add("@udt", tabletosend, DbType.Object);
                            param.Add("@P_CompCode", CompCode);
                            param.Add("@P_UserName", UserName);
                            param.Add("@P_DivCode", DivCode);
                            param.Add("@P_BOID", BOID);
                            param.Add("@P_IPAddress", IPAddress);
                            param.Add("@P_Entry_Date", DateTime.Now);
                            param.Add("@P_SelectedAction", Action);
                            jsonResponse = con.Query<JsonResponse>(sql: "SAVE_BOID_FOR_DIVIDEND_SPLIT", param: param, trans, commandType: CommandType.StoredProcedure).FirstOrDefault();
                            trans.Commit();
                            //return jsonResponse;
                        }
                    catch (Exception ex)
                    {
                        jsonResponse.HasError = true;
                        //jsonResponse.ResponseData = ex;
                        jsonResponse.IsSuccess = false;
                        jsonResponse.Message = ex.Message;
                        trans.Rollback();
                    
                    }
                }
            }
            return jsonResponse;
        }
        public JsonResponse GetSplitPostingList(string CompCode, string DivCode, string Username, string IPAddress,int? ParentId)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection con = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    DynamicParameters param = new DynamicParameters();
                    param.Add("P_CompCode", CompCode);
                    param.Add("P_divcode",DivCode);
                    param.Add("P_ID", ParentId);
                    param.Add("P_UserName",Username);
                    param.Add("P_IPADdress", IPAddress);
                    param.Add("p_Entry_Date", DateTime.Now);
                    List<ATTSplit> splitList = con.Query<ATTSplit>("GET_DATA_FOR_DIVIDEND_SPLIT_POSTING", param, commandType: CommandType.StoredProcedure).ToList();
                    if(splitList.Count > 0)
                    {
                        response.ResponseData = splitList;
                        response.IsSuccess = true;
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = "There are no BOIDs for split!!!!!!";
                    }
                }
                catch(Exception ex)
                {
                    response.IsSuccess = false;
                    response.Message = ex.Message;
                }
            }
            return response;
        }

        public JsonResponse SubmitForPostring(List<ATTSplit> postingList, string Username, string IpAddress, string Action, string PostingDate, string Remarks)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection con = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                con.Open();
                using (SqlTransaction trans = con.BeginTransaction())
                {
                    try
                    {
                        DataTable dt = new DataTable();
                        dt.Columns.Add("column1");
                        postingList.ForEach(x => dt.Rows.Add(x.Split_id));
                        DynamicParameters param = new DynamicParameters();
                        param.Add("@P_COMPCODE", postingList[0].Compcode);
                        param.Add("@P_divCODE", postingList[0].Divcode);
                        param.Add("@P_SELECTEDACTION", Action);
                        param.Add("@P_USERNAME", Username);
                        param.Add("@P_IPADDRESS", IpAddress);
                        param.Add("@P_ENTRY_DATE", DateTime.Now);
                        param.Add("@udt", dt,DbType.Object);
                        param.Add("@P_POSTINGDATE", PostingDate);
                        param.Add("@P_POSTING_REMARKS", Remarks);
                        response = con.Query<JsonResponse>("SAVE_BOID_FOR_DIVIDEND_SPLIT_POSTING", param,trans, commandType: CommandType.StoredProcedure).FirstOrDefault();
                        trans.Commit();
                    }
                    catch(Exception ex)
                    {
                        response.Message = ex.Message;
                        response.IsSuccess = false;
                        trans.Rollback();
                    }
                }

            }
            return response;
        }
    }
}
