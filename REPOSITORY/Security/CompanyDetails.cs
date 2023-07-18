
using Dapper;
using Entity.Common;
using Interface.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;


using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
namespace Repository.Security
{
    public class CompanyDetails : ICompanyDetails
    {
        IOptions<ReadConfig> connectionString;
        IConfiguration _configuration;
        public CompanyDetails(IOptions<ReadConfig> connectionString, IConfiguration configuration)
        {
            this.connectionString = connectionString;
            this._configuration = configuration;
        }
        public JsonResponse GetCompanyDetails(string CompCode)
        {
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                JsonResponse response = new JsonResponse();
                response.Message = "No Record Found";
                string isLDAP = _configuration.GetSection("LDAPAuthentication").GetSection("Enabled").Value;
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("CompCode", CompCode);
                    List<ATTCompany> aTTCompanies = new List<ATTCompany>();
                    aTTCompanies = connection.Query<ATTCompany>("GetParaCompDetails", param, commandType: CommandType.StoredProcedure).ToList();
                    if (aTTCompanies != null)
                    {
                        response.IsSuccess = true;
                        response.Message = "Record of Company " + CompCode + " Found";
                        response.ResponseData = aTTCompanies;

                    }

                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;
                }
                response.IsValid = isLDAP.ToLower() == "true" ? false : true;
                return response;
            }
        }
    }
}
