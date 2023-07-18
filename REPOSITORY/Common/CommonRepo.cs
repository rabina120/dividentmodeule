

using Dapper;
using Entity.Common;
using Interface.Common;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.Common
{
    public class CommonRepo : ICommon, IShareType
    {
        IOptions<ReadConfig> connectionString;
        public CommonRepo(IOptions<ReadConfig> connectionString)
        {
            this.connectionString = connectionString;
        }

        public JsonResponse GetAllShareTypes()
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {

                try
                {
                    connection.Open();
                    List<ATTLookupShareType> aTTLookupShareTypes = connection.Query<ATTLookupShareType>(sql: "COMMON_GETSHARETYPE", param: null, null, commandType: CommandType.StoredProcedure).ToList();

                    if (aTTLookupShareTypes != null)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = aTTLookupShareTypes;
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = ATTMessages.NO_RECORDS_FOUND;
                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.ResponseData = ex;
                    response.HasError = true;
                }
                return response;
            }
        }

        public JsonResponse GetCertificateStatus(string DependOn)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {

                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_DEPENDON", DependOn);
                    List<ATTCertificateStatus> certificateStatuses = connection.Query<ATTCertificateStatus>(sql: "GET_ALL_CERTIFICATE_STATUSES", param: param, null, commandType: CommandType.StoredProcedure).ToList();

                    if (certificateStatuses != null)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = certificateStatuses;
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = ATTMessages.NO_RECORDS_FOUND;
                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.ResponseData = ex;
                    response.HasError = true;
                }
                return response;
            }
        }

        public JsonResponse GetDistricts()
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {

                try
                {
                    connection.Open();
                    List<ATTDistricts> districtsToReturn = connection.Query<ATTDistricts>(sql: "GET_All_DISTRICT", param: null, null, commandType: CommandType.StoredProcedure).ToList();

                    if (districtsToReturn != null)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = districtsToReturn;
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = ATTMessages.NO_RECORDS_FOUND;
                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.ResponseData = ex;
                    response.HasError = true;
                }
                return response;
            }
        }

        public JsonResponse GetOccupations(string shownertype)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_SHOWNERTYPE", (shownertype));

                    List<ATTOccupations> occupationsToReturn = connection.Query<ATTOccupations>(sql: "GET_ALl_OCCUPATION", param: param, null, commandType: CommandType.StoredProcedure).ToList();

                    if (occupationsToReturn.Count > 0)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = occupationsToReturn;
                    }
                    else
                    {
                        response.Message = ATTMessages.NO_RECORDS_FOUND;
                        response.IsSuccess = false;
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

        public JsonResponse GetOccupations()
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_SHOWNERTYPE", null);

                    List<ATTOccupations> occupationsToReturn = connection.Query<ATTOccupations>(sql: "GET_ALl_OCCUPATION", param: param, null, commandType: CommandType.StoredProcedure).ToList();

                    if (occupationsToReturn.Count > 0)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = occupationsToReturn;
                    }
                    else
                    {
                        response.Message = ATTMessages.NO_RECORDS_FOUND;
                        response.IsSuccess = false;
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

        public JsonResponse GetSelectedDistrict(string distcode)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("P_DISTCODE", Convert.ToInt32(distcode));
                    List<ATTDistricts> districtsToReturn = connection.Query<ATTDistricts>(sql: "GET_DISTRICT", param: param, null, commandType: CommandType.StoredProcedure).ToList();

                    if (districtsToReturn.Count > 0)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = districtsToReturn;
                    }
                    else
                    {
                        response.IsSuccess = false;
                        response.Message = ATTMessages.NO_RECORDS_FOUND;
                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.ResponseData = ex;
                    response.HasError = true;
                }
                return response;
            }
        }

        public JsonResponse GetSelectedOccupation(string occupationId)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_OCCUPATIONID", Convert.ToInt32(occupationId));

                    List<ATTOccupations> occupationsToReturn = connection.Query<ATTOccupations>(sql: "GET_OCCUPATION", param: param, null, commandType: CommandType.StoredProcedure).ToList();

                    if (occupationsToReturn.Count > 0)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = occupationsToReturn;
                    }
                    else
                    {
                        response.Message = ATTMessages.NO_RECORDS_FOUND;
                        response.IsSuccess = false;
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

        public JsonResponse GetShareOwnerType()
        {
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                JsonResponse response = new JsonResponse();
                response.Message = ATTMessages.NO_RECORDS_FOUND;
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();

                    List<ATTShareOwnerType> aTTShownerType = new List<ATTShareOwnerType>();
                    aTTShownerType = connection.Query<ATTShareOwnerType>("GetShareOwnerType", null, commandType: CommandType.StoredProcedure).ToList();
                    if (aTTShownerType != null)
                    {
                        response.IsSuccess = true;
                        response.Message = "Record of ShOwnerType " + aTTShownerType + " Found";
                        response.ResponseData = aTTShownerType;
                    }

                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;
                }
                return response;
            }
        }

        public JsonResponse GetShareTypes(string compcode)
        {
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                JsonResponse response = new JsonResponse();
                response.Message = "No Record Found";
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_COMPCODE",compcode);

                    List<ATTShareType> aTTShownerType = new List<ATTShareType>();
                    aTTShownerType = connection.Query<ATTShareType>("GET_CERTIFICATEDETAIL", param: param, null, commandType: CommandType.StoredProcedure).ToList();
                    if (aTTShownerType != null)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = aTTShownerType;
                    }

                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;
                }
                return response;
            }
        }

        public JsonResponse GetShOwnerSubTypes(string shOwnerTypeID)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_SHOWNERTYPE", (shOwnerTypeID));

                    List<ATTShOwnerSubTypes> shOwnerSubTypesToReturn = connection.Query<ATTShOwnerSubTypes>(sql: "GET_ALL_SHOWNERSUBTYPE", param: param, null, commandType: CommandType.StoredProcedure).ToList();

                    if (shOwnerSubTypesToReturn.Count > 0)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = shOwnerSubTypesToReturn;
                    }
                    else
                    {
                        response.Message = ATTMessages.NO_RECORDS_FOUND;
                        response.IsSuccess = false;
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
        public JsonResponse GetShOwnerSubTypes()
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_SHOWNERTYPE", null);

                    List<ATTShOwnerSubTypes> shOwnerSubTypesToReturn = connection.Query<ATTShOwnerSubTypes>(sql: "GET_ALL_SHOWNERSUBTYPE", param: param, null, commandType: CommandType.StoredProcedure).ToList();

                    if (shOwnerSubTypesToReturn.Count > 0)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = shOwnerSubTypesToReturn;
                    }
                    else
                    {
                        response.Message = ATTMessages.NO_RECORDS_FOUND;
                        response.IsSuccess = false;
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

        public JsonResponse GetShOwnerTypes(bool isLookUp)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_ISLOOKUP", isLookUp);
                    List<ATTShOwnerTypes> shOwnerTypesToReturn = connection.Query<ATTShOwnerTypes>(sql: "GET_ALL_SHOWNERTYPE", param: param, null, commandType: CommandType.StoredProcedure).ToList();

                    if (shOwnerTypesToReturn.Count > 0)
                    {
                        response.IsSuccess = true;
                        response.ResponseData = shOwnerTypesToReturn;
                    }
                    else
                    {
                        response.Message = ATTMessages.NO_RECORDS_FOUND;
                        response.IsSuccess = false;
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
    }
}
