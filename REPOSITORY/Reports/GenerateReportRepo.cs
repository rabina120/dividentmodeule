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
    public class GenerateReportRepo : IGenerateReport
    {
        IOptions<ReadConfig> connectionstring;
        public GenerateReportRepo(IOptions<ReadConfig> _connectionstring)
        {
            this.connectionstring = _connectionstring;
        }

        public JsonResponse GenerateReport(string CompCode, string ReportType, string UserName, string IPAddress)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionstring.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@P_CompCode", CompCode);
                    parameters.Add("@P_Username", UserName);
                    parameters.Add("@P_IP_ADDRESS", IPAddress);
                    parameters.Add("@P_DATE_NOW", DateTime.Now);

                    if (ReportType == "S")
                    {
                        ATTExportSummaryReport aTTSummaryReport = new ATTExportSummaryReport();
                        parameters.Add("@P_Part", 1);
                        List<ATTSummaryReport1> aTTSummaryReport1s = connection.Query<ATTSummaryReport1>(sql: "GET_SUMMARY_REPORT_BY_COMPCODE", parameters, commandType: System.Data.CommandType.StoredProcedure).ToList();
                        for (int i = 0; i < aTTSummaryReport1s.Count; i++)
                        {
                            switch (aTTSummaryReport1s[i].certstatus)
                            {
                                case 1:
                                case 5:
                                    aTTSummaryReport.NormalCertificate += aTTSummaryReport1s[i].tcount;
                                    aTTSummaryReport.NormalKitta += aTTSummaryReport1s[i].tkitta;
                                    break;

                                case 2:
                                case 12:
                                    aTTSummaryReport.PledgeCertificate += aTTSummaryReport1s[i].tcount;
                                    aTTSummaryReport.PledgeKitta += aTTSummaryReport1s[i].tkitta;
                                    break;

                                case 3:
                                case 13:
                                    aTTSummaryReport.SuspendCertificate += aTTSummaryReport1s[i].tcount;
                                    aTTSummaryReport.SuspendKitta += aTTSummaryReport1s[i].tkitta;
                                    break;

                                case 4:
                                case 14:
                                    aTTSummaryReport.LostCertificate += aTTSummaryReport1s[i].tcount;
                                    aTTSummaryReport.LostKitta += aTTSummaryReport1s[i].tkitta;
                                    break;

                                case 6:
                                    aTTSummaryReport.SplittedCertificate += aTTSummaryReport1s[i].tcount;
                                    aTTSummaryReport.SplittedKitta += aTTSummaryReport1s[i].tkitta;
                                    break;

                                case 7:
                                    aTTSummaryReport.ConsolidatedCertificate += aTTSummaryReport1s[i].tcount;
                                    aTTSummaryReport.ConsolidatedKitta += aTTSummaryReport1s[i].tkitta;
                                    break;
                                case 20:
                                    aTTSummaryReport.DemateUnderProcessCertificate += aTTSummaryReport1s[i].tcount;
                                    aTTSummaryReport.DemateUnderProcessKitta += aTTSummaryReport1s[i].tkitta;
                                    break;
                                case 21:
                                    aTTSummaryReport.DemateCertificate += aTTSummaryReport1s[i].tcount;
                                    aTTSummaryReport.DemateKitta += aTTSummaryReport1s[i].tkitta;
                                    break;

                            }
                        }

                        parameters.Add("@P_Part", 2);
                        List<ATTSummaryReport2> aTTSummaryReport2s = connection.Query<ATTSummaryReport2>(sql: "GET_SUMMARY_REPORT_BY_COMPCODE", parameters, commandType: CommandType.StoredProcedure).ToList();
                        for (int i = 0; i < aTTSummaryReport2s.Count; i++)
                        {
                            switch (aTTSummaryReport2s[i].sharetype)
                            {
                                case 1:
                                    aTTSummaryReport.PreferencialCertificate += aTTSummaryReport2s[i].tcount;
                                    aTTSummaryReport.PreferencialKitta += aTTSummaryReport2s[i].tkitta;
                                    break;
                                case 2:
                                    aTTSummaryReport.OrdinaryCertificate += aTTSummaryReport2s[i].tcount;
                                    aTTSummaryReport.OrdinaryKitta += aTTSummaryReport2s[i].tkitta;
                                    break;
                                case 3:
                                    aTTSummaryReport.BonusCertificate += aTTSummaryReport2s[i].tcount;
                                    aTTSummaryReport.BonusKitta += aTTSummaryReport2s[i].tkitta;
                                    break;
                                case 4:
                                    aTTSummaryReport.RightShareCertificate += aTTSummaryReport2s[i].tcount;
                                    aTTSummaryReport.RightShareKitta += aTTSummaryReport2s[i].tkitta;
                                    break;
                            }

                        }

                        parameters.Add("@P_Part", 3);
                        List<ATTSummaryReport3> aTTSummaryReport3s = connection.Query<ATTSummaryReport3>(sql: "GET_SUMMARY_REPORT_BY_COMPCODE", parameters, commandType: CommandType.StoredProcedure).ToList();
                        for (int i = 0; i < aTTSummaryReport3s.Count; i++)
                        {
                            switch (aTTSummaryReport3s[i].shownertype)
                            {
                                case 1:
                                    aTTSummaryReport.PromoterCertificate += aTTSummaryReport3s[i].tcount;
                                    aTTSummaryReport.PromoterKitta += aTTSummaryReport3s[i].tkitta;
                                    break;
                                case 2:
                                    aTTSummaryReport.StaffCertificate += aTTSummaryReport3s[i].tcount;
                                    aTTSummaryReport.StaffKitta += aTTSummaryReport3s[i].tkitta;
                                    break;
                                case 3:
                                    aTTSummaryReport.PublicCertificate += aTTSummaryReport3s[i].tcount;
                                    aTTSummaryReport.PublicKitta += aTTSummaryReport3s[i].tkitta;
                                    break;
                            }

                        }

                        parameters.Add("@P_Part", 4);
                        ATTSummaryReport4 aTTSummaryReport4s = connection.Query<ATTSummaryReport4>(sql: "GET_SUMMARY_REPORT_BY_COMPCODE", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
                        if (aTTSummaryReport4s != null)
                        {
                            aTTSummaryReport.DuplicateCertificate = aTTSummaryReport4s.tcount;
                            aTTSummaryReport.DuplicateKitta = aTTSummaryReport4s.tkitta;
                        }
                        else
                        {
                            aTTSummaryReport.DuplicateCertificate = 0;
                            aTTSummaryReport.DuplicateKitta = 0;
                        }

                        parameters.Add("@P_Part", 5);
                        ATTSummaryReport5 aTTSummaryReport5s = connection.Query<ATTSummaryReport5>(sql: "GET_SUMMARY_REPORT_BY_COMPCODE", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
                        if (aTTSummaryReport5s != null)
                        {
                            aTTSummaryReport.FullyPaidCertificate = aTTSummaryReport5s.tcount;
                            aTTSummaryReport.FullyPaidKitta = aTTSummaryReport5s.tkitta;
                        }
                        else
                        {
                            aTTSummaryReport.FullyPaidCertificate = 0;
                            aTTSummaryReport.FullyPaidKitta = 0;
                        }

                        parameters.Add("@P_Part", 6);
                        ATTSummaryReport6 aTTSummaryReport6s = connection.Query<ATTSummaryReport6>(sql: "GET_SUMMARY_REPORT_BY_COMPCODE", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
                        if (aTTSummaryReport6s != null)
                        {
                            aTTSummaryReport.UnPaidCertificate = aTTSummaryReport6s.tcount;
                            aTTSummaryReport.UnPaidKitta = aTTSummaryReport6s.tkitta;
                        }
                        else
                        {
                            aTTSummaryReport.UnPaidCertificate = 0;
                            aTTSummaryReport.UnPaidKitta = 0;
                        }


                        parameters.Add("@P_Part", 7);
                        List<ATTSummaryReport7> aTTSummaryReport7s = connection.Query<ATTSummaryReport7>(sql: "GET_SUMMARY_REPORT_BY_COMPCODE", parameters, commandType: CommandType.StoredProcedure).ToList();
                        for (int i = 0; i < aTTSummaryReport7s.Count; i++)
                        {
                            if (aTTSummaryReport7s[i].transferred)
                            {
                                aTTSummaryReport.TransferedCertificate = aTTSummaryReport7s[i].tcount;
                            }
                            else
                            {
                                aTTSummaryReport.UntransferedCertificate = aTTSummaryReport7s[i].tcount;
                            }
                        }

                        parameters.Add("@P_Part", 8);
                        List<ATTSummaryReport8> aTTSummaryReport8s = connection.Query<ATTSummaryReport8>(sql: "GET_SUMMARY_REPORT_BY_COMPCODE", parameters, commandType: CommandType.StoredProcedure).ToList();

                        for (int i = 0; i < aTTSummaryReport8s.Count; i++)
                        {
                            if (aTTSummaryReport8s[i].minor)
                            {
                                aTTSummaryReport.TotalMinorShareHolder = aTTSummaryReport8s[i].tcount;
                                aTTSummaryReport.MinorKitta = aTTSummaryReport8s[i].tkitta;
                            }
                            else
                            {
                                aTTSummaryReport.TotalShareHolder = aTTSummaryReport8s[i].tcount;
                                aTTSummaryReport.MKitta = aTTSummaryReport8s[i].tkitta;
                            }
                        }
                        parameters.Add("@P_Part", 9);
                        ATTSummaryReport9 aTTSummaryReport9s = connection.Query<ATTSummaryReport9>(sql: "GET_SUMMARY_REPORT_BY_COMPCODE", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
                        if (aTTSummaryReport9s != null)
                        {
                            aTTSummaryReport.TotalFrac = aTTSummaryReport9s.FracKitta;
                        }
                        parameters.Add("@P_Part", 10);
                        List<ATTSummaryReport10> aTTSummaryReport10s = connection.Query<ATTSummaryReport10>(sql: "GET_SUMMARY_REPORT_BY_COMPCODE", parameters, commandType: CommandType.StoredProcedure)?.ToList();
                        if (aTTSummaryReport10s.Count > 0)
                        {
                            for (int i = 0; i < aTTSummaryReport10s.Count; i++)
                            {
                                if (aTTSummaryReport10s[i].distcert)
                                {
                                    aTTSummaryReport.DistCertificate = aTTSummaryReport10s[i].tcount;
                                    aTTSummaryReport.DistCertificateKitta = aTTSummaryReport10s[i].tkitta;
                                }
                                else
                                {
                                    aTTSummaryReport.UnDistCertificate = aTTSummaryReport10s[i].tcount;
                                    aTTSummaryReport.UnDistCertificateKitta = aTTSummaryReport10s[i].tkitta;
                                }
                            }
                        }

                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = aTTSummaryReport;
                    }
                    else
                    {

                        List<ATTKittaWiseReport> returnedReports = connection.Query<ATTKittaWiseReport>(sql: "GET_KITTAWISE_REPORT_BY_COMPCODE", parameters, commandType: CommandType.StoredProcedure).ToList();
                        if (returnedReports.Count > 0)
                        {
                            jsonResponse.IsSuccess = true;
                            jsonResponse.ResponseData = returnedReports;
                        }
                        else
                        {
                            jsonResponse.IsSuccess = false;
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

        public JsonResponse GenerateReportDemateHolderList(string CompCode, string DataUploadDate, string UserName, string IPAddress)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionstring.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@P_CompCode", CompCode.Trim());
                    parameters.Add("@P_DataUploadDate", DataUploadDate.Trim());

                    List<ATTDemateHolderList> aTTDemateHolderLists = connection.Query<ATTDemateHolderList>(sql: "GET_DEMATE_HOLDER_LIST_FOR_REPORT", parameters, commandType: CommandType.StoredProcedure).ToList();
                    if (aTTDemateHolderLists.Count > 0)
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = aTTDemateHolderLists;
                    }
                    else
                    {
                        jsonResponse.Message = "Cannot Find Any Data !!!";
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

        public JsonResponse GenerateReportDemateRemateList(string CompCode, string TransferedDtFrom, string TransferedDtTo, string UserName, string IPAddress)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionstring.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@P_CompCode", CompCode.Trim());
                    parameters.Add("@P_TransferedDtFrom", TransferedDtFrom.Trim());
                    parameters.Add("@P_TransferedDtTo", TransferedDtTo.Trim());
                    parameters.Add("@P_username", UserName);
                    parameters.Add("@P_DATE_NOW", TransferedDtTo.Trim());
                    parameters.Add("@P_IP_ADDRESS", IPAddress);

                    List<dynamic> aTTDemateRemateLists = connection.Query<dynamic>(sql: "GET_DEMATE_REMATE_TRANSFER_LIST_FOR_REPORT", parameters, commandType: CommandType.StoredProcedure).ToList();
                    if (aTTDemateRemateLists.Count > 0)
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = aTTDemateRemateLists;
                    }
                    else
                    {
                        jsonResponse.Message = "Cannot Find Any Data !!!";
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
