using Dapper;
using Entity.Common;
using Entity.Reports;
using Interface.Reports;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.Reports
{
    public class ShareHolderReportRepo : IShareHolderReport
    {
        IOptions<ReadConfig> _connectionString;

        public ShareHolderReportRepo(IOptions<ReadConfig> connectionString)
        {
            _connectionString = connectionString;
        }

        public JsonResponse GenerateReport(string UserName, string CompCode, string ExportReportType, string IPAddress, ATTShareHolderReportData ShareHolderReportData)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("USERNAME", UserName);
                    param.Add("IPADDRESS", IPAddress);
                    param.Add("COMPCODE", CompCode);
                    param.Add("REPORTTYPE", ShareHolderReportData.ReportType);
                    param.Add("REPORTDETAILS", ShareHolderReportData.ReportDetails);
                    param.Add("SHAREHOLDERNOFROM", ShareHolderReportData.ShHolderNoFrom);
                    param.Add("SHAREHOLDERNOTO", ShareHolderReportData.ShHolderNoTo);
                    param.Add("SHAREHOLDERNAME", ShareHolderReportData.ShHolderName);
                    param.Add("SHAREHOLDERADDRESS", ShareHolderReportData.Address);
                    param.Add("DISTRICT", ShareHolderReportData.District);
                    param.Add("SHKITTAFROM", ShareHolderReportData.ShKittaFrom);
                    param.Add("SHKITTATO", ShareHolderReportData.ShKittaTo);
                    param.Add("OCCUPATION", ShareHolderReportData.Occupation);
                    param.Add("HOLDERTYPE", ShareHolderReportData.HolderType);
                    param.Add("SHOWNERTYPE", ShareHolderReportData.ShOwnerType);
                    param.Add("SHOWNERSUBTYPE", ShareHolderReportData.ShOwnerSubType);
                    param.Add("ORDERBY", ShareHolderReportData.OrderBy);
                    param.Add("ENTRYDATE", DateTime.Now);

                    if (ExportReportType == "P")
                    {
                        switch (ShareHolderReportData.ReportType)
                        {
                            case "ATL":
                                List<ATTShareHolderReport> reportData = connection.Query<ATTShareHolderReport>("SHAREHOLDERREPORT_GENERATEREPORT", param, null, commandType: CommandType.StoredProcedure).ToList();
                                if (reportData.Count > 0)
                                {
                                    jsonResponse.IsSuccess = true;
                                    jsonResponse.ResponseData = reportData;
                                }
                                else
                                {
                                    jsonResponse.Message = ATTMessages.NO_RECORDS_FOUND;
                                }
                                break;
                            default:
                                List<dynamic> reportData1 = connection.Query<dynamic>("SHAREHOLDERREPORT_GENERATEREPORT", param, null, commandType: CommandType.StoredProcedure).ToList();
                                if (reportData1.Count > 0)
                                {
                                    jsonResponse.IsSuccess = true;
                                    jsonResponse.ResponseData = reportData1;
                                }
                                else
                                {
                                    jsonResponse.Message = ATTMessages.NO_RECORDS_FOUND;
                                }
                                break;
                        }
                    }
                    else
                    {
                        SqlCommand cmd = new SqlCommand("SHAREHOLDERREPORT_GENERATEREPORT", connection);
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter excelParam = new SqlParameter();
                        excelParam = cmd.Parameters.AddWithValue("@USERNAME", UserName);
                        excelParam = cmd.Parameters.AddWithValue("@IPADDRESS", IPAddress);
                        excelParam = cmd.Parameters.AddWithValue("@COMPCODE", CompCode);
                        excelParam = cmd.Parameters.AddWithValue("@REPORTTYPE", ShareHolderReportData.ReportType);
                        excelParam = cmd.Parameters.AddWithValue("@REPORTDETAILS", ShareHolderReportData.ReportDetails);
                        excelParam = cmd.Parameters.AddWithValue("@SHAREHOLDERNOFROM", ShareHolderReportData.ShHolderNoFrom);
                        excelParam = cmd.Parameters.AddWithValue("@SHAREHOLDERNOTO", ShareHolderReportData.ShHolderNoTo);
                        excelParam = cmd.Parameters.AddWithValue("@SHAREHOLDERNAME", ShareHolderReportData.ShHolderName);
                        excelParam = cmd.Parameters.AddWithValue("@SHAREHOLDERADDRESS", ShareHolderReportData.Address);
                        excelParam = cmd.Parameters.AddWithValue("@DISTRICT", ShareHolderReportData.District);
                        excelParam = cmd.Parameters.AddWithValue("@SHKITTAFROM", ShareHolderReportData.ShKittaFrom);
                        excelParam = cmd.Parameters.AddWithValue("@SHKITTATO", ShareHolderReportData.ShKittaTo);
                        excelParam = cmd.Parameters.AddWithValue("@OCCUPATION", ShareHolderReportData.Occupation);
                        excelParam = cmd.Parameters.AddWithValue("@HOLDERTYPE", ShareHolderReportData.HolderType);
                        excelParam = cmd.Parameters.AddWithValue("@SHOWNERTYPE", ShareHolderReportData.ShOwnerType);
                        excelParam = cmd.Parameters.AddWithValue("@SHOWNERSUBTYPE", ShareHolderReportData.ShOwnerSubType);
                        excelParam = cmd.Parameters.AddWithValue("@ORDERBY", ShareHolderReportData.OrderBy);
                        excelParam = cmd.Parameters.AddWithValue("@ENTRYDATE", DateTime.Now);

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

                        //List<dynamic> reportData1 = connection.Query<dynamic>("SHAREHOLDERREPORT_GENERATEREPORT", param, null, commandType: CommandType.StoredProcedure).ToList();
                        //if (reportData1.Count > 0)
                        //{
                        //    jsonResponse.IsSuccess = true;
                        //    jsonResponse.ResponseData = reportData1;
                        //}
                        //else
                        //{
                        //    jsonResponse.Message = ATTMessages.NO_RECORDS_FOUND;
                        //}
                    }


                    //List<dynamic> reportData = connection.Query<dynamic>("SHAREHOLDERREPORT_GENERATEREPORT", param, null, commandType: CommandType.StoredProcedure).ToList();
                    ////jsonResponse.ResponseData = connection.Query<string>("SHAREHOLDERREPORT_GENERATEREPORT", param, null, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    //if (reportData.Count > 0)
                    //{
                    //    jsonResponse.IsSuccess = true;
                    //    jsonResponse.ResponseData = reportData;
                    //}
                    //else
                    //{
                    //    jsonResponse.Message = ATTMessages.NO_RECORDS_FOUND;
                    //}
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

        public JsonResponse ShholderLockUnlock(string CompCode, string DataType, string StatusType ,string DateFrom, string DateTo, string HolderNoFrom, string HolderNoTo, string ReportType, string UserName, string IPAddress)
        {
        JsonResponse jsonResponse=new JsonResponse();


            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param=new DynamicParameters();
                    param.Add("@COMPCODE", CompCode);
                    param.Add("@USERNAME", UserName);
                    param.Add("@IPADDRESS", IPAddress);
                    param.Add("@DataType", DataType);
                    param.Add("@StatusType", StatusType);
                    param.Add("@DateFrom", DateFrom);
                    param.Add("@DateTo", DateTo);
                    param.Add("@HolderNoFrom", HolderNoFrom);
                    param.Add("@HolderNoTo", HolderNoTo);
                    if (ReportType == "P")
                    {
                        List<dynamic> reportData=connection.Query<dynamic>("ShholderLockUnlockReport",param,null,commandType:CommandType.StoredProcedure).ToList();
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
                        SqlCommand cmd = new SqlCommand("ShholderLockUnlockReport", connection);
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter excelParam = new SqlParameter();
                        excelParam = cmd.Parameters.AddWithValue("@COMPCODE", CompCode);
                        excelParam = cmd.Parameters.AddWithValue("@USERNAME", UserName);
                        excelParam = cmd.Parameters.AddWithValue("@IPADDRESS", IPAddress);
                        excelParam = cmd.Parameters.AddWithValue("@DataType", DataType);
                        excelParam = cmd.Parameters.AddWithValue("@StatusType", StatusType);
                        excelParam = cmd.Parameters.AddWithValue("@DateFrom", DateFrom);
                        excelParam = cmd.Parameters.AddWithValue("@DateTo", DateTo);
                        excelParam = cmd.Parameters.AddWithValue("@HolderNoFrom", HolderNoFrom);
                        excelParam = cmd.Parameters.AddWithValue("@HolderNoTo", HolderNoTo);
                        


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
                    jsonResponse.HasError = true;
                    jsonResponse.Message = ex.Message;
                    jsonResponse.ResponseData = ex;
                }
            }
            
                return jsonResponse;
        }
   
    }
}
