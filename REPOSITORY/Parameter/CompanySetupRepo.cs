using Dapper;
using Entity.Common;
using Entity.Parameter;
using Interface.Parameter;
using Microsoft.Extensions.Options;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.Parameter
{
    public class CompanySetupRepo : ICompanySetup
    {
        IOptions<ReadConfig> connectionstring;

        public CompanySetupRepo(IOptions<ReadConfig> _connectionstring)
        {
            this.connectionstring = _connectionstring;

        }
        public JsonResponse GetCompanyCode()
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionstring.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    string sqlQuery = "GETCOMPCODE ";
                    var data = connection.Query<int>(sql: sqlQuery, param: null, null, commandType: null)?.FirstOrDefault();
                    jsonResponse.Message = ((1000 + data).ToString()).Substring(1, 3);
                    if (jsonResponse.Message != null)
                    {
                        jsonResponse.IsSuccess = true;
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

        public JsonResponse GetCompanyDetails(string CompCode)
        {
            JsonResponse jsonResponse = new JsonResponse();

            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionstring.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_COMCODE", CompCode);
                    ATTCompanySetup aTTCompanySetup = connection.Query<ATTCompanySetup>("Get_Company_Details", param: param, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                    if (aTTCompanySetup != null)
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = aTTCompanySetup;
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

        public JsonResponse SaveCompanyDetails(ATTCompanySetup aTTCompanySetup, string ActionType, string UserName)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionstring.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (SqlTransaction trans = connection.BeginTransaction())
                    {
                        DynamicParameters param = new DynamicParameters();
                        param.Add("P_CompCode ", aTTCompanySetup.CompCode);
                        param.Add("P_CompEnName ", aTTCompanySetup.CompEnName);
                        param.Add("P_CompEnAdd1 ", aTTCompanySetup.CompEnAdd1);
                        param.Add("P_CompEnAdd2 ", aTTCompanySetup.CompEnAdd2 ?? aTTCompanySetup.CompEnAdd2);
                        param.Add("P_CompNpName ", aTTCompanySetup.CompNpName ?? aTTCompanySetup.CompNpName);
                        param.Add("P_CompNpAdd1 ", aTTCompanySetup.CompNpAdd1 ?? aTTCompanySetup.CompNpAdd1);
                        param.Add("P_CompNpAdd2", aTTCompanySetup.CompNpAdd2 ?? aTTCompanySetup.CompNpAdd2);
                        param.Add("P_TelNo", aTTCompanySetup.TelNo);
                        param.Add("P_PBoxNo", aTTCompanySetup.PBoxNo);
                        param.Add("P_Email", aTTCompanySetup.Email ?? aTTCompanySetup.Email);
                        param.Add("P_MaxKitta", aTTCompanySetup.MaxKitta);
                        param.Add("P_PerShVal", aTTCompanySetup.PerShVal);
                        param.Add("P_fstCallVal", aTTCompanySetup.fstCallVal);
                        param.Add("P_fstCallDt", aTTCompanySetup.fstCallDt);
                        param.Add("P_sndCallVal", aTTCompanySetup.sndCallVal ?? aTTCompanySetup.sndCallVal);
                        param.Add("P_sndCallDt", aTTCompanySetup.sndCallDt ?? aTTCompanySetup.sndCallDt);
                        param.Add("P_trdCallVal", aTTCompanySetup.trdCallVal ?? aTTCompanySetup.trdCallVal);
                        param.Add("P_trdCallDt", aTTCompanySetup.trdCallDt ?? aTTCompanySetup.trdCallDt);
                        param.Add("P_SoftInstDt", aTTCompanySetup.SoftInstDt ?? aTTCompanySetup.SoftInstDt);
                        param.Add("P_SoftInstNo", aTTCompanySetup.SoftInstNo ?? aTTCompanySetup.SoftInstNo);
                        param.Add("P_SignDir", aTTCompanySetup.SignDir ?? aTTCompanySetup.SignDir);
                        param.Add("P_SoldSoftCode", aTTCompanySetup.SoldSoftCode ?? aTTCompanySetup.SoldSoftCode);
                        param.Add("P_AgentName  ", aTTCompanySetup.AgentName ?? aTTCompanySetup.AgentName);
                        param.Add("P_CurAgmNo", aTTCompanySetup.SoldSoftCode ?? aTTCompanySetup.SoldSoftCode);
                        param.Add("P_IsEngDt ", true);
                        param.Add("P_Action", ActionType);
                        param.Add("UserName", UserName);
                        jsonResponse = connection.Query<JsonResponse>("INSERT_UPDATE_PARACOMP", param, trans, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                        trans.Commit();

                    }
                    //AuditRepo audit = new AuditRepo(connectionstring);
                    //audit.auditSave(UserName, "Company Setup ", "Company Setup insert Update");

                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.ResponseData = ex;
                    jsonResponse.HasError = true;

                }
                return jsonResponse;
            }
        }
    }
}
