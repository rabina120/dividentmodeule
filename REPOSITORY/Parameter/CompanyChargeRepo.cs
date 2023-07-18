using Dapper;
using Entity.Common;
using Entity.Parameter;
using Interface.Parameter;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.Parameter
{
    public class CompanyChargeRepo : ICompanyCharge
    {
        IOptions<ReadConfig> connectionstring;
        public CompanyChargeRepo(IOptions<ReadConfig> _connectionstring)
        {
            this.connectionstring = _connectionstring;
        }
        public JsonResponse GetChargeCode(string CompCode)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionstring.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@CompCode", CompCode);

                    string sqlQuery = "GETCHARGECODE";
                    var data = connection.Query<string>(sql: sqlQuery, param, null, commandType: CommandType.StoredProcedure)?.FirstOrDefault();

                    jsonResponse.Message = data != null ? data.ToString() : "01";

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

        public JsonResponse GetCompanyCharge(string Compcode, string ChargeCode, string UserName, string IPAddress)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionstring.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@Compcode", Compcode);
                    param.Add("@ChargeCode", ChargeCode);
                    param.Add("@USERNAME", UserName);
                    param.Add("@ENTRY_DATE", DateTime.Now);
                    param.Add("@IP_ADDRESS", IPAddress);

                    ATTCompanyCharge aTTCompanyCharge = connection.Query<ATTCompanyCharge>("GETCOMPANYCHARGE", param, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                    if (aTTCompanyCharge != null)
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = aTTCompanyCharge;
                    }
                    else
                    {
                        jsonResponse.IsSuccess = false;
                        jsonResponse.ResponseData = ATTMessages.NO_RECORDS_FOUND;
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

        public JsonResponse GetCompanyChargeDetail(string CompCode, string UserName, string IPAddress)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionstring.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();


                    DynamicParameters param = new DynamicParameters();
                    param.Add("@CompCode", CompCode);
                    param.Add("@USERNAME", UserName);
                    param.Add("@ENTRY_DATE", DateTime.Now);
                    param.Add("@IP_ADDRESS", IPAddress);

                    List<ATTCompanyCharge> aTTCompanyCharge = connection.Query<ATTCompanyCharge>("GetCompanyChargeDetail", param, commandType: CommandType.StoredProcedure)?.ToList();
                    if (aTTCompanyCharge != null)
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = aTTCompanyCharge;
                    }
                    else
                    {
                        jsonResponse.IsSuccess = false;
                        jsonResponse.ResponseData = ATTMessages.NO_RECORDS_FOUND;
                    }
                }
                catch (Exception ex)
                {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.HasError = true;
                    jsonResponse.ResponseData = ex;
                }
            }
            return jsonResponse;
        }

        public JsonResponse SaveCompanyCharge(ATTCompanyCharge aTTCompanyCharge, string ActionType, string UserName, string IPAddress)
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
                        param.Add("@CompCode", aTTCompanyCharge.CompCode);
                        param.Add("@chargecode", aTTCompanyCharge.ChargeCode);
                        param.Add("@Charge_Desc", aTTCompanyCharge.Charge_Desc);
                        param.Add("@Charge", aTTCompanyCharge.Charge);
                        param.Add("@ActionType", ActionType);
                        param.Add("@USERNAME", UserName);
                        param.Add("@ENTRY_DATE", DateTime.Now);
                        param.Add("@IP_ADDRESS", IPAddress);


                        jsonResponse = connection.Query<JsonResponse>("INSERT_UPDATE_CompanyChargeSetup", param, trans, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                        if (jsonResponse.IsSuccess)
                            trans.Commit();
                        else
                        {

                            jsonResponse.IsSuccess = false;
                            jsonResponse.ResponseData = ATTMessages.CANNOT_SAVE;

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
    }
}
