using Dapper;
using Entity.Common;
using Entity.DakhilTransfer;
using Interface.DakhilTransfer;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.DakhilTransfer
{
    public class DakhilShareTransferRepo : IDakhilShareTransfer
    {
        IOptions<ReadConfig> _connectionString;

        public DakhilShareTransferRepo(IOptions<ReadConfig> connectionString)
        {
            this._connectionString = connectionString;
        }

        public JsonResponse GetIndividualShareTransferList(string CompCode, string RegNo, string BHolderNo, string UserName, string IPAddress)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (SqlTransaction tran = connection.BeginTransaction())
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@P_CompCode", CompCode);
                        parameters.Add("@P_RegNo", RegNo);
                        parameters.Add("@P_BHolderNo", BHolderNo);
                        parameters.Add("@P_UserName", UserName);
                        parameters.Add("@P_IPAddress", IPAddress);
                        parameters.Add("@P_EntryDate", DateTime.Now);
                        List<ATTShareDakhilTransfer> aTTDakhilTransfers = connection.Query<ATTShareDakhilTransfer>(sql: "GET_SHARE_TRANSFER_LIST_INDIVIDUAL",
                            param: parameters, transaction: tran, commandType: CommandType.StoredProcedure)?.ToList();
                        if (aTTDakhilTransfers.Count > 0)
                        {
                            response.IsSuccess = true;
                            response.ResponseData = aTTDakhilTransfers;
                            response.TotalRecords = aTTDakhilTransfers.Count;
                        }
                        else
                        {
                            response.Message = ATTMessages.NO_RECORDS_FOUND;
                        }
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

        public JsonResponse GetShareTransferList(string CompCode, string RegNoFrom, string RegNoTo, string DateFrom, string DateTo, string UserName, string IPAddress)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (SqlTransaction tran = connection.BeginTransaction())
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@P_CompCode", CompCode);
                        parameters.Add("@P_RegNoFrom", RegNoFrom);
                        parameters.Add("@P_RegNoTo", RegNoTo);
                        parameters.Add("@P_DateFrom", DateFrom);
                        parameters.Add("@P_DateTo", DateTo);
                        parameters.Add("@P_UserName", UserName);
                        parameters.Add("@P_EntryDate", DateTime.Now);
                        List<ATTShareDakhilTransfer> aTTDakhilTransfers = connection.Query<ATTShareDakhilTransfer>(sql: "CHECK_SHARE_TRANSFER_LIST_FOR_DUPLICATE",
                            param: parameters, transaction: tran, commandType: CommandType.StoredProcedure)?.ToList();
                        string message = string.Empty;
                        if (aTTDakhilTransfers.Count > 0)
                        {
                            var data = from item in aTTDakhilTransfers
                                       select "RegNo: " + item.RegNo + "  CertNo: " + item.CertNo;
                            message = string.Join("\n", data);
                        }
                        response.Message = message;
                        parameters.Add("@P_IPAddress", IPAddress);

                        aTTDakhilTransfers = connection.Query<ATTShareDakhilTransfer>(sql: "GET_SHARE_TRANSFER_LIST_2",
                            param: parameters, transaction: tran, commandType: CommandType.StoredProcedure)?.ToList();
                        if (aTTDakhilTransfers.Count > 0)
                        {
                            if (aTTDakhilTransfers[0].ErrorMessage == null)
                            {
                                response.IsSuccess = true;
                                response.ResponseData = aTTDakhilTransfers;
                                response.TotalRecords = aTTDakhilTransfers.Count;
                            }
                            else
                            {
                                response.Message = aTTDakhilTransfers[0].ErrorMessage;
                            }

                        }
                        else
                        {
                            response.Message = ATTMessages.NO_RECORDS_FOUND;
                        }
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

        public JsonResponse SaveShareTransfer(string CompCode, List<ATTShareDakhilTransfer> aTTShareDakhilTransfers, string UserName, string IPAddress, string TransferedDate, string SelectedAction, string FolioNo = null, string BatchNo = null)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (SqlTransaction tran = connection.BeginTransaction())
                    {
                        DataTable dt = new DataTable();

                        dt.Columns.Add("BuyerHolderNo");
                        dt.Columns.Add("RegNo");

                        aTTShareDakhilTransfers.ForEach(x => dt.Rows.Add(x.BHolderNo, x.RegNo));
                        SqlCommand cmd = new SqlCommand("DAKHIL_TRANSFER_COMPLETE_POSTING", connection);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Transaction = tran;
                        SqlParameter param = cmd.Parameters.AddWithValue("@udt", dt);
                        param = cmd.Parameters.AddWithValue("@P_COMPCODE", CompCode);
                        param = cmd.Parameters.AddWithValue("@P_EntryDate", DateTime.Now);
                        param = cmd.Parameters.AddWithValue("@P_IPAddress", IPAddress);
                        param = cmd.Parameters.AddWithValue("@P_TransferedDate", TransferedDate);
                        param = cmd.Parameters.AddWithValue("@P_FolioNo", FolioNo);
                        param = cmd.Parameters.AddWithValue("@P_BatchNo", BatchNo);
                        param = cmd.Parameters.AddWithValue("@P_USERNAME", UserName);
                        param = cmd.Parameters.AddWithValue("@P_SELECTED_ACTION", SelectedAction);
                        param.Direction = ParameterDirection.Input;
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (reader.GetString(0) == "1")
                                {
                                    response.IsSuccess = true;
                                    response.Message = SelectedAction == "A" ? ATTMessages.DAKHIL_TRANSFER.POSTING_SUCCESS : ATTMessages.DAKHIL_TRANSFER.CANCEL_SUCCESS;
                                }
                                else
                                {
                                    response.Message = ATTMessages.DAKHIL_TRANSFER.FAILURE;
                                }
                            }
                        }
                        if (response.IsSuccess)
                            tran.Commit();
                        else
                            tran.Rollback();
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
