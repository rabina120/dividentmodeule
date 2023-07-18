using Dapper;
using Entity.Common;
using Entity.ShareHolder;
using Interface.ShareHolder;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.ShareHolder
{
    public class HolderInformation : IHolderInformation
    {

        IOptions<ReadConfig> connectionString;
        public HolderInformation(IOptions<ReadConfig> _connectionString)
        {
            this.connectionString = _connectionString;
        }


        public JsonResponse GetNewShHolderNo(string compcode)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    DynamicParameters param = new DynamicParameters();
                    param.Add("P_COMPCODE", compcode);
                    string sqlQuery = "SELECT isnull(max(shholderno),0)+1 as ShholderNo from shholder where compcode = '" + compcode + "'";
                    var newShHolderNo = connection.Query<ATTShHolder>(sql: sqlQuery, param: null, null, commandType: null)?.FirstOrDefault();


                    if (newShHolderNo != null)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = newShHolderNo.ShholderNo;
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

        public JsonResponse GetSHholder(string ShHolderNo, string CompCode, string SelectedAction, string UserName, string IP)
        {
            JsonResponse response = new JsonResponse();
            //ATTShHolder shHolderToReturn = new ATTShHolder();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {

                try
                {
                    connection.Open();

                    DynamicParameters param = new DynamicParameters();
                    param.Add("P_SHHOLDERNO", Convert.ToInt32(ShHolderNo));
                    param.Add("P_COMPCODE", CompCode);
                    param.Add("P_SelectedAction", SelectedAction);

                    param.Add("@P_USERNAME", UserName);
                    param.Add("@P_IP_ADDRESS", IP);
                    param.Add("@P_DATE_NOW", DateTime.Now);


                    ATTShHolder shHolderToReturn = SqlMapper.Query<ATTShHolder, ATTMinor, ATTShHolder>(connection, sql: "GET_SHHOLDER_INFORMATIONRK",
                        (shHolder, minor) =>
                        {
                            shHolder.aTTMinor = minor;
                            return shHolder;
                        },
                        param: param, null, splitOn: "SpMinor", commandType: CommandType.StoredProcedure)?.FirstOrDefault();



                    // shHolderToReturn.aTTMinor = (ATTMinor)connection.Query<ATTMinor>(sql: "GET_SHHOLDER_INFORMATION", param: param, null, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                    //shHolderToReturn.aTTMinor.  = Convert.ToDateTime(shHolderToReturn.aTTMinor.DOB).ToString("yyyy-MM-dd");

                    if (shHolderToReturn != null)
                    {
                        if (shHolderToReturn.HolderLock == "Y")
                        {
                            response.Message = ATTMessages.SHARE_HOLDER.LOCK;
                            response.IsValid = true;
                        }
                        else if (shHolderToReturn.Approved == "N" || shHolderToReturn.AppStatus == "UNPOSTED")
                        {
                            response.Message = ATTMessages.SHARE_HOLDER.UNPOSTED;
                            response.IsValid = true;

                        }

                        response.IsSuccess = true;
                        
                            response.ResponseData = shHolderToReturn;
                       

                    }
                    else
                    {
                        response.Message = SelectedAction = "Cannot Find the Holder !!!";
                    }
                }
                catch (Exception ex)
                {
                    response.ResponseData = ex;
                    response.IsSuccess = false;
                    response.HasError = true;
                }
                return response;
            }

        }

        public JsonResponse SaveShHolder(ATTShHolder shholder, byte[] signature, string signFileLength, string filename, string updateRemarks, string selectedAction)
        {
            JsonResponse response = new JsonResponse();

            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (SqlTransaction tran = connection.BeginTransaction())
                    {

                        try
                        {
                            DynamicParameters param = new DynamicParameters();
                            param.Add("P_COMPCODE", shholder.compcode);
                            param.Add("P_ACCOUNTNO", shholder.AccountNo);
                            param.Add("P_ACTIONTYPE", shholder.ActionType);
                            param.Add("P_ADDRESS", shholder.Address);
                            param.Add("P_ADDRESS1", shholder.Address1 ?? shholder.Address1);
                            param.Add("P_ADDRESS2", shholder.Address2 ?? shholder.Address2);
                            param.Add("P_APPREMARKS", shholder.AppRemarks ?? shholder.AppRemarks);
                            param.Add("P_APPSTATUS", shholder.AppStatus ?? shholder.AppStatus);
                            param.Add("P_APPROVED", shholder.Approved ?? shholder.Approved);
                            //param.Add("P_APPROVEDBY", shholder.ApprovedBy ?? shholder.ApprovedBy);
                            //param.Add("P_APPROVEDDATE", shholder.ApprovedDate ?? shholder.ApprovedDate);
                            param.Add("P_BANKNAME", shholder.BankName ?? shholder.BankName);
                            param.Add("P_CITIZENSHIPNO", shholder.CitizenshipNo ?? shholder.CitizenshipNo);
                            param.Add("P_DISTCODE", shholder.DistCode ?? shholder.DistCode);
                            param.Add("P_EMAILADD", shholder.EmailAdd ?? shholder.EmailAdd);
                            param.Add("P_ENTRYDATE", DateTime.Now);
                            param.Add("P_FANAME", shholder.FaName ?? shholder.FaName);
                            param.Add("P_FNAME", shholder.FName ?? shholder.FName);
                            param.Add("P_GRFANAME", shholder.GrFaName ?? shholder.GrFaName);
                            param.Add("P_HOLDERLOCK", shholder.HolderLock == null ? "N" : shholder.AccountNo);
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
                            param.Add("P_SHOWNERTYPE", shholder.ShownerSubtype ?? shholder.ShownerSubtype);
                            param.Add("P_SHOWNERSUBTYPE", shholder.shownertype ?? shholder.shownertype);
                            param.Add("P_TELNO", shholder.TelNo ?? shholder.TelNo);
                            param.Add("P_TFRACKITTA", shholder.tfrackitta);
                            param.Add("P_TITLE", shholder.Title ?? shholder.Title);
                            param.Add("P_TOTALKITTA", shholder.TotalKitta);
                            param.Add("P_USERNAME", shholder.UserName ?? shholder.UserName);
                            param.Add("P_UpdateRemarks", updateRemarks ?? updateRemarks);


                            //MInort Params
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


                            param.Add("@P_SELECTED_ACTION", selectedAction);
                            response = connection.Query<JsonResponse>("CREATE_SHHOLDER", param, tran, commandType: CommandType.StoredProcedure).FirstOrDefault();

                            tran.Commit();

                        }
                        catch (Exception ex)
                        {
                            response.IsSuccess = false;
                            response.Message = ex.Message;
                            tran.Rollback();

                        }


                    }

                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;

                }
                return response;
            }
        }

        public JsonResponse SaveShHolder(ATTShHolder shholder, string updateRemarks, string selectedAction, string UserName, string IP)
        {
            JsonResponse response = new JsonResponse();

            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (SqlTransaction tran = connection.BeginTransaction())
                    {
                        DynamicParameters param = new DynamicParameters();
                        param.Add("P_COMPCODE", shholder.compcode);
                        param.Add("P_ACCOUNTNO", shholder.AccountNo);
                        param.Add("P_ACTIONTYPE", shholder.ActionType);
                        param.Add("P_ADDRESS", shholder.Address);
                        param.Add("P_ADDRESS1", shholder.Address1 ?? shholder.Address1);
                        param.Add("P_ADDRESS2", shholder.Address2 ?? shholder.Address2);
                        param.Add("P_APPREMARKS", shholder.AppRemarks ?? shholder.AppRemarks);
                        param.Add("P_APPSTATUS", shholder.AppStatus ?? shholder.AppStatus);
                        param.Add("P_APPROVED", shholder.Approved ?? shholder.Approved);
                        //param.Add("P_APPROVEDBY", shholder.ApprovedBy ?? shholder.ApprovedBy);
                        //param.Add("P_APPROVEDDATE", shholder.ApprovedDate ?? shholder.ApprovedDate);
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
                        param.Add("P_UpdateRemarks", updateRemarks ?? updateRemarks);

                        param.Add("@P_USERNAME", UserName);
                        param.Add("@P_IP_ADDRESS", IP);
                        param.Add("@P_DATE_NOW", DateTime.Now);
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



                        param.Add("@P_SELECTED_ACTION", selectedAction);
                        response = connection.Query<JsonResponse>("CREATE_SHHOLDER_NO_SIGNATURERK", param, tran, commandType: CommandType.StoredProcedure).FirstOrDefault();
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



        public JsonResponse GetHolderByQuery(string CompCode, string FirstName, string LastName, string FatherName, string GrandFatherName, string UserName, string IP)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    string procedureName = "GET_HOLDER_BY_QUERYRK";
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@P_CompCode", CompCode);
                    parameters.Add("@P_FirstName", FirstName);
                    parameters.Add("@P_LastName", LastName);
                    parameters.Add("@P_FatherName", FatherName);
                    parameters.Add("@P_GrandFatherName", GrandFatherName);
                    parameters.Add("@P_USERNAME", UserName);
                    parameters.Add("@P_IP_ADDRESS", IP);
                    parameters.Add("@P_DATE_NOW", DateTime.Now);

                    List<ATTShHolder> aTTShHolders = connection.Query<ATTShHolder>(sql: procedureName, param: parameters, null, commandType: CommandType.StoredProcedure)?.ToList();

                    if (aTTShHolders.Count > 0)
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = aTTShHolders;
                    }
                    else
                    {
                        jsonResponse.Message = ATTMessages.SHARE_HOLDER.NOT_FOUND;
                    }


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


    }
}
