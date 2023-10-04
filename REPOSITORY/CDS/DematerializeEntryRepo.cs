using Dapper;
using Entity.CDS;
using Entity.Certificate;
using Entity.Common;
using Entity.ShareHolder;
using Interface.CDS;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.CDS
{
    public class DematerializeEntryRepo : IDematerializeEntry
    {
        IOptions<ReadConfig> _connectionString;
        public DematerializeEntryRepo(IOptions<ReadConfig> connectionString)
        {
            _connectionString = connectionString;
        }

        public JsonResponse GetAllParaCompChild(string CompCode)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@CompCode", CompCode);

                    List<ParaComp_Child> paraComps = connection.Query<ParaComp_Child>("GET_PARACOMP_CHILD", param, commandType: CommandType.StoredProcedure)?.ToList();
                    if (paraComps.Count > 0)
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = paraComps;
                    }
                    else
                    {
                        jsonResponse.Message = "ISINO For Company Not Found";
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

        public JsonResponse GetMaxRegNo(string CompCode)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@P_CompCode", CompCode);
                    int maxRegNo = connection.Query<int>("GET_MAX_REG_NO_DEMATE_ENTRY", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    if (maxRegNo == 0)
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = maxRegNo + 1;

                    }
                    else
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = maxRegNo + 1;
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

        public JsonResponse GetShHolderInformation(string CompCode, string ShHolderNo, string Occupation, string UserName, string IP)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_CompCode", CompCode);
                    param.Add("@P_HolderNo", ShHolderNo);
                    param.Add("@P_Occupation", Occupation);
                    param.Add("@P_USERNAME", UserName);
                    param.Add("@P_IP_ADDRESS", IP);
                    param.Add("@P_DATE_NOW", DateTime.Now);
                    ATTShHolder shHolder = connection.Query<ATTShHolder>("GET_SHHOLDER_INFORMATION_DEMATE", param, commandType: CommandType.StoredProcedure)?.FirstOrDefault();

                    if (shHolder != null)
                    {
                        if (shHolder.HolderLock == "N" || shHolder.HolderLock == null)
                        {
                            jsonResponse.IsSuccess = true;
                            jsonResponse.ResponseData = shHolder;
                        }
                        else
                        {
                            jsonResponse.IsSuccess = false;
                            jsonResponse.Message = ATTMessages.SHARE_HOLDER.LOCK;
                        }
                    }
                    else
                    {
                        jsonResponse.Message = ATTMessages.SHARE_HOLDER.NOT_FOUND;
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

        public JsonResponse GetCertificateDetails(string CompCode, string DemateType, string ShOwnerType, string HolderNo, string SrNoFrom, string SrNoTo, string UserName, string IP)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_CompCode", CompCode);
                    param.Add("@P_DemateType", DemateType);
                    param.Add("@P_ShOwnerType", ShOwnerType);
                    param.Add("@P_HolderNo", HolderNo);
                    param.Add("@P_SrNoFrom", SrNoFrom);
                    param.Add("@P_SrNoTo", SrNoTo);
                    param.Add("@P_USERNAME", UserName);
                    param.Add("@P_IP_ADDRESS", IP);
                    param.Add("@P_DATE_NOW", DateTime.Now);

                    List<ATTCertDet> certDetailsToReturn = connection.Query<ATTCertDet>(sql: "GET_CERTIFICATE_LIST_DEMATE_ENTRY", param: param, commandType: CommandType.StoredProcedure)?.ToList();
                    if (certDetailsToReturn.Count > 0)
                    {
                        jsonResponse.ResponseData = certDetailsToReturn;
                        jsonResponse.IsSuccess = true;
                    }
                    else
                    {
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

        public JsonResponse GetDataFromCertificateDetail(string CompCode)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_CompCode", CompCode);

                    List<ATTCertificateDetail> certDetailsToReturn = connection.Query<ATTCertificateDetail>(sql: "GET_DATA_FROM_CERTIFICATE_DETAIL", param: param, commandType: CommandType.StoredProcedure)?.ToList();
                    if (certDetailsToReturn.Count > 0)
                    {
                        jsonResponse.ResponseData = certDetailsToReturn;
                        jsonResponse.IsSuccess = true;
                    }
                    else
                    {
                        jsonResponse.Message = "No Certificate Detail List For Company Found !!!<br/> Are You Sure You Are Choosing the Right Company? <br/>";
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

        public JsonResponse GetMaxDemateRegNo(string CompCode)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@P_CompCode", CompCode);
                    int? maxDemateRegNo = connection.Query<int?>("GET_MAX_DEMATE_REG_NO_DEMATE_ENTRY", parameters, commandType: CommandType.StoredProcedure).FirstOrDefault();
                    if (maxDemateRegNo != null)
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = maxDemateRegNo + 1;

                    }
                    else
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = 1;
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

        public JsonResponse SaveDematerializeCertificate(List<ATTCertDet> CertificateList, string CompCode, string DemateRegNo, string ShHolderNo, string EntryDate, string DemateReqDate, string BOID, string DrnNo, string DPCode, string Remarks, string RegNO, string ISINNo, string BonusCode, string SelectedAction, string UserName, string IP)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (SqlTransaction trans = connection.BeginTransaction())
                    {
                        #region delete certficate for delete and update



                        if (SelectedAction == "U" || SelectedAction == "D")
                        {
                            DynamicParameters parameters = new DynamicParameters();
                            parameters.Add("@P_CompCode", CompCode);
                            parameters.Add("@P_UserName", UserName);
                            parameters.Add("@P_DemateRegNo", DemateRegNo);
                            parameters.Add("@P_SelectedAction", SelectedAction);
                            parameters.Add("@P_DemateReqDate", DemateReqDate);
                            parameters.Add("@P_IP_ADDRESS", IP);
                            parameters.Add("@P_DATE_NOW", DateTime.Now);
                            jsonResponse = connection.Query<JsonResponse>(sql: "DELETE_CERTIFICATE_FOR_DEMATERIALIZATION_UPDATE_OR_DELETE", param: parameters, trans, commandType: CommandType.StoredProcedure)?.FirstOrDefault();

                            if (!jsonResponse.IsSuccess)
                            {
                                trans.Rollback();
                                jsonResponse.Message = ATTMessages.CERTIFICATE.DELETE_FAILED;
                                return jsonResponse;
                            }
                            else
                            {
                                jsonResponse.IsSuccess = true;
                                if (SelectedAction == "D")
                                {
                                    trans.Commit();
                                    jsonResponse.Message = ATTMessages.CERTIFICATE.DELETE_SUCCESS;
                                    return jsonResponse;
                                }
                            }


                        }
                        #endregion

                        #region adding certificate udt
                        if (SelectedAction == "A" || SelectedAction == "U")
                        {
                            DataTable dt = new DataTable();

                            dt.Columns.Add("SEQ_NO");
                            dt.Columns.Add("Certno");

                            int i = 1;
                            CertificateList.ForEach(x => dt.Rows.Add(i++, x.CertNo));

                            SqlCommand cmd = new SqlCommand("SAVE_CERTIFICATE_FOR_DEMATERIALIZATION", connection);
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.Transaction = trans;

                            SqlParameter param = cmd.Parameters.AddWithValue("@udt", dt);
                            param = cmd.Parameters.AddWithValue("@P_EntryDate", EntryDate);
                            param = cmd.Parameters.AddWithValue("@P_BOID", BOID);
                            param = cmd.Parameters.AddWithValue("@P_DRN_NO", DrnNo);
                            param = cmd.Parameters.AddWithValue("@P_DPCode", DPCode);
                            param = cmd.Parameters.AddWithValue("@P_Remarks", Remarks);
                            param = cmd.Parameters.AddWithValue("@P_RegNo", RegNO);
                            param = cmd.Parameters.AddWithValue("@P_ISINNO", ISINNo);
                            param = cmd.Parameters.AddWithValue("@P_BonusCode", BonusCode);
                            param = cmd.Parameters.AddWithValue("@P_CompCode", CompCode);
                            param = cmd.Parameters.AddWithValue("@P_UserName", UserName);
                            param = cmd.Parameters.AddWithValue("@P_DemateRegNo", DemateRegNo);
                            param = cmd.Parameters.AddWithValue("@P_SelectedAction", SelectedAction);
                            param = cmd.Parameters.AddWithValue("@P_DemateReqDate", DemateReqDate);
                            param = cmd.Parameters.AddWithValue("@P_IP_ADDRESS", IP);
                            param = cmd.Parameters.AddWithValue("@P_DATE_NOW", DateTime.Now);

                            param.Direction = ParameterDirection.Input;
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    if (reader.GetString(0) == "1")
                                    {
                                        jsonResponse.IsSuccess = true;
                                        jsonResponse.ResponseData = reader.GetInt32(1);
                                        jsonResponse.Message = ATTMessages.CERTIFICATE.DEMATE_ENTRY + "\n with Reg No : " + reader.GetInt32(1);
                                    }
                                    else
                                    {
                                        jsonResponse.Message = ATTMessages.CERTIFICATE.DEMATE_ENTRY_FALIED;
                                    }

                                }
                            }

                        }
                        #endregion

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

        public JsonResponse GetSignature(string CompCode, string HolderNo)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {

                try
                {
                    connection.Open();

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_COMPCODE", (CompCode));
                    param.Add("@P_HOLDERNO", (HolderNo));

                    ATTSignatureDematerialize signaturesToReturn = connection.Query<ATTSignatureDematerialize>(sql: "GET_SIGNATURE", param: param, null, commandType: CommandType.StoredProcedure)?.FirstOrDefault();

                    if (signaturesToReturn != null)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = signaturesToReturn;
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

        public JsonResponse GetHolderByQuery(string CompCode, string FirstName, string Occupation)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@P_CompCode", CompCode);
                    parameters.Add("@P_FirstName", FirstName);
                    parameters.Add("@P_Occupation", Occupation);


                    List<ATTShHolder> aTTShHolders = connection.Query<ATTShHolder>(sql: "GET_SHHOLDER_INFORMATION_BY_QUERY_DEMATE_ENTRY", param: parameters, null, commandType: CommandType.StoredProcedure)?.ToList();

                    if (aTTShHolders.Count > 0)
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = aTTShHolders;
                    }
                    else
                    {
                        jsonResponse.Message = "Cannot Find the Holder.";
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

        public JsonResponse GetDematedCertificateList(string CompCode, string HolderNo)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@P_CompCode", CompCode);
                    parameters.Add("@P_HolderNo", HolderNo);


                    //List<CertificateDemateDetails> certificateDemateDetails = SqlMapper.Query<CertificateDemateDetails,ATTShHolder,CertificateDemateDetails>(connection, sql: "GET_DEMATED_CERTIFICATE_LIST", 
                    //    (certificate, shholder) =>
                    //    {
                    //        certificate.aTTShHolder = shholder;
                    //        return certificate;
                    //    },
                    //    param: parameters, null,splitOn: "ShHolder", commandType: CommandType.StoredProcedure)?.ToList();


                    List<CertificateDemateDetails> certificateDemateDetails = SqlMapper.QueryAsync<CertificateDemateDetails, ATTShHolder, CertificateDemateDetails>(connection, "GET_DEMATED_CERTIFICATE_LIST",
                        (certificate, holder) =>
                        {
                            certificate.aTTShHolder = holder;
                            return certificate;
                        }, parameters, null, splitOn: "ShHolder", commandType: CommandType.StoredProcedure)?.Result.AsList();


                    if (certificateDemateDetails.Count > 0)
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = certificateDemateDetails;
                    }
                    else
                    {
                        jsonResponse.Message = "Cannot Find the Certificates.";
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

        public JsonResponse GetDematedCertificateDetails(string CompCode, string DemateRegNo)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@P_CompCode", CompCode);
                    parameters.Add("@P_DemateRegNo", DemateRegNo);
                    List<ATTCertDet> certDets = connection.Query<ATTCertDet>("GET_DEMATED_CERTIFICATE_DETAILS", parameters, commandType: CommandType.StoredProcedure)?.ToList();
                    if (certDets.Count > 0)
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = certDets;
                    }
                    else
                    {
                        jsonResponse.Message = "No Certificate Found !!!";
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

        public JsonResponse GetStartSrNoEndSrNo(string CompCode, string BonusIssueCode)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters parameters = new DynamicParameters();
                    parameters.Add("@P_CompCode", CompCode);
                    parameters.Add("@P_BonusIssueCode", BonusIssueCode);
                    List<ATTCertDet> certDets = connection.Query<ATTCertDet>("GET_START_END_SRNO_CERTIFICATE_BONUS_ISSUE", parameters, commandType: CommandType.StoredProcedure)?.ToList();
                    if (certDets.Count > 0)
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = certDets;
                    }
                    else
                    {
                        jsonResponse.Message = "No Record Found !!!";
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
    }
}
