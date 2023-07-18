using Dapper;
using Entity.Common;
using Entity.ShareHolder;
using ENTITY.ShareHolder;
using Interface.ShareHolder;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.ShareHolder
{
    public class ShareTransactionBookRepo: IShareTransactionBook
    {
        IOptions<ReadConfig> connectionString;
        public ShareTransactionBookRepo(IOptions<ReadConfig> _connectionString)
        {
            connectionString = _connectionString;
        }
        public JsonResponse GetShareHolderTransactionBook(string CompCode, string SHNumber, string UserName, string IPAddress)
        {
            JsonResponse response = new JsonResponse();
            List<ATTSHTransactionBook> transactionBook = new List<ATTSHTransactionBook>();
            List<ATTShHolder> HolderInfo = new List<ATTShHolder>();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand("GetShareTransactionBook", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter param = cmd.Parameters.AddWithValue("@p_shholderno", SHNumber);
                    param = cmd.Parameters.AddWithValue("@p_CompCode", CompCode);
                    param = cmd.Parameters.AddWithValue("@P_USERNAME", UserName);
                    param = cmd.Parameters.AddWithValue("@P_IP_ADDRESS", IPAddress);
                    param = cmd.Parameters.AddWithValue("@P_ENTRY_DATE", DateTime.Now);
                    param.Direction = ParameterDirection.Input;
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        var parser = reader.GetRowParser<ATTSHTransactionBook>(typeof(ATTSHTransactionBook));

                        while (reader.Read())
                        {
                            ATTSHTransactionBook obj = parser(reader);
                            transactionBook.Add(obj);
                        }
                        if (reader.NextResult())
                        {
                            var rowCount = 0;

                            while (reader.Read())
                            {
                                ATTShHolder sh = new ATTShHolder();
                                sh.FName = reader["FName"].ToString();
                                sh.NpName = reader["NpName"].ToString();
                                sh.Address = reader["Address"].ToString();
                                sh.Address1 = reader["Address1"].ToString();
                                sh.ordinarykitta = reader["ordinarykitta"].ToString();
                                sh.bonuskitta = reader["bonuskitta"].ToString();
                                sh.rightkitta = reader["rightkitta"].ToString();
                                sh.TotalKitta = int.Parse(reader["totalkitta"].ToString());
                                HolderInfo.Add(sh);
                            }
                        }
                    }
                    response.ResponseData = transactionBook;
                    response.ResponseData2 = HolderInfo[0];
                    response.IsSuccess = true;

                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;
                    response.IsSuccess = false;
                    response.HasError = true;
                }

                return response;
            }
        }
        public JsonResponse GetShareTypes(string CompCode, string SHNumber, string ShareType, string UserName, string IPAddress)
        {
            JsonResponse response = new JsonResponse();
            List<ATTSHTransactionBook> transactionBook = new List<ATTSHTransactionBook>();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    DynamicParameters param = new DynamicParameters();
                    param.Add("@p_sharetype", ShareType);
                    param.Add("@p_shholderno", SHNumber);
                    param.Add("@p_CompCode", CompCode);
                    param.Add("@P_USERNAME", UserName);
                    param.Add("@P_IP_ADDRESS", IPAddress);
                    param.Add("@P_ENTRY_DATE", DateTime.Now);
                    transactionBook = connection.Query<ATTSHTransactionBook>("GetCertDetailForShareTransactionBook", param, commandType: CommandType.StoredProcedure).ToList();
                    response.ResponseData = transactionBook;
                    response.IsSuccess = true;
                }
                catch(Exception ex)
                {
                    response.IsSuccess = false;
                    response.HasError = true;
                    response.Message = ex.Message;
                }
            }
            return response;
        }
        public JsonResponse GetPurchaseSalesReport(string CompCode, string SHNumber, string ShareType, string UserName, string IPAddress, string FileType)
        {
            JsonResponse response = new JsonResponse();
            //List<ATTSHTransactionBook> transactionBook = new List<ATTSHTransactionBook>();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    if(FileType == "E")
                    {
                        SqlCommand cmd = new SqlCommand("GetPurchaseSellForShareTransactionBook", connection);
                        cmd.CommandType = CommandType.StoredProcedure;
                        SqlParameter excelParam = new SqlParameter();
                        excelParam = cmd.Parameters.AddWithValue("@p_datatype", ShareType);
                        excelParam = cmd.Parameters.AddWithValue("@p_shholderno", SHNumber);
                        excelParam = cmd.Parameters.AddWithValue("@p_CompCode", CompCode);
                        excelParam = cmd.Parameters.AddWithValue("@P_USERNAME", UserName);
                        excelParam = cmd.Parameters.AddWithValue("@P_IP_ADDRESS", IPAddress);
                        excelParam = cmd.Parameters.AddWithValue("@P_ENTRY_DATE", DateTime.Now);
                        excelParam.Direction = ParameterDirection.Input;
                        DataSet ds = new DataSet("Data");
                        SqlDataAdapter da = new SqlDataAdapter();
                        da.SelectCommand = cmd;

                        da.Fill(ds);

                        if (ds.Tables[0].Rows.Count == 0)
                        {
                            response.IsSuccess = false;
                            response.Message = ATTMessages.NO_RECORDS_FOUND;
                        }
                        else
                        {
                            response.IsSuccess = true;
                            response.ResponseData = ds;
                        }

                    }
                    else if (FileType == "P")
                    {
                        DynamicParameters param = new DynamicParameters();
                        param.Add("@p_datatype", ShareType);
                        param.Add("@p_shholderno", SHNumber);
                        param.Add("@p_CompCode", CompCode);
                        param.Add("@P_USERNAME", UserName);
                        param.Add("@P_IP_ADDRESS", IPAddress);
                        param.Add("@P_ENTRY_DATE", DateTime.Now);
                       List<dynamic> transactionBook = connection.Query<dynamic>("GetPurchaseSellForShareTransactionBook", param, commandType: CommandType.StoredProcedure).ToList();

                        if (transactionBook.Count() == 0)
                        {
                            response.ResponseData = transactionBook;
                            response.IsSuccess = false;
                            response.Message = ATTMessages.NO_RECORDS_FOUND;
                        }
                        else
                        {
                            response.ResponseData = transactionBook;
                            response.IsSuccess = true;

                        }

                    }
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.HasError = true;
                    response.Message = ex.Message;
                }
            }
            return response;
        }
    }
}
