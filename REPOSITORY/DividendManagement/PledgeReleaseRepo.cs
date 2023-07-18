using Dapper;
using Entity.Common;
using Entity.DemateDividend;
using INTERFACE.DividendManagement;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace REPOSITORY.DividendManagement
{
    public class PledgeReleaseRepo : IPledgeRelease
    {
        IOptions<ReadConfig> _connectionString;

        public PledgeReleaseRepo(IOptions<ReadConfig> connectionString)
        {
            this._connectionString = connectionString;
        }

        public JsonResponse SavePRList(string CompCode, string DivCode, List<ATTPledgeReleaseDataList> PRList, string Status, string SelectedAction, string Username, string IPAddress)
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
                        param.Add("@P_CompCode", CompCode);
                        param.Add("@P_DivCode", DivCode);
                        param.Add("@P_TRANTYPE", Status);
                        param.Add("@P_Username", Username);
                        param.Add("@P_IPAddress", IPAddress);
                        param.Add("@P_ENTRY_DATE", DateTime.Now);
                        foreach(ATTPledgeReleaseDataList pr in PRList)
                        {
                            param.Add("@P_BOID", pr.Boid);
                            param.Add("@P_PNAME", pr.Name);
                            param.Add("@P_PDATE", pr.Date);
                            param.Add("@P_PREMARKS", pr.Remarks);

                            response = connection.Query<JsonResponse>("SAVE_HOLDER_FOR_PLEDGE_RELEASE", param, trans, commandType: System.Data.CommandType.StoredProcedure).FirstOrDefault();
                        }
                    }
                    catch (Exception ex)
                    {
                        response.IsSuccess = false;
                        response.HasError = true;
                        response.Message = ex.Message;
                        trans.Rollback();
                    }
                }

            }
            return response;
        }
    }
}
