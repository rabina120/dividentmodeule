


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
    public class CertificateSplitReportRepo : ICertificateSplitReport
    {
        IOptions<ReadConfig> connectionstring;
        public CertificateSplitReportRepo(IOptions<ReadConfig> _connectionstring)
        {
            this.connectionstring = _connectionstring;
        }

        public JsonResponse GenerateReport(ATTCERTIFICATEREPORT ReportData, string ExportReportType, string FromSystem, string UserName, string IPAddress)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionstring.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@P_COMPCODE", ReportData.CompCode);
                    parameters.Add("@P_SPLITDATEFROM", ReportData.SplitDateFrom);
                    parameters.Add("@P_SPLITDATETO", ReportData.SplitDateTo);
                    parameters.Add("@P_CERTNOFROM", ReportData.CertNoFrom);
                    parameters.Add("@P_CERTNOTO", ReportData.CertNoTo);
                    parameters.Add("@P_SHHOLDERNOFROM", ReportData.HolderNoFrom);
                    parameters.Add("@P_SHHOLDERNOTO", ReportData.HolderNoTo);
                    parameters.Add("@P_FROMSYSTEM", FromSystem);
                    parameters.Add("@P_USERNAME", UserName);
                    parameters.Add("@P_POSTED", ReportData.DataType);
                    parameters.Add("@P_IP_ADDRESS", IPAddress);
                    parameters.Add("@P_DATE_NOW", DateTime.Now);


                    if (ReportData.DataType == "P")
                    {
                        List<ATTCERTIFICATEREPORT> certificate = connection.Query<ATTCERTIFICATEREPORT>("GET_CERTIFICATE_SPLIT_REPORT", parameters, commandType: CommandType.StoredProcedure)?.ToList();
                        if (certificate.Count > 0)
                        {
                            jsonResponse.ResponseData = certificate;
                            jsonResponse.Message = "Record Found";
                            jsonResponse.IsSuccess = true;

                        }
                        else
                        {

                            jsonResponse.Message = "Record Not found";

                        }


                    }
                    else
                    {
                        List<ATTCERTIFICATEREPORT> certificate = connection.Query<ATTCERTIFICATEREPORT>("GET_CERTIFICATE_SPLIT_REPORT", parameters, commandType: CommandType.StoredProcedure)?.ToList();
                        if (certificate.Count > 0)
                        {
                            jsonResponse.ResponseData = certificate;
                            jsonResponse.Message = "Record Found";
                            jsonResponse.IsSuccess = true;


                        }
                        else
                        {
                            jsonResponse.Message = "Record Not found";

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
