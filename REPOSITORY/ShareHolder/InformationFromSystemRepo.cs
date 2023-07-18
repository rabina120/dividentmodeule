using Dapper;
using Entity.Common;
using Entity.Dividend;
using Entity.HolderInfo;
using Interface.ShareHolder;
using Microsoft.Extensions.Options;
using Repository.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace Repository.ShareHolder
{
    public class InformationFromSystemRepo : IInformationFromSystem
    {
        IOptions<ReadConfig> connectionString;

        public InformationFromSystemRepo(IOptions<ReadConfig> _connectionString)
        {
            this.connectionString = _connectionString;
        }
        public async Task<JsonResponse> GetInformationDetails(string ComCode, string DivCode, string ShareType)
        {
            JsonResponse jsonResponse = new JsonResponse();
            int? Lotno;
            string sql = null, rsql = null;
            DataTable dt = new DataTable();
            int i = 1;
            double? TotalAmt = 0, divtax = 0, bonustax = 0, bonusadj = 0;
            List<ATTHolderDemateInfo> aTTHolderDemate = new List<ATTHolderDemateInfo>();
            ATTHolderDemateInfo data;


            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_DivCode", DivCode);
                    param.Add("@P_COMPCODE", ComCode);
                    param.Add("@TableName1", ShareType);
                    string tableName = new TableReporsitory(connectionString).GetTableName(param);
                    if (tableName != null)
                    {
                        bool Output = CheckPostedUnPostedData(tableName, connection);
                        if (Output)
                        {
                            if (ShareType == "1")
                            {
                                int? Batchno = GetPhysicalMaxBatchNo(tableName, connection);
                                Lotno = (Batchno == null) ? 0 : Batchno;
                                Lotno = Lotno > 0 ? Lotno + 1 : 1;

                                param = new DynamicParameters();
                                param.Add("@P_TABLENAME", tableName);
                                List<ATTDivMaster> aTTDivMasters = connection.QueryAsync<ATTDivMaster>("LOAD_PHYSICAL_DEMATE_INFO", param, null, commandType: CommandType.StoredProcedure).Result?.ToList();

                                if (aTTDivMasters.Count > 0)
                                {

                                    dt.Columns.Add("ShHolderNo", typeof(int));
                                    dt.Columns.Add("Boid", typeof(string));
                                    dt.Columns.Add("WarrantNo", typeof(string));
                                    dt.Columns.Add("Fullname", typeof(string));
                                    dt.Columns.Add("Address", typeof(string));
                                    dt.Columns.Add("Contactno", typeof(string));
                                    dt.Columns.Add("BankCode", typeof(string));
                                    dt.Columns.Add("Bank Name", typeof(string));
                                    dt.Columns.Add("Bank Accno", typeof(string));
                                    dt.Columns.Add("Total", typeof(decimal));


                                    foreach (ATTDivMaster aTTDivMaster in aTTDivMasters)
                                    {
                                        param = new DynamicParameters();
                                        param.Add("@P_BOID", aTTDivMaster.boidno);
                                        ATTHolderDemateInfo aTTHolderDemateInfo = connection.QueryAsync<ATTHolderDemateInfo>("GET_DEMATE_INFO_BY_BOID", param: param, null, commandType: CommandType.StoredProcedure).Result?.FirstOrDefault();
                                        if (aTTHolderDemateInfo != null)
                                        {
                                            ATTRejection aTTRejection = connection.QueryAsync<ATTRejection>("GET_REJECTEDLIST_BY_BOID", param, null, commandType: CommandType.StoredProcedure).Result?.FirstOrDefault();

                                            if (aTTRejection != null)
                                            {
                                                continue;
                                            }
                                            else if (aTTRejection == null)
                                            {
                                                data = new ATTHolderDemateInfo();
                                                data.ShHolderNo = aTTDivMaster.ShHolderNo;
                                                data.Boid = aTTDivMaster.boidno;
                                                data.WarrantNo = aTTDivMaster.WarrantNo;
                                                data.Fullname = aTTHolderDemateInfo.Fullname;
                                                data.address = aTTHolderDemateInfo.address;
                                                data.contactno = aTTHolderDemateInfo.contactno;
                                                data.bankcode = aTTHolderDemateInfo.bankcode;
                                                data.bankname = aTTHolderDemateInfo.bankname;
                                                data.bankaccno = aTTHolderDemateInfo.bankaccno;
                                                TotalAmt = aTTDivMaster.WarrantAmt;
                                                divtax = aTTDivMaster.TaxDamt;
                                                bonustax = aTTDivMaster.bonustax;
                                                bonusadj = aTTDivMaster.bonusadj;

                                                data.Total = TotalAmt - divtax - bonustax - bonusadj;
                                                data.lot = aTTHolderDemateInfo.lot;
                                                //Add Rows in DataTable  

                                                dt.Rows.Add(i, aTTDivMaster.ShHolderNo, aTTDivMaster.boidno, aTTDivMaster.WarrantNo, aTTHolderDemateInfo.Fullname, aTTHolderDemateInfo.address, aTTHolderDemateInfo.contactno, aTTHolderDemateInfo.bankcode, aTTHolderDemateInfo.bankname, aTTHolderDemateInfo.bankaccno);
                                                i++;
                                                aTTHolderDemate.Add(data);
                                            }
                                        }
                                        else
                                        {
                                            jsonResponse.IsSuccess = false;
                                            jsonResponse.Message = "Sorry No Record Found ";
                                        }

                                    }
                                }
                                else
                                {
                                    jsonResponse.IsSuccess = false;
                                    jsonResponse.Message = "Sorry No Record Found ";
                                }
                            }
                            else if (ShareType == "2")
                            {
                                int? Batchno = GetPhysicalMaxBatchNo(tableName, connection);
                                Lotno = (Batchno == null) ? 0 : Batchno;
                                Lotno = Lotno > 0 ? Lotno + 1 : 1;

                                param = new DynamicParameters();
                                param.Add("@P_TABLENAME", tableName);
                                List<ATTDivMasterCDS> aTTDivMasterCDs = connection.Query<ATTDivMasterCDS>("LOAD__DEMATE_INFO", param, null, commandType: CommandType.StoredProcedure).ToList();
                                if (aTTDivMasterCDs.Count > 0)
                                {
                                    foreach (ATTDivMasterCDS aTTDiv in aTTDivMasterCDs)
                                    {
                                        param = new DynamicParameters();
                                        param.Add("@P_BOID", aTTDiv.BO_idno);
                                        ATTHolderDemateInfo aTTHolderDemateInfo = connection.Query<ATTHolderDemateInfo>("GET_DEMATE_INFO_BY_BOID", param: param, null, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                                        if (aTTHolderDemateInfo != null)
                                        {
                                            ATTRejection aTTRejection = connection.Query<ATTRejection>("GET_REJECTEDLIST_BY_BOID", param, null, commandType: CommandType.StoredProcedure)?.FirstOrDefault();

                                            if (aTTRejection != null)
                                            {
                                                continue;
                                            }
                                            else if (aTTRejection == null)
                                            {
                                                data = new ATTHolderDemateInfo();
                                                data.Boid = aTTDiv.BO_idno;
                                                data.WarrantNo = aTTDiv.warrantno;
                                                data.Fullname = aTTHolderDemateInfo.Fullname;
                                                data.address = aTTHolderDemateInfo.address;
                                                data.contactno = aTTHolderDemateInfo.contactno;
                                                data.bankcode = aTTHolderDemateInfo.bankcode;
                                                data.bankname = aTTHolderDemateInfo.bankname;
                                                data.bankaccno = aTTHolderDemateInfo.bankaccno;
                                                TotalAmt = aTTDiv.warrantamt;
                                                divtax = aTTDiv.taxdamt;
                                                bonustax = aTTDiv.bonustax;
                                                bonusadj = aTTDiv.bonusadj;
                                                data.Total = TotalAmt - divtax - bonustax - bonusadj;
                                                data.lot = aTTHolderDemateInfo.lot;
                                                aTTHolderDemate.Add(data);
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                jsonResponse.Message = "Physical or Demate mismatched";
                                return jsonResponse;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    jsonResponse.Message = ex.Message;
                    jsonResponse.IsSuccess = false;
                }

                connection.Execute("DELETE_DATA_FROM_TEMP", null, null, commandType: CommandType.StoredProcedure);

            }

            if (aTTHolderDemate.Count > 0)
            {
                jsonResponse.IsSuccess = true;
                jsonResponse.ResponseData = aTTHolderDemate;
            }


            return jsonResponse;
        }


        public bool CheckPostedUnPostedData(string TableName, SqlConnection connection)
        {
            bool Output = false;
            int? value;
            try
            {
                DynamicParameters param = new DynamicParameters();
                param.Add("@P_TABLENAME", TableName);
                value = connection.Query<int>("CheckPostedUnPosted_System_INFO", param, null, commandType: CommandType.StoredProcedure).FirstOrDefault();
                if (value == 0)
                {
                    Output = true;
                }
            }
            catch (Exception ex)
            {
                Output = false;
            }

            return Output;
        }


        public int? GetPhysicalMaxBatchNo(string TableName, SqlConnection connection)
        {
            int? Batchno;
            try
            {
                string sql = "Select max(batchno) from " + TableName;
                Batchno = connection.Query<int>(sql: sql, param: null, commandType: null).FirstOrDefault();
                if (Batchno == 0)
                {
                    Batchno = 0;
                }

            }
            catch (Exception ex)
            {
                Batchno = null;
            }
            return Batchno;
        }

        public void SaveTempData(string CompCode, SqlConnection connection)
        {

            connection.Execute("DELETE_DATA_FROM_TEMP", null, null, commandType: CommandType.StoredProcedure);
            DynamicParameters param = new DynamicParameters();
            param.Add("@P_COMPCODE", CompCode);
            connection.Query<int>(sql: "SAVE_TEMP_DATA", param, null, commandType: CommandType.StoredProcedure);


        }

        public void DeleteTempData(string CompCode, SqlConnection connection)
        {

            connection.Execute("DELETE_DATA_FROM_TEMP", null, null, commandType: CommandType.StoredProcedure);

        }

        public async Task<List<ATTDirtyInfromationFromSystem>> LoadDataTable(ATTDataListRequest request)
        {
            try
            {
                using (var connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
                {
                    connection.Open();
                    try
                    {

                        var parameters = new DynamicParameters();
                        parameters.Add("SearchValue", request.SearchValue.Trim(), DbType.String);
                        parameters.Add("PageNo", request.PageNo, DbType.Int32);
                        parameters.Add("PageSize", request.PageSize, DbType.Int32);
                        parameters.Add("SortColumn", request.SortColumn, DbType.Int32);
                        parameters.Add("SortDirection", request.SortDirection, DbType.String);


                        List<ATTDirtyInfromationFromSystem> dirtyInfromationFromSystems = (await connection.QueryAsync<ATTDirtyInfromationFromSystem>("GET_DATA_FROM_DIRTY_INFORMATION_FROM_SYSTEM", parameters, commandType: CommandType.StoredProcedure)).ToList();
                        return dirtyInfromationFromSystems;
                    }
                    catch (Exception ex)
                    {
                        connection.Query("DELETE_DIRTY_INFROMATION_FROM_SYSTEM", null, null, commandType: CommandType.StoredProcedure);
                        throw new Exception(ex.Message, ex);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }

        }

        public async Task UpdateDivTable(string tableName, string UserName, string ActionType, string LotNo, string ShHolderNo, string WarrantNo, string BankAddress, string bankname, string bankaccno)
        {
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    using (SqlTransaction transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            DynamicParameters param = new DynamicParameters();
                            param.Add("@TableName", tableName);
                            param.Add("@ActionType", ActionType);
                            param.Add("@P_ShHolderNo", ShHolderNo);
                            param.Add("@P_WarrantNo", WarrantNo);
                            param.Add("@P_BANKADDRESS", BankAddress);
                            param.Add("@P_BANKNAME", bankname);
                            param.Add("@P_BANKACCNO", bankaccno);
                            param.Add("@P_LotNo", LotNo);
                            param.Add("@P_UserName", UserName);

                            await connection.ExecuteAsync("UPDATE_DIV_TABLE", param, transaction, commandType: CommandType.StoredProcedure);
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                        }
                    }

                }
                catch (Exception ex)
                {

                }
            }
        }

        public JsonResponse GetAllData(string ComCode, string DivCode, string ShareType, string UserName)
        {
            JsonResponse response = new JsonResponse();

            try
            {
                using (var connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
                {
                    connection.Open();

                    try
                    {

                        List<ATTDirtyInfromationFromSystem> dirtyInfromationFromSystems = connection.Query<ATTDirtyInfromationFromSystem>("SELECT_DIRTY_INFROMATION_FROM_SYSTEM", null, commandType: CommandType.StoredProcedure)?.ToList();
                        if (dirtyInfromationFromSystems.Count > 0)
                        {
                            using (SqlTransaction transaction = connection.BeginTransaction())
                            {
                                try
                                {

                                    DynamicParameters param = new DynamicParameters();
                                    param.Add("@P_DivCode", DivCode);
                                    param.Add("@P_CompCode", ComCode);
                                    param.Add("@P_ShareType", ShareType);
                                    param.Add("@P_UserName", UserName);
                                    JsonResponse resp = connection.Query<JsonResponse>("SAVE_INFORMATION_FROM_SYSTEM", param, transaction, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                                    if (resp.IsSuccess)
                                    {
                                        transaction.Commit();
                                        response.IsSuccess = true;
                                        response.Message = "Data Successfully Saved !!!";
                                        response.ResponseData = dirtyInfromationFromSystems;
                                    }
                                    else
                                    {
                                        transaction.Rollback();
                                        //connection.Query("DELETE_DIRTY_INFROMATION_FROM_SYSTEM", null, null, commandType: CommandType.StoredProcedure);
                                        response.Message = resp.Message;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    transaction.Rollback();
                                    //connection.Query("DELETE_DIRTY_INFROMATION_FROM_SYSTEM", null, null, commandType: CommandType.StoredProcedure);
                                    response.Message = ex.Message;
                                }
                            }

                        }
                        else
                        {
                            //connection.Query("DELETE_DIRTY_INFROMATION_FROM_SYSTEM", null, null, commandType: CommandType.StoredProcedure);
                            response.Message = "Cannot Find Any Records !!!";
                        }

                    }
                    catch (Exception ex)
                    {
                        //connection.Query("DELETE_DIRTY_INFROMATION_FROM_SYSTEM", null, null, commandType: CommandType.StoredProcedure);
                        response.Message = ex.Message;
                    }
                }

            }
            catch (Exception ex)
            {
                response.Message = ex.Message;
            }
            return response;
        }

        public JsonResponse DeleteDirtyTableData()
        {
            JsonResponse response = new JsonResponse();

            try
            {
                using (var connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
                {
                    connection.Open();
                    connection.Execute("DELETE_DIRTY_INFROMATION_FROM_SYSTEM", null, null, commandType: CommandType.StoredProcedure);

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
}
