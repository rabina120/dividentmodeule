using Dapper;
using Entity.Common;
using ENTITY.Dividend;
using INTERFACE.DividendManagement;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace REPOSITORY.DividendManagement
{
    public class CalculationRepo : ICalculationRepo
    {
        IOptions<ReadConfig> _connectionString;
        public CalculationRepo(IOptions<ReadConfig> connectionString)
        {
            _connectionString = connectionString;
        }

        public JsonResponse SaveCalculation(ATTCalculation data, string selectedOption)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {

                    connection.Open();
                    using (SqlTransaction trans = connection.BeginTransaction())
                    {
                        if (selectedOption == "P")
                        {
                            DynamicParameters param = new DynamicParameters();
                            param.Add("@P_TotalKitta", data.TotalKitta);
                            param.Add("@P_FractionKitta", data.FractionKitta);
                            param.Add("@P_CompCode", data.CompanyId);
                            param.Add("@P_HolderName", data.HolderName);
                            param.Add("@P_HolderNo", data.HolderNo);
                            param.Add("@P_Address", data.Address);
                            param.Add("@Total", data.Total);
                            param.Add("@Action", data.Action);
                            response = connection.Query<JsonResponse>(sql: "ADD_CALCULATION_DIVMASTER_DIR", param: param, transaction: trans, commandType: CommandType.StoredProcedure).FirstOrDefault();
                            if (response.IsSuccess == true)
                            {
                                trans.Commit();
                            }
                            else
                            {
                                trans.Rollback();
                            }
                            return response;
                        }
                        else
                        {
                            DynamicParameters param = new DynamicParameters();
                            param.Add("@P_TotalKitta", data.TotalKitta);
                            param.Add("@P_FractionKitta", data.FractionKitta);
                            param.Add("@P_CompCode", data.CompanyId);
                            param.Add("@P_HolderName", data.HolderName);
                            param.Add("@P_BOID", data.Boid);
                            param.Add("@P_Address", data.Address);
                            param.Add("@Total", data.Total);
                            param.Add("@Action", data.Action);
                            response = connection.Query<JsonResponse>(sql: "ADD_CALCULATION_DIVMASTER_CDS_DIR", param: param, transaction: trans, commandType: CommandType.StoredProcedure).FirstOrDefault();
                            if (response.IsSuccess == true)
                            {
                                trans.Commit();
                            }
                            else
                            {
                                trans.Rollback();
                            }
                            return response;
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }

        //public JsonResponse GetAllCalclationData(string selectedOption, int? pageNo, int? pageSize, out int TotalRecords)
        //{
        //    JsonResponse response = new JsonResponse();
        //    try
        //    {
        //        using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
        //        {
        //            connection.Open();
        //            DynamicParameters param = new DynamicParameters();
        //            param.Add("P_SelType", selectedOption);
        //            param.Add("P_PageNo", pageNo);
        //            param.Add("P_PageSize", pageSize);
        //            param.Add("@P_Count", dbType: DbType.Int32, direction: ParameterDirection.Output);
        //            var lists = connection.Query<ATTCalculation>(sql: "GetAllCalculationData", param: param, null, commandType: CommandType.StoredProcedure).ToList();
        //            TotalRecords = param.Get<Int32>("@P_Count");
        //            response.IsSuccess = true;
        //            response.ResponseData = lists;
        //            return response;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        response.IsSuccess = false;
        //        response.HasError = true;
        //        response.ResponseData = ex;
        //    }
        //}

        public List<ATTCalculation> GetAllCalclationData(string selectedOption, string CompanyId, int? pageNo, int? pageSize, out int TotalRecords)
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
                {

                    DynamicParameters param = new DynamicParameters();
                    param.Add("P_SelType", selectedOption);
                    param.Add("P_CompanyId", CompanyId);
                    param.Add("P_PageNo", pageNo);
                    param.Add("P_PageSize", pageSize);
                    param.Add("@P_Count", dbType: DbType.Int32, direction: ParameterDirection.Output);
                    List<ATTCalculation> list = SqlMapper.Query<ATTCalculation>(
                                    connection, "[dbo].[GetAllCalculationData]", param, commandType: CommandType.StoredProcedure).ToList();
                    TotalRecords = param.Get<Int32>("@P_Count");

                    return list;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        // this is optional while saving from excel 
        public JsonResponse SaveCalculationFromExcel(List<ATTCalculation> data, string selectedOption)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {

                    connection.Open();
                    using (SqlTransaction trans = connection.BeginTransaction())
                    {
                        foreach (var item in data)
                        {
                            if (selectedOption == "P")
                            {
                                DynamicParameters param = new DynamicParameters();
                                param.Add("@P_TotalKitta", item.TotalKitta);
                                param.Add("@P_FractionKitta", item.FractionKitta);
                                param.Add("@P_CompCode", item.CompanyId);
                                param.Add("@P_HolderName", item.HolderName);
                                param.Add("@P_HolderNo", item.HolderNo);
                                param.Add("@P_Address", item.Address);
                                param.Add("@Total", item.Total);
                                param.Add("@Action", 'A');
                                response = connection.Query<JsonResponse>(sql: "dbo.ADD_CALCULATION_DIVMASTER_DIR", param: param, transaction: trans, commandType: CommandType.StoredProcedure).FirstOrDefault();
                            }
                            else
                            {
                                DynamicParameters param = new DynamicParameters();
                                param.Add("@P_TotalKitta", item.TotalKitta);
                                param.Add("@P_FractionKitta", item.FractionKitta);
                                param.Add("@P_CompCode", item.CompanyId);
                                param.Add("@P_HolderName", item.HolderName);
                                param.Add("@P_BOID", item.Boid);
                                param.Add("@P_Address", item.Address);
                                param.Add("@Total", item.Total);
                                param.Add("@Action", 'A');
                                response = connection.Query<JsonResponse>(sql: "dbo.ADD_CALCULATION_DIVMASTER_CDS_DIR", param: param, transaction: trans, commandType: CommandType.StoredProcedure).FirstOrDefault();
                            }
                        }
                        if (response.IsSuccess == true)
                        {
                            trans.Commit();
                        }
                        else
                        {
                            trans.Rollback();
                        }
                        return response;
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

        }





        public JsonResponse Calculate(/*List<ATTCalculation> data,*/ string selectedOption, string Bonus, string Divident,string compcode)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (SqlTransaction trans = connection.BeginTransaction())
                    {
                                
                                DynamicParameters param = new DynamicParameters();
                                param.Add("@P_SelType", selectedOption);
                                param.Add("@P_CompCode", compcode);
                                param.Add("@Bonus", Bonus);
                                param.Add("@Divident", Divident);
                                //param.Add("@FractionKitta", item.FractionKitta);
                                //param.Add("@TotShKitta", item.TotalKitta);
                                //param.Add("@SHHolderNo", item.HolderNo);
                                //param.Add("@BoidNo", item.Boid);
                                response = connection.Query<JsonResponse>(sql: "dbo.Calculate_Divident_Bonus", param: param, transaction: trans, commandType: CommandType.StoredProcedure).FirstOrDefault();
                        if (response.IsSuccess == true)
                        {
                            DynamicParameters tparams = new DynamicParameters();
                            tparams.Add("@P_SelType", selectedOption);
                            tparams.Add("@P_CompCode", compcode);
                            tparams.Add("@Bonus", Bonus);
                            tparams.Add("@Divident", Divident);
                            var iscalculated= connection.Query<JsonResponse>(sql: "dbo.Update_IsClaculated", param: tparams, transaction: trans, commandType: CommandType.StoredProcedure).FirstOrDefault();
                            trans.Commit();
                        }
                        else
                        {
                            trans.Rollback();
                        }
                        return response;
                    }
                }
                catch (Exception ex)
                {
                    response.Message=ex.Message;
                    response.IsSuccess = false;
                    return response;
                }
            }

        }


        public  JsonResponse BulkCopyCalculationData(DataSet ds, DataTable dt, string selectedOption)
        {
            JsonResponse response = new JsonResponse();
            JsonResponse CheckResponse = new JsonResponse();
            var sourcearrayNames = (from DataColumn x
              in dt.Columns.Cast<DataColumn>()
                                    select x.ColumnName).ToArray();
            string tableName = "";
            if (selectedOption == "P")
            {
                tableName = "DivMaster_Dirty";
            }
            else {
                tableName = "DivMasterCDS_Dirty";
            }
            try
            {

                using (SqlConnection con = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
                {
                    con.Open();
                    CheckResponse = CheckUploadedExcel(dt, selectedOption);
                    if (CheckResponse.IsSuccess == true)
                    {
                        string sqlquery = "SELECT Column_Name FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_CATALOG = '" + con.Database + "' AND Table_name = '" + tableName + "'";
                        List<ATTCalculation> soucecolumn = con.Query<ATTCalculation>(sqlquery, null, commandType: null)?.ToList();

                        using (SqlTransaction trans = con.BeginTransaction())
                        {
                            using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(con, SqlBulkCopyOptions.KeepIdentity, trans))
                            {
                                if (tableName != "")
                                {
                                    //Set the database table name.
                                    sqlBulkCopy.DestinationTableName = "dbo." + tableName;

                                    string colname = string.Empty;
                                    ATTCalculation emptycolumn = new ATTCalculation();
                                    emptycolumn.Column_Name = String.Empty;
                                    foreach (DataColumn col in dt.Columns)
                                    {
                                        colname = (soucecolumn.Find(x => x.Column_Name.ToLower() == col.ColumnName.ToLower()) ?? emptycolumn).Column_Name;
                                        if (colname != String.Empty)
                                        {
                                            sqlBulkCopy.ColumnMappings.Add(col.ColumnName.ToLower(), colname);
                                            colname = string.Empty;
                                        }
                                    }
                                    sqlBulkCopy.BulkCopyTimeout = 0;
                                    sqlBulkCopy.WriteToServer(dt);
                                    trans.Commit();
                                    response.IsSuccess = true;
                                    response.Message = "Excel Successfully Uploaded!!!! Please Proceed Further";
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
                response.IsSuccess = false;
                throw ex;
            }
            return response;
        }



        private JsonResponse CheckUploadedExcel(DataTable dt,string selectedOption) 
        {
            JsonResponse response = new JsonResponse();
            try
            {
                foreach (DataRow row in dt.Rows)
                {
                    ATTCalculation obj = new ATTCalculation();
                    if (selectedOption == "P")
                    {
                        obj.HolderNo = string.IsNullOrEmpty(row["shHolderNo"].ToString()) ? 0 : Int32.Parse(row["shHolderNo"].ToString());
                        obj.HolderName = row["HolderName"].ToString();
                        obj.Address = row["Address"].ToString();
                        obj.TotalKitta = row["TotShKitta"].ToString();
                        obj.FractionKitta = string.IsNullOrEmpty(row["FractionKitta"].ToString()) ? 0 : decimal.Parse(row["FractionKitta"].ToString(), System.Globalization.NumberStyles.Float);
                        obj.Total = string.IsNullOrEmpty(row["Total"].ToString()) ? 0 : decimal.Parse(row["Total"].ToString(), System.Globalization.NumberStyles.Float);
                        if (obj.HolderNo == 0) {
                            throw new Exception("HolderNo cannot be null or 0!!!");
                        }
                        decimal fractotal = decimal.Parse(obj.TotalKitta) + obj.FractionKitta;
                        //if (fractotal != obj.Total) {
                        //    throw new Exception($"Total kitta doesn't match in holderno {obj.HolderNo} !!!");
                        //}
                        response.IsSuccess = true;
                    }
                    else
                    {
                        obj.Boid = row["BO_idno"].ToString();
                        obj.HolderName = row["fullname"].ToString();
                        obj.Address = row["Address"].ToString();
                        obj.TotalKitta = row["TotShKitta"].ToString();
                        obj.FractionKitta = string.IsNullOrEmpty(row["FractionKitta"].ToString()) ? 0 : decimal.Parse(row["FractionKitta"].ToString(), System.Globalization.NumberStyles.Float);
                        obj.Total = string.IsNullOrEmpty(row["Total"].ToString()) ? 0 : decimal.Parse(row["Total"].ToString(), System.Globalization.NumberStyles.Float);
                        if (obj.Boid == "" || obj.Boid == "0")
                        {
                            throw new Exception("Boid cannot be null or 0!!!");
                        }
                        decimal fractotal = decimal.Parse(obj.TotalKitta) + obj.FractionKitta;
                        //if (fractotal != obj.Total)
                        //{
                        //    throw new Exception($"Total kitta doesn't match in BOID {obj.Boid} !!!");
                        //}
                        response.IsSuccess = true;
                    }
                }
            }
            catch (Exception ex)
            {
                response.IsSuccess = false;
                response.Message = ex.Message;
                throw;
            }
            return response;
        }
    }
}
