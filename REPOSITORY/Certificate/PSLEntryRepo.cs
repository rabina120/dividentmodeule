



using Dapper;
using Entity.Certificate;
using Entity.Common;

using Interface.Certificate;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.Certificate
{
    public class PSLEntryRepo : IPSLEntry
    {
        IOptions<ReadConfig> _connectionString;

        public PSLEntryRepo(IOptions<ReadConfig> connectionString)
        {
            _connectionString = connectionString;
        }

        public JsonResponse GetAllPledgeAt()
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    //DynamicParameters param = new DynamicParameters();
                    List<ATTPSLEntry> aTTPaymentCenter = connection.Query<ATTPSLEntry>("GET_ALL_PLEDGE_CENTER", param: null, commandType: null).ToList();

                    if (aTTPaymentCenter.Count > 0)
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = aTTPaymentCenter;
                    }
                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.ResponseData = ex.Message;

                }
                return jsonResponse;
            }
        }

        public JsonResponse GethlderinfoBysearch(string CompCode, string UserName)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_COMPCODE", CompCode);
                    param.Add("@P_USERNAME", UserName);




                    List<ATTPSLEntry> certificate = connection.Query<ATTPSLEntry>("GET_PSL_DETAIL", param, commandType: CommandType.StoredProcedure)?.ToList();
                    if (certificate.Count > 0)
                    {
                        jsonResponse.ResponseData = certificate;
                        jsonResponse.Message = "Record Found";
                        jsonResponse.IsSuccess = true;
                    }
                    else
                    {
                        jsonResponse.Message = "Cannot Find Sholder !!!";

                    }


                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.Message = ex.Message;
                }
            }
            return jsonResponse;
        }

        public JsonResponse GethlderinfoBysearch(string CompCode, string ShholderNo, string SelectedAction)
        {
            throw new NotImplementedException();
        }

        public JsonResponse GetHolderByQuery(string CompCode, string FirstName, string LastName, string FatherName, string GrandFatherName, string UserName, string IP)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    string procedureName = "GET_HOLDER_BY_QUERY_PSL";
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@P_CompCode", CompCode);
                    parameters.Add("@P_FirstName", FirstName);
                    parameters.Add("@P_LastName", LastName);
                    parameters.Add("@P_FatherName", FatherName);
                    parameters.Add("@P_GrandFatherName", GrandFatherName);
                    parameters.Add("@P_USERNAME", UserName);
                    parameters.Add("@P_IP_ADDRESS", IP);
                    parameters.Add("@P_DATE_NOW", DateTime.Now);

                    List<ATTPSLEntry> PSLEntry = connection.Query<ATTPSLEntry>(sql: procedureName, param: parameters, null, commandType: CommandType.StoredProcedure)?.ToList();

                    if (PSLEntry.Count > 0)
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = PSLEntry;
                    }
                    else
                    {
                        jsonResponse.Message = "Cannot Find the Holder.";
                    }


                }
                catch (Exception ex)
                {
                    jsonResponse.Message = ex.Message;
                }
                return jsonResponse;
            }
        }



        public JsonResponse GetShholderDetailsByShHolderNo(string CompCode, int ShholderNo, string SelectedAction, int pslno, string UserName, string IP)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_COMPCODE", CompCode);
                    param.Add("@P_SHHOLDERNO", ShholderNo);
                    param.Add("@P_SELECTEDACTION", SelectedAction);
                    param.Add("@P_PSLNO", pslno);
                    param.Add("@P_USERNAME", UserName);
                    param.Add("@P_IP_ADDRESS", IP);
                    param.Add("@P_DATE_NOW", DateTime.Now);

                    //if (SelectedAction == "A")
                    //{
                    List<ATTPSLEntry> PSLEntry = connection.Query<ATTPSLEntry>("GET_HOLDERINFO_PSL", param, null, commandType: CommandType.StoredProcedure)?.ToList();
                    if (PSLEntry.Count > 0)
                    {
                        jsonResponse.ResponseData = PSLEntry;
                        jsonResponse.Message = "Record Found";
                        jsonResponse.IsSuccess = true;
                    }
                    else
                    {
                        jsonResponse.Message = "Cannot Find Sholder !!!";

                    }
                    //}

                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.Message = ex.Message;
                }
            }
            return jsonResponse;
        }

        public JsonResponse Getstatus(string Trantype)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();

                    param.Add("@P_TRANTYPE", Trantype);



                    List<ATTPSLEntry> PSLEntry = connection.Query<ATTPSLEntry>("GET_PSL_CHARGE", param, null, commandType: CommandType.StoredProcedure)?.ToList();
                    if (PSLEntry.Count > 0)
                    {
                        jsonResponse.ResponseData = PSLEntry;
                        jsonResponse.Message = "Record Found";
                        jsonResponse.IsSuccess = true;
                    }
                    else
                    {
                        jsonResponse.Message = "Record Cannot Found !!!";

                    }
                    //}

                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.Message = ex.Message;
                }
            }
            return jsonResponse;
        }

        public JsonResponse InsertCertnoInfo(List<ATTPSLEntry> aTTPSLEntry)
        {
            JsonResponse response = new JsonResponse();
            List<ATTPSLEntry> LISTOBJ = new List<ATTPSLEntry>();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            DataTable dt = new DataTable();
                            dt.Columns.Add("CertNo");
                            aTTPSLEntry.ForEach(x => dt.Rows.Add(x.CertNo));

                            SqlCommand cmd = new SqlCommand("GET_CERT_DETAIL_PSL", connection);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Transaction = transaction;
                            SqlParameter param = cmd.Parameters.AddWithValue("@udt", dt);


                            using (SqlDataReader sdr = cmd.ExecuteReader())
                            {
                                while (sdr.Read())
                                {
                                    response.IsSuccess = true;
                                    ATTPSLEntry OBJ = new ATTPSLEntry();
                                    OBJ.SrNoFrom = Convert.ToInt32(sdr["SRNOFROM"]);
                                    OBJ.SrNoTo = Convert.ToInt32(sdr["SRNOTO"]);
                                    OBJ.ShholderNo = Convert.ToInt32(sdr["SHHOLDERNO"]);
                                    OBJ.CertStatus = Convert.ToString(sdr["CERTSTATUS"]);
                                    OBJ.CertNo = Convert.ToInt32(sdr["CERTNO"]);
                                    OBJ.ShOwnerType = Convert.ToString(sdr["SHOWNERTYPE"]);
                                    OBJ.ShKitta = Convert.ToInt32(sdr["SHKITTA"]);
                                    // OBJ.SrNoTo = Convert.ToInt32(sdr["SRNOTO"]);
                                    LISTOBJ.Add(OBJ);
                                }
                            }
                            if (response.IsSuccess)
                            {
                                response.ResponseData = LISTOBJ;
                                response.Message = "Record Found";
                                response.IsSuccess = true;
                            }
                            else
                            {
                                response.Message = "Cannot Find Sholder !!!";

                            }

                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            response.Message = ex.Message;

                        }
                    }
                }

                catch (Exception ex)
                {
                    response.Message = ex.Message;
                }
            }
            return response;
        }


        //public JsonResponse SavePslBatchEntry(List<ATTPSLEntry> ReportData, string CompCode, string ShholderNo, string Code, string Remark, string Transdate, string UserName, string SelectedAction, string Pleggeamount, string Status, int pslno)
        //{
        //    JsonResponse jsonResponse = new JsonResponse();
        //    using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
        //    {
        //        try
        //        {
        //            connection.Open();
        //            using (SqlTransaction tran = connection.BeginTransaction())
        //            {
        //                try
        //                {
        //                    DataTable dt = new DataTable();

        //                    dt.Columns.Add("SeqNo");

        //                    dt.Columns.Add("CertNo");

        //                    dt.Columns.Add("ShKitta");

        //                    int i = 1;
        //                    ReportData.ForEach(x => dt.Rows.Add((i++).ToString(), (x.CertNo.ToString()), (x.ShKitta.ToString())));

        //                    SqlCommand cmd = new SqlCommand("SAVE_PSL_BATCH_ENTRY", connection);
        //                    cmd.CommandType = CommandType.StoredProcedure;
        //                    cmd.Transaction = tran;
        //                    SqlParameter param = cmd.Parameters.AddWithValue("@udt", dt);


        //                    //DynamicParameters param = new DynamicParameters();
        //                    param = cmd.Parameters.AddWithValue("@P_COMPCODE", CompCode);
        //                    param = cmd.Parameters.AddWithValue("@P_SHHOLDERNO", ShholderNo);
        //                    param = cmd.Parameters.AddWithValue("@P_CODE", Code);
        //                    param = cmd.Parameters.AddWithValue("@P_REMARKS", Remark);
        //                    param = cmd.Parameters.AddWithValue("@P_TRANDATE", Transdate);
        //                    param = cmd.Parameters.AddWithValue("@P_SELECTEDACTION", SelectedAction);
        //                    param = cmd.Parameters.AddWithValue("@P_USERNAME", UserName);
        //                    param = cmd.Parameters.AddWithValue("@P_PLEDGEAMOUNT", Pleggeamount);
        //                    param = cmd.Parameters.AddWithValue("@P_STATUS", Status);

        //                    param = cmd.Parameters.AddWithValue("@P_PSLNO ", pslno);

        //                    if (SelectedAction == "A")
        //                    {
        //                        if (Status == "p")
        //                        {


        //                            List<ATTPSLEntry> cc = connection.Query<ATTPSLEntry>("SAVE_PSL_BATCH_ENTRY", param, commandType: CommandType.StoredProcedure)?.ToList();
        //                            if (cc.Count > 0)
        //                            {
        //                                jsonResponse.ResponseData = cc;
        //                                jsonResponse.Message = "Record Found";
        //                                jsonResponse.IsSuccess = true;
        //                            }
        //                            else
        //                            {
        //                                jsonResponse.Message = "Cannot Find Sholder !!!";

        //                            }
        //                        }
        //                        //else if (Status == "S")
        //                        //{


        //                        //    List<ATTPSLEntry> PSLEntry = connection.Query<ATTPSLEntry>("SAVE_PSL_BATCH_ENTRY", param, commandType: CommandType.StoredProcedure)?.ToList();
        //                        //    if (PSLEntry.Count > 0)
        //                        //    {
        //                        //        jsonResponse.ResponseData = PSLEntry;
        //                        //        jsonResponse.Message = "Record Found";
        //                        //        jsonResponse.IsSuccess = true;
        //                        //    }
        //                        //    else
        //                        //    {
        //                        //        jsonResponse.Message = "Cannot Find Sholder !!!";

        //                        //    }
        //                        //}
        //                        //else
        //                        //{


        //                        List<ATTPSLEntry> PSLEntry = connection.Query<ATTPSLEntry>("SAVE_PSL_BATCH_ENTRY", param, commandType: CommandType.StoredProcedure)?.ToList();
        //                        if (PSLEntry.Count > 0)
        //                        {
        //                            jsonResponse.ResponseData = PSLEntry;
        //                            jsonResponse.Message = "Record Found";
        //                            jsonResponse.IsSuccess = true;
        //                        }
        //                        else
        //                        {
        //                            jsonResponse.Message = "Cannot Find Sholder !!!";

        //                        }
        //                    }

        //                    else if (SelectedAction == "E")

        //                        if (Status == "p")
        //                        {


        //                            List<ATTPSLEntry> PSLEntry = connection.Query<ATTPSLEntry>("SAVE_PSL_BATCH_ENTRY", param, commandType: CommandType.StoredProcedure)?.ToList();
        //                            if (PSLEntry.Count > 0)
        //                            {
        //                                jsonResponse.ResponseData = PSLEntry;
        //                                jsonResponse.Message = "Record Found";
        //                                jsonResponse.IsSuccess = true;
        //                            }
        //                            else
        //                            {
        //                                jsonResponse.Message = "Cannot Find Sholder !!!";

        //                            }
        //                        }
        //                        else if (Status == "S")
        //                        {


        //                            List<ATTPSLEntry> PSLEntry = connection.Query<ATTPSLEntry>("SAVE_PSL_BATCH_ENTRY", param, commandType: CommandType.StoredProcedure)?.ToList();
        //                            if (PSLEntry.Count > 0)
        //                            {
        //                                jsonResponse.ResponseData = PSLEntry;
        //                                jsonResponse.Message = "Record Found";
        //                                jsonResponse.IsSuccess = true;
        //                            }
        //                            else
        //                            {
        //                                jsonResponse.Message = "Cannot Find Sholder !!!";

        //                            }
        //                        }
        //                        else
        //                        {


        //                            List<ATTPSLEntry> PSLEntry = connection.Query<ATTPSLEntry>("SAVE_PSL_BATCH_ENTRY", param, commandType: CommandType.StoredProcedure)?.ToList();
        //                            if (PSLEntry.Count > 0)
        //                            {
        //                                jsonResponse.ResponseData = PSLEntry;
        //                                jsonResponse.Message = "Record Found";
        //                                jsonResponse.IsSuccess = true;
        //                            }
        //                            else
        //                            {
        //                                jsonResponse.Message = "Cannot Find Sholder !!!";

        //                            }
        //                        }

        //                    using (SqlDataReader reader = cmd.ExecuteReader())
        //                    {
        //                        if (reader.Read())
        //                        {
        //                            if (reader.GetString(0) == "1")
        //                            {
        //                                jsonResponse.IsSuccess = true;
        //                                jsonResponse.Message = "Dakhil Transfer Successfully !!!";
        //                            }
        //                            else
        //                            {
        //                                jsonResponse.Message = "Failed To Dakhil Transfer !!!";
        //                            }

        //                        }
        //                    }
        //                    if (jsonResponse.IsSuccess)
        //                        tran.Commit();
        //                    else
        //                        tran.Rollback();

        //                }
        //                catch (Exception ex)
        //                {
        //                    jsonResponse.IsSuccess = false;
        //                    jsonResponse.Message = ex.Message;
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            jsonResponse.IsSuccess = false;
        //            jsonResponse.Message = ex.Message;
        //        }
        //        return jsonResponse;
        //    }
        //}

        public JsonResponse SavePslBatchEntry(List<ATTPSLEntry> PSLEntry, string CompCode, int ShholderNo, string Code, string Remark, string Transdate, string UserName, string SelectedAction, string Pleggeamount, string Status, int pslno, string IP, float charge)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (SqlTransaction tran = connection.BeginTransaction())
                    {
                        try
                        {
                            DataTable dt = new DataTable();

                            dt.Columns.Add("SeqNo");

                            dt.Columns.Add("CertNo");

                            dt.Columns.Add("ShKitta");

                            int i = 1;
                            PSLEntry.ForEach(x => dt.Rows.Add(i++, x.CertNo, x.ShKitta));

                            SqlCommand cmd = new SqlCommand("SAVE_PSL_BATCH_ENTRY", connection);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Transaction = tran;
                            SqlParameter param = cmd.Parameters.AddWithValue("@udt", dt);
                            param = cmd.Parameters.AddWithValue("@P_COMPCODE", CompCode);
                            param = cmd.Parameters.AddWithValue("@P_SHHOLDERNO", ShholderNo);
                            param = cmd.Parameters.AddWithValue("@P_CODE", Code);
                            param = cmd.Parameters.AddWithValue("@P_REMARKS", Remark);
                            param = cmd.Parameters.AddWithValue("@P_TRANDATE", Transdate);
                            param = cmd.Parameters.AddWithValue("@P_SELECTEDACTION", SelectedAction);
                            param = cmd.Parameters.AddWithValue("@P_USERNAME", UserName);
                            param = cmd.Parameters.AddWithValue("@P_PLEDGEAMOUNT", Pleggeamount);
                            param = cmd.Parameters.AddWithValue("@P_STATUS", Status);

                            param = cmd.Parameters.AddWithValue("@P_PSLNO ", pslno);
                            param = cmd.Parameters.AddWithValue("@P_IP", IP);
                            param = cmd.Parameters.AddWithValue("@P_CHARGE", charge);

                            param = cmd.Parameters.AddWithValue("@P_DATE_NOW", DateTime.Now);


                            param.Direction = ParameterDirection.Input;
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    if (reader.GetInt32(0) == 1)
                                    {
                                        jsonResponse.IsSuccess = true;
                                        jsonResponse.Message = ATTMessages.SAVED_SUCCESS; ;
                                    }
                                    else
                                    {
                                       
                                        jsonResponse.Message = ATTMessages.SAVED_SUCCESS; ;
                                    }

                                }
                            }
                            if (jsonResponse.IsSuccess)
                                tran.Commit();
                            else
                                tran.Rollback();
                        }
                        catch (Exception ex)
                        {
                            jsonResponse.IsSuccess = false;
                            jsonResponse.Message = ex.Message;
                        }
                    }
                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.Message = ex.Message;
                }
                return jsonResponse;
            }
        }


    }
}


