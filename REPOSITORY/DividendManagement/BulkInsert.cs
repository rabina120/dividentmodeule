
using Dapper;
using Entity.Common;
using Entity.Dividend;
using Interface.DividendManagement;

using Microsoft.Extensions.Options;
using Repository.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.DividendManagement
{
    public class BulkInsert : IBulkInsert
    {
        IOptions<ReadConfig> _connectionString;

        public BulkInsert(IOptions<ReadConfig> connectionString)
        {
            this._connectionString = connectionString;
        }
        public JsonResponse GetDividendTableList(string compcode, string ShareType)
        {
            JsonResponse jsonResponse = new JsonResponse();
            string sql = string.Empty;
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@CompCode", compcode);
                    param.Add("@DivType", ShareType);
                    //Physical Bonus
                    if (ShareType == "03")
                    {
                        sql = "select AgmNo, divcode, description, fiscalyr, TABLENAME1 " +
                           "from divtable where(divtype= 2) and(tablename1 is not null) " +
                           "and compcode = '" + compcode + "' and approved='Y' " +
                           "and app_status = 'POSTED' and physical_upload = 'N'  " +
                           "order by divcode";
                    }
                    //Demate Bonus
                    else if (ShareType == "04")
                    {
                        sql = "SELECT AgmNo, divcode, description, fiscalyr, TABLENAME2 " +
                            "FROM DIVTABLE " +
                            "WHERE (divtype= 2) and(tablename2 is not null)  " +
                            "and COMPCODE = '" + compcode + "' and approved = 'Y' " +
                            "and app_status = 'POSTED'  " +
                            "and demate_upload = 'N' order by divcode";
                    }
                    //Physical Dividend
                    else if (ShareType == "01")
                    {
                        sql = "SELECT AgmNo, divcode, description, fiscalyr, TABLENAME1  " +
                            "FROM DIVTABLE " +
                            "WHERE DIVCALCULATED = 1 and(tablename1 is not null)" +
                            " and (DIVTYPE = 1 or DIVTYPE = 3) and COMPCODE = '" + compcode + "'  and approved='Y' " +
                            "and app_status = 'POSTED'  " +
                            "and physical_upload = 'N' order by divcode";
                    }
                    //Demate Dividend
                    else if (ShareType == "02")
                    {
                        sql = "SELECT AgmNo, divcode, description, fiscalyr, TABLENAME2 " +
                            "FROM DIVTABLE WHERE DIVCALCULATED = 1 and(tablename2 is not null) " +
                            "and (DIVTYPE = 1 or DIVTYPE = 3) " +
                            "and COMPCODE = '" + compcode + "'  and approved='Y' " +
                            "and app_status = 'POSTED' " +
                            "and demate_upload = 'N' order by divcode";
                    }
                    List<ATTDividend> aTTDividendTables = connection.Query<ATTDividend>(sql: sql, param, commandType: null).ToList();
                    if (aTTDividendTables.Count > 0)
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = aTTDividendTables;
                    }
                    else
                    {
                        jsonResponse.IsSuccess = false;
                        jsonResponse.Message = ATTMessages.NO_RECORDS_FOUND;
                    }
                }
                catch (Exception ex)
                {
                    jsonResponse.ResponseData = ex;
                    jsonResponse.IsSuccess = false;
                    jsonResponse.HasError = true;
                }
                return jsonResponse;
            }
        }

        public JsonResponse Issue(string compcode, string divcode, string tablename1, string ActionType, int SheetId, DataSet ds, DataTable dt, string UserName, string IPAddress)
        {
            JsonResponse jsonResponse = new JsonResponse();
            var sourcearrayNames = (from DataColumn x
              in dt.Columns.Cast<DataColumn>()
                                    select x.ColumnName).ToArray();


            using (SqlConnection con = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                con.Open();

                DynamicParameters param = new DynamicParameters();
                param.Add("@P_DivCode", divcode);
                param.Add("@P_COMPCODE", compcode);
                param.Add("@TableName1", tablename1);
                string tableName = new TableReporsitory(_connectionString).GetTableName(param);

                if (tableName != null)
                {
                    string sql = "select top (1) * from " + tableName;
                    List<ATTDivMaster> aTTDivMasters = con.Query<ATTDivMaster>(sql, null, commandType: null)?.ToList();
                    //var data = aTTDivMasters[0];
                    if (aTTDivMasters.Count == 0)
                    {

                        string sqlquery = "SELECT Column_Name FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_CATALOG = '" + con.Database + "' AND Table_name = '" + tableName + "'";
                        List<ATTDivMaster> soucecolumn = con.Query<ATTDivMaster>(sqlquery, null, commandType: null)?.ToList();
                        using (SqlTransaction trans = con.BeginTransaction())
                        {
                            using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con, SqlBulkCopyOptions.KeepIdentity, trans))
                            {
                                //Set the database table name.
                                sqlBulkCopy.DestinationTableName = "dbo." + tableName;
                                //[OPTIONAL]: Map the Excel columns with that of the database table
                                //phyusical dividend
                                if (ActionType == "01")
                                {

                                    string colname = string.Empty;
                                    ATTDivMaster emptycolumn = new ATTDivMaster();
                                    emptycolumn.Column_Name = String.Empty;
                                    foreach (DataColumn col in dt.Columns)
                                    {
                                        colname = (soucecolumn.Find(x => x.Column_Name.ToLower() == col.ColumnName.ToLower()) ?? emptycolumn).Column_Name;
                                        if (colname != String.Empty)
                                        {
                                            sqlBulkCopy.ColumnMappings.Add(col.ColumnName.ToLower(), colname);
                                            colname = string.Empty;
                                        }
                                    }
                                    try
                                    {
                                        sqlBulkCopy.WriteToServer(dt);
                                        param = new DynamicParameters();
                                        param.Add("@CompCode", compcode.Substring(0, 3));
                                        param.Add("@UserName", UserName);
                                        param.Add("@P_IP_ADDRESS", IPAddress);
                                        param.Add("@DivCode", divcode);
                                        param.Add("TableName", tableName);
                                        param.Add("@P_ENTRY_DATE", DateTime.Now);
                                        param.Add("@ActionType", ActionType);
                                        con.Execute("UPDATE_TABLE_DIVMASTER_BULK_INSERT", param, trans, commandType: CommandType.StoredProcedure);

                                        trans.Commit();
                                        jsonResponse.IsSuccess = true;
                                        jsonResponse.Message = "Excel file Inserted to the  " + tableName;

                                    }
                                    catch (Exception ex)
                                    {
                                        trans.Rollback();
                                        jsonResponse.IsSuccess = false;
                                        jsonResponse.Message = ex.Message;
                                        jsonResponse.HasError = true;
                                        jsonResponse.ResponseData = ex;
                                    }
                                }
                                //demate dividends
                                else if (ActionType == "02")
                                {
                                    string colname = string.Empty;
                                    ATTDivMaster emptycolumn = new ATTDivMaster();
                                    emptycolumn.Column_Name = String.Empty;

                                    foreach (DataColumn col in dt.Columns)
                                    {
                                        colname = (soucecolumn.Find(x => x.Column_Name.ToLower() == col.ColumnName.ToLower()) ?? emptycolumn).Column_Name;
                                        if (colname != String.Empty)
                                        {
                                            sqlBulkCopy.ColumnMappings.Add(col.ColumnName.ToLower(), colname);
                                            colname = string.Empty;
                                        }
                                    }
                                    try
                                    {
                                        sqlBulkCopy.WriteToServer(dt);
                                        param = new DynamicParameters();
                                        param.Add("@CompCode", compcode.Substring(0, 3));
                                        param.Add("@P_IP_ADDRESS", IPAddress);
                                        param.Add("@UserName", UserName);
                                        param.Add("@DivCode", divcode);
                                        param.Add("TableName", tableName);
                                        param.Add("@P_ENTRY_DATE", DateTime.Now);
                                        param.Add("@ActionType", ActionType);
                                        con.Execute("UPDATE_TABLE_DIVMASTER_BULK_INSERT", param, trans, commandType: CommandType.StoredProcedure);

                                        trans.Commit();
                                        jsonResponse.IsSuccess = true;
                                        jsonResponse.Message = "Excel file Inserted to the  " + tableName;

                                    }
                                    catch (Exception ex)
                                    {
                                        trans.Rollback();
                                        jsonResponse.IsSuccess = false;
                                        jsonResponse.Message = ex.Message;
                                        jsonResponse.HasError = true;
                                        jsonResponse.ResponseData = ex;

                                    }
                                }
                                //physical bonus 
                                else if (ActionType == "03")
                                {
                                    dt.Columns.Add(new DataColumn("shownertype", typeof(int)));
                                    foreach (DataRow drow in dt.Rows)
                                    {
                                        int occupation = int.Parse(drow["occupation"].ToString());
                                        if (occupation == 2)
                                        {
                                            drow["shownertype"] = 1;
                                        }
                                        else
                                        {
                                            drow["shownertype"] = 3;
                                        }
                                    }
                                    dt.Columns.Remove("occupation");
                                    //if ocupation =2 theshownertype =1  else 3
                                    string colname = string.Empty;
                                    ATTDivMaster emptycolumn = new ATTDivMaster();
                                    emptycolumn.Column_Name = String.Empty;
                                    foreach (DataColumn col in dt.Columns)
                                    {
                                        colname = (soucecolumn.Find(x => x.Column_Name.ToLower() == col.ColumnName.ToLower()) ?? emptycolumn).Column_Name;
                                        if (colname != String.Empty)
                                        {
                                            sqlBulkCopy.ColumnMappings.Add(col.ColumnName.ToLower(), colname);
                                            colname = string.Empty;
                                        }
                                    }
                                    try
                                    {
                                        sqlBulkCopy.WriteToServer(dt);
                                        trans.Commit();
                                        using (SqlTransaction secondTransaction = con.BeginTransaction())
                                        {
                                            try
                                            {
                                                param = new DynamicParameters();
                                                param.Add("@CompCode", compcode);
                                                param.Add("@UserName", UserName);
                                                param.Add("@DivCode", divcode);
                                                param.Add("TableName", tableName);
                                                param.Add("@ActionType", ActionType);
                                                param.Add("@P_IP_ADDRESS", IPAddress);
                                                param.Add("@P_ENTRY_DATE", DateTime.Now);
                                                jsonResponse = con.Query<JsonResponse>("UPDATE_TABLE_DIVMASTER_BULK_INSERT", param, secondTransaction, commandType: CommandType.StoredProcedure).FirstOrDefault();
                                                if (jsonResponse.IsSuccess)
                                                {
                                                    secondTransaction.Commit();
                                                    jsonResponse.IsSuccess = true;
                                                    jsonResponse.Message = "Excel file Inserted to the  " + tableName + ".<br/>Please Call For Support";
                                                }
                                                else
                                                {
                                                    secondTransaction.Rollback();
                                                    using (SqlTransaction sqlTransaction = con.BeginTransaction())
                                                    {
                                                        con.Execute("delete from " + tableName, param, sqlTransaction, commandType: CommandType.Text);
                                                        sqlTransaction.Commit();
                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                con.Execute("delete from " + tableName, param, null, commandType: CommandType.Text);
                                                trans.Rollback();
                                                secondTransaction.Rollback();
                                                jsonResponse.IsSuccess = false;
                                                jsonResponse.Message = ex.Message;
                                                jsonResponse.HasError = true;
                                                jsonResponse.ResponseData = ex;
                                            }

                                        }


                                    }
                                    catch (Exception ex)
                                    {
                                        trans.Rollback();
                                        jsonResponse.IsSuccess = false;
                                        jsonResponse.Message = ex.Message;
                                        jsonResponse.HasError = true;
                                        jsonResponse.ResponseData = ex;

                                    }
                                }
                                //demate bonus
                                else if (ActionType == "04")
                                {
                                    dt.Columns.Add(new DataColumn("shownertype", typeof(int)));
                                    foreach (DataRow drow in dt.Rows)
                                    {
                                        int shhownertype = int.Parse(drow["occupation"].ToString());
                                        if (shhownertype == 2)
                                        {
                                            drow["shownertype"] = 1;
                                        }
                                        else
                                        {
                                            drow["shownertype"] = 3;
                                        }
                                    }
                                    string colname = string.Empty;
                                    ATTDivMaster emptycolumn = new ATTDivMaster();
                                    emptycolumn.Column_Name = String.Empty;
                                    foreach (DataColumn col in dt.Columns)
                                    {
                                        colname = (soucecolumn.Find(x => x.Column_Name.ToLower() == col.ColumnName.ToLower()) ?? emptycolumn).Column_Name;
                                        if (colname != String.Empty)
                                        {
                                            sqlBulkCopy.ColumnMappings.Add(col.ColumnName.ToLower(), colname);
                                            colname = string.Empty;
                                        }
                                    }
                                    try
                                    {
                                        sqlBulkCopy.WriteToServer(dt);
                                        trans.Commit();
                                        using (SqlTransaction secondTransaction = con.BeginTransaction())
                                        {
                                            try
                                            {
                                                param = new DynamicParameters();
                                                param.Add("@CompCode", compcode);
                                                param.Add("@UserName", UserName);
                                                param.Add("@DivCode", divcode);
                                                param.Add("TableName", tableName);
                                                param.Add("@ActionType", ActionType);
                                                param.Add("@P_IP_ADDRESS", IPAddress);
                                                param.Add("@P_ENTRY_DATE", DateTime.Now);
                                                jsonResponse = con.Query<JsonResponse>("UPDATE_TABLE_DIVMASTER_BULK_INSERT", param, secondTransaction, commandType: CommandType.StoredProcedure).FirstOrDefault();
                                                if (jsonResponse.IsSuccess)
                                                {
                                                    secondTransaction.Commit();
                                                    jsonResponse.IsSuccess = true;
                                                    jsonResponse.Message = "Excel file Inserted to the  " + tableName + ".<br/>Please Call For Support";
                                                }
                                                else
                                                {
                                                    secondTransaction.Rollback();
                                                    using (SqlTransaction sqlTransaction = con.BeginTransaction())
                                                    {
                                                        con.Execute("delete from " + tableName, param, sqlTransaction, commandType: CommandType.Text);
                                                        sqlTransaction.Commit();
                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                                con.Execute("delete from " + tableName, param, null, commandType: CommandType.Text);
                                                trans.Rollback();
                                                secondTransaction.Rollback();
                                                jsonResponse.IsSuccess = false;
                                                jsonResponse.Message = ex.Message;
                                                jsonResponse.HasError = true;
                                                jsonResponse.ResponseData = ex;
                                            }

                                        }


                                    }
                                    catch (Exception ex)
                                    {
                                        trans.Rollback();
                                        jsonResponse.IsSuccess = false;
                                        jsonResponse.Message = ex.Message;
                                        jsonResponse.HasError = true;
                                        jsonResponse.ResponseData = ex;

                                    }
                                }

                                con.Close();
                            }
                        }
                    }
                    else
                    {
                        jsonResponse.IsSuccess = false;
                        jsonResponse.Message = "Cash Issue have Already been Stored om table " + tableName;
                    }
                }
                else
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.Message = "Failed to Get Cash Dividend table ";

                }
            }


            return jsonResponse;
        }
    }
}
