
using Dapper;
using Entity.Common;
using Entity.ShareHolder;
using Interface.ShareHolder;
using Microsoft.Extensions.Options;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.ShareHolder
{
    public class UpdateShHolderRepo : IUpdateShHolder
    {
        IOptions<ReadConfig> _connectionString;
        public UpdateShHolderRepo(IOptions<ReadConfig> connectionString)
        {
            _connectionString = connectionString;
        }
        public JsonResponse GetApplicationInformation(string CompCode, string ApplicationNo, string UserName, string IPAddress)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_CompCode", CompCode);
                    param.Add("@P_ApplicationNo", ApplicationNo);
                    param.Add("@P_USERNAME", UserName);
                    param.Add("@P_IP_ADDRESS", IPAddress);
                    param.Add("@P_ENTRY_DATE", DateTime.Now);
                    string shHolder = connection.Query<string>(sql: "GET_SHHOLDERNO_FROM_APPLICATIONNO", param: param, null, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                    if (shHolder != null)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = shHolder;
                    }
                    else
                    {
                        response.Message = ATTMessages.NO_RECORDS_FOUND;
                    }
                }
                catch (Exception ex)
                {
                    response.HasError = true;
                    response.IsSuccess = false;
                    response.ResponseData = ex;
                }
                return response;
            }
        }
        public JsonResponse SaveApplicationUpdate(ATTShHolder shholder, string ApplicaitonNo, string UserName, string IPAddress)
        {
            JsonResponse response = new JsonResponse();

            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (SqlTransaction tran = connection.BeginTransaction())
                    {
                        DynamicParameters param = new DynamicParameters();
                        param.Add("P_COMPCODE", shholder.compcode);
                        param.Add("P_ACCOUNTNO", shholder.AccountNo);
                        param.Add("P_ADDRESS", shholder.Address);
                        param.Add("P_ADDRESS1", shholder.Address1 ?? shholder.Address1);
                        param.Add("P_ADDRESS2", shholder.Address2 ?? shholder.Address2);
                        param.Add("P_APPSTATUS", shholder.AppStatus ?? shholder.AppStatus);
                        param.Add("P_APPROVED", shholder.Approved ?? shholder.Approved);
                        param.Add("P_BANKNAME", shholder.BankName ?? shholder.BankName);
                        param.Add("P_CITIZENSHIPNO", shholder.CitizenshipNo ?? shholder.CitizenshipNo);
                        param.Add("P_DISTCODE", shholder.DistCode ?? shholder.DistCode);
                        param.Add("P_EMAILADD", shholder.EmailAdd ?? shholder.EmailAdd);
                        param.Add("P_ENTRYDATE", DateTime.Now);
                        param.Add("P_FANAME", shholder.FaName ?? shholder.FaName);
                        param.Add("P_FNAME", shholder.FName ?? shholder.FName);
                        param.Add("P_GRFANAME", shholder.GrFaName ?? shholder.GrFaName);
                        param.Add("P_HOLDERLOCK", "N");
                        param.Add("P_HUSBANDNAME", shholder.HusbandName ?? shholder.HusbandName);
                        param.Add("P_LNAME", shholder.LName ?? shholder.LName);
                        param.Add("P_MINOR", shholder.Minor);
                        param.Add("P_MOBILENO", shholder.MobileNo ?? shholder.MobileNo);
                        param.Add("P_NOMINEENAME", shholder.NomineeName ?? shholder.NomineeName);
                        param.Add("P_NPADD", shholder.NpAdd ?? shholder.NpAdd);
                        param.Add("P_NPNAME", shholder.NpName ?? shholder.NpName);
                        param.Add("P_NPTITLE", shholder.NpTitle ?? shholder.NpTitle);
                        param.Add("P_OCCUPATION", shholder.Occupation);
                        param.Add("P_PBOXNO", shholder.PboxNo ?? shholder.PboxNo);
                        param.Add("P_REFAPPNO", shholder.refAppNo);
                        param.Add("P_GRELATION", shholder.Relation ?? shholder.Relation);
                        param.Add("P_REMARKS", shholder.Remarks ?? shholder.Remarks);
                        param.Add("P_SHHOLDERNO", shholder.ShholderNo);
                        param.Add("P_SHOWNERTYPE", shholder.shownertype ?? shholder.shownertype);
                        param.Add("P_SHOWNERSUBTYPE", shholder.ShownerSubtype ?? shholder.ShownerSubtype);
                        param.Add("P_TELNO", shholder.TelNo ?? shholder.TelNo);
                        param.Add("P_TFRACKITTA", shholder.tfrackitta);
                        param.Add("P_TITLE", shholder.Title ?? shholder.Title);
                        param.Add("P_TOTALKITTA", shholder.TotalKitta);
                        param.Add("P_USERNAME", shholder.UserName ?? shholder.UserName);

                        param.Add("@P_USERNAME", UserName);
                        param.Add("@P_IP_ADDRESS", IPAddress);
                        param.Add("@P_DATE_NOW", DateTime.Now);
                        param.Add("@P_APPLICATIONNO", ApplicaitonNo);
                        //MInort Params
                        if (shholder.Minor)
                        {
                            if (shholder.aTTMinor != null)
                            {
                                DateTime now = DateTime.Today;
                                DateTime birthday = Convert.ToDateTime(shholder.aTTMinor.DOB);

                                int age = now.Year - birthday.Year;

                                if (now.Month < birthday.Month || (now.Month == birthday.Month && now.Day < birthday.Day))
                                {
                                    age--;
                                }

                                param.Add("P_MDOB", shholder.aTTMinor.DOB);
                                param.Add("P_MAGE", age);
                                param.Add("P_MGENNAME", shholder.aTTMinor.GEnName ?? shholder.aTTMinor.GEnName);
                                param.Add("P_MRELATION", shholder.aTTMinor.Relation ?? shholder.aTTMinor.GEnName);
                            }
                        }
                        response = connection.Query<JsonResponse>("UPDATE_SHHOLDER_FROM_APPLICATION", param, tran, commandType: CommandType.StoredProcedure).FirstOrDefault();
                        if (response.IsSuccess)
                            tran.Commit();
                        else
                        {
                            tran.Rollback();
                            response.Message = ATTMessages.CANNOT_SAVE;
                        }
                    }

                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.HasError = true;
                    response.ResponseData = ex;

                }
                return response;
            }
        }
    }
}
