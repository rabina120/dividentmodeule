



using Dapper;
using Entity.CDS;
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
    public class ReMaterializeEntryRepo : IReMaterializeEntry
    {
        IOptions<ReadConfig> _connectionString;

        public ReMaterializeEntryRepo(IOptions<ReadConfig> connectionString)
        {
            _connectionString = connectionString;
        }
        public JsonResponse GetMaxRemageRegNo(string Compcode)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection con = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    con.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_CompCode", Compcode);
                    int? maxRemateregNo = con.Query<int?>("GET_MAX_REG_NO_DEMATE_ENTRY", param, commandType: CommandType.StoredProcedure).FirstOrDefault();

                    if (maxRemateregNo != null)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = maxRemateregNo + 1;
                    }
                    else
                    {
                        response.IsSuccess = true;
                        response.ResponseData = 1;
                    }

                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;
                }
                return response;
            }
        }

        public JsonResponse GetHolderDetails(string Compcode, int Occupation, string ShHolderNo)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection con = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    con.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_CompCode", Compcode);
                    param.Add("@P_HolderNo", ShHolderNo);
                    param.Add("@P_Occupation", Occupation);

                    ATTShHolder aTTShHolders = con.Query<ATTShHolder>("GET_SHHOLDER_INFORMATION_DEMATE", param, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                    if (aTTShHolders != null)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = aTTShHolders;
                    }
                    else
                    {
                        response.Message = "Data cannot be found!!!";
                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.Message = ex.Message;
                }
            }
            return response;
        }

        public JsonResponse GetParaCompChild(string CompCode)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection con = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    con.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@CompCode", CompCode);

                    List<ParaComp_Child> paraComp_Child = con.Query<ParaComp_Child>("GET_PARACOMP_CHILD", param, commandType: CommandType.StoredProcedure)?.ToList();
                    if (paraComp_Child.Count > 0)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = paraComp_Child;
                    }

                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.Message = ex.Message;

                }

            }
            return response;
        }

        public JsonResponse GetCertificate(string Compcode, int Certno, int Shownertype, int ShHolderno)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection con = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    con.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@Compcode", Compcode);
                    param.Add("@Certno", Certno);
                    param.Add("@Shownertype", Shownertype);
                    param.Add("@Shholderno", ShHolderno);

                    List<ATTCertDet> aTTCert = con.Query<ATTCertDet>("Get_Certificate_detail", param, commandType: CommandType.StoredProcedure).ToList();
                    if (aTTCert.Count > 0)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = aTTCert;
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = "No certificate found.";
                    }
                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;
                }
            }
            return response;
        }

        public JsonResponse SaveRematerializeCertificate(List<ATTCertDet> certificateList, string compCode, string demateRegNo, string shHolderNo, string TranDate, string bOID, string drnNo, string dPCode, string remarks, string regNO, string iSINNo, string bonusCode, string selectedAction, string Username)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection con = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    con.Open();
                    using (SqlTransaction tran = con.BeginTransaction())
                    {
                        try
                        {
                            DynamicParameters param = new DynamicParameters();
                            param.Add("@compcode", compCode);
                            param.Add("@demateRegno", demateRegNo);
                            param.Add("@shholderno", shHolderNo);
                            param.Add("@boid", bOID);
                            param.Add("@drnno", drnNo);
                            param.Add("@dpcode", dPCode);
                            param.Add("@remarks", remarks);
                            param.Add("@regno", regNO);
                            param.Add("@isin_no", iSINNo);
                            param.Add("@bonuscode", bonusCode);
                            param.Add("@Actiontype", selectedAction);
                            param.Add("@tranDate", TranDate);
                            param.Add("@entryDate", DateTime.Now.ToString("yyyy-MM-dd"));
                            param.Add("@username", Username);

                            int seqno = 1;
                            foreach (ATTCertDet aTTCertDet in certificateList)
                            {
                                param.Add("@seqNo", seqno);
                                param.Add("@certNo", aTTCertDet.CertNo);
                                param.Add("@certStatus", aTTCertDet.CertStatus);
                                param.Add("@srNoFrom", aTTCertDet.SrNoFrom);
                                param.Add("@srNoTo", aTTCertDet.SrNoTo);
                                param.Add("@shKitta", aTTCertDet.ShKitta);
                                param.Add("@shareType", aTTCertDet.ShareType);
                                param.Add("@shOwnerType", aTTCertDet.ShOwnerType);

                                con.Query("Rematerialize_Certificate", param, tran, commandType: CommandType.StoredProcedure);

                                seqno++;
                            }
                            response.IsSuccess = true;
                            response.Message = "Certificate Have Been Rematerialize";
                            tran.Commit();
                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
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
    }

}
