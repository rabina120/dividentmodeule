

using Dapper;

using Entity.Common;
using ENTITY.Parameter;
using INTERFACE.Parameter;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace REPOSITORY.Parameter
{
    public class CDSCompanyParameterRepo : ICDSCompanyParameter
    {
        IOptions<ReadConfig> _connectionstring;
        public CDSCompanyParameterRepo(IOptions<ReadConfig> connectionstring)
        {
            _connectionstring = connectionstring;
        }
        public JsonResponse SaveCDSCompanyParameterList(string compCode, List<ATTCDSCompanyParameter> companyParameterLists, string userName, string ipAddress)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionstring.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (SqlTransaction trans = connection.BeginTransaction())
                    {
                        DynamicParameters param = new DynamicParameters();
                        param.Add("@compCode", compCode);
                        param.Add("@username", userName);
                        param.Add("@ipAddress", ipAddress);
                        param.Add("@entryDate", DateTime.Now);

                        int isDeleted = 0;

                        List<ATTCDSCompanyParameter> comParameter = new List<ATTCDSCompanyParameter>();
                        param.Add("@isDeleted", isDeleted);

                        jsonResponse = connection.Query<JsonResponse>("Save_CDSCompanyParameter", param, trans, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                        isDeleted = 1;


                        foreach (var companyParameter in companyParameterLists)
                        {
                            param.Add("@isDeleted", isDeleted);
                            param.Add("@isin_no", companyParameter.IsinNo);
                            param.Add("@shholderNo", companyParameter.ShholderNo);
                            param.Add("@desc_share", companyParameter.Description);
                            param.Add("@shownerType", companyParameter.ownerType);
                            jsonResponse = connection.Query<JsonResponse>("Save_CDSCompanyParameter", param, trans, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                        }
                        //jsonResponse = connection.Query<JsonResponse>("INSERT_UPDATE_OCCUPATION", param, trans, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                        //if (jsonResponse.IsSuccess)
                        //    trans.Commit();
                        //else
                        //{
                        //    trans.Rollback();
                        //    jsonResponse.Message = ATTMessages.CANNOT_SAVE;
                        //}

                        
                        if (jsonResponse.IsSuccess)
                        {
                            trans.Commit();

                        }
                        else
                        {
                            trans.Rollback();
                            jsonResponse.Message = ATTMessages.CANNOT_SAVE;
                        }

                    }
                }
                catch (Exception ex) {
                    jsonResponse.IsSuccess = false;
                    jsonResponse.HasError = true;
                    jsonResponse.ResponseData = ex;
                }
            }

            return jsonResponse;
        }

        public JsonResponse GetCDSCompanyParameter(string compCode)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionstring.Value.DefaultConnection)))
            {
                try {
                    connection.Open();
                    using (SqlTransaction trans = connection.BeginTransaction())
                    {
                        DynamicParameters param = new DynamicParameters();
                        param.Add("@compCode", compCode);
                        List<ATTCDSCompanyParameter> lst = connection.Query<ATTCDSCompanyParameter>("Get_CDSCompanyParameter", param, trans, commandType: CommandType.StoredProcedure)?.ToList();

                        if (lst.Count > 0)
                        {
                            jsonResponse.ResponseData = lst;
                            jsonResponse.IsSuccess = true;
                            trans.Commit();

                        }
                        else
                        {
                            trans.Rollback();
                            jsonResponse.Message = ATTMessages.NO_RECORDS_FOUND;
                        }


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

        public JsonResponse UpdateCDSCompanyParameter(string compCode, ATTCDSCompanyParameter cdsParameter)
        {
        JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionstring.Value.DefaultConnection)))
            {
                try 
                {
                connection.Open();
                    using (SqlTransaction trans = connection.BeginTransaction())
                    {
                        DynamicParameters param = new DynamicParameters();
                        param.Add("@compCode", compCode);
                        param.Add("@isin_no", cdsParameter.IsinNo);
                        param.Add("@shholderNo", cdsParameter.ShholderNo);
                        param.Add("@desc_share", cdsParameter.Description);
                        param.Add("@shownerType", cdsParameter.ownerType);
                        

                    }

                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.HasError = true;
                    response.ResponseData = ex;

                }
            }
                return response;
        }
    }
}
