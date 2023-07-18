using Dapper;
using Entity.Common;
using ENTITY.Parameter;
using INTERFACE.Parameter;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace REPOSITORY.Parameter
{
    public class OwnerCategorySetupRepo : IOwnerCategorySetup
    {
        IOptions<ReadConfig> _connectionString;
        public OwnerCategorySetupRepo(IOptions<ReadConfig> connectionString)
        {
            this._connectionString = connectionString;
        }



        public JsonResponse GetOwnerCategory(string username, string ipaddress)
        {

            var response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                connection.Open();
                using (SqlTransaction trans = connection.BeginTransaction())
                {
                    try
                    {
                        DynamicParameters param = new DynamicParameters();

                        //  param.Add("@P_Username", username);
                        //param.Add("@P_IPAddress", ipaddress);
                        //param.Add("@P_ENTRY_DATE", DateTime.Now);




                        var list = connection.Query<ATTOwnerCategory>("GetOwnerCategory", param, trans, commandType: System.Data.CommandType.StoredProcedure).ToList();
                        if (list.Count > 0)
                        {
                            response.IsSuccess = true;
                            response.ResponseData = list;
                        }
                        if (response.IsSuccess)

                        {
                            trans.Commit();
                        }
                    }
                    catch (Exception ex)
                    {
                        response.IsSuccess = false;
                        response.HasError = true;
                        response.Message = ex.Message;

                    }

                    return response;
                }

            }
        }
        public JsonResponse SaveOwnerCategory(List<ATTOwnerCategory> ShownerType, string username, string ipaddress)
        {

            {
                var response = new JsonResponse();
                using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
                {
                    connection.Open();
                    using (SqlTransaction trans = connection.BeginTransaction())
                    {
                        try
                        {
                            DynamicParameters param = new DynamicParameters();

                            //  param.Add("@P_Username", username);
                            //param.Add("@P_IPAddress", ipaddress);
                            //param.Add("@P_ENTRY_DATE", DateTime.Now);
                            foreach (ATTOwnerCategory pr in ShownerType)
                            {
                                param.Add("@Shownersubtypeid", pr.ShownerSubTypeId);
                                param.Add("@Shownertypeid", pr.ShownerTypeId);
                                param.Add("@@Shownersubtype", pr.ShownerSubType);


                                response = connection.Query<JsonResponse>("SaveOwnerCategory", param, trans, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();

                            }
                            if (response.IsSuccess)
                            {
                                trans.Commit();
                            }
                        }
                        catch (Exception ex)
                        {
                            response.IsSuccess = false;
                            response.HasError = true;
                            response.Message = ex.Message;

                        }
                    }

                }
                return response;
            }
        }
    }
}
