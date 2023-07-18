
using Dapper;

using Entity.Common;
using Entity.Dividend;
using Interface.DividendProcessing;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.DividendProcessing
{
    public class DemateDividend : IDemateDividend
    {
        IOptions<ReadConfig> _connectionString;
        public DemateDividend(IOptions<ReadConfig> connectionString)
        {
            _connectionString = connectionString;
        }

        public JsonResponse GetDemateDividendInformation(string CompCode, string DivCode, string shholderno, string a, string action,string Username,string IPADDRESS)
        {
            //JsonResponse jsonResponse = new JsonResponse();
            //using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            //{
            //    ATTCashDividend cashDividendInformation;

            //    try
            //    {
            //        connection.Open();

            //        string tableName = connection.Query<string>(sql: "SELECT tablename2 FROM DIVTABLE WHERE  DIVCODE = " + DivCode + " and DIVCALCULATED = 1 and COMPCODE='" + CompCode + "'", param: null, commandType: null).FirstOrDefault();
            //        DynamicParameters param = new DynamicParameters();
            //        param.Add("@P_TABLENAME", tableName);
            //        param.Add("@P_SHHOLDERNO", shholderno);
            //        if (a == "S")
            //        {
            //            param.Add("@P_ACTION", "S");
            //            cashDividendInformation = SqlMapper.Query<ATTCashDividend, ATTShHolder, ATTCashDividend>(connection, sql: "GET_HOLDER_DEMATE_DIVIDEND_INFORMATION",
            //           (cashDividendInformation, shHolder) =>
            //           {
            //               cashDividendInformation.attShholder = shHolder;
            //               return cashDividendInformation;
            //           },
            //           param: param, null, splitOn: "ShholderBind", commandType: CommandType.StoredProcedure)?.FirstOrDefault();
            //        }
            //        else
            //        {
            //            param.Add("@P_ACTION", "W");
            //            cashDividendInformation = SqlMapper.Query<ATTCashDividend, ATTShHolder, ATTCashDividend>(connection, sql: "GET_HOLDER_DEMATE_DIVIDEND_INFORMATION",
            //          (cashDividendInformation, shHolder) =>
            //          {
            //              cashDividendInformation.attShholder = shHolder;
            //              return cashDividendInformation;
            //          },
            //          param: param, null, splitOn: "ShholderBind", commandType: CommandType.StoredProcedure)?.FirstOrDefault();
            //        }


            //        if (cashDividendInformation != null)
            //        {
            //            if (cashDividendInformation.WIssued == true && cashDividendInformation.wissue_status == "POSTED" && cashDividendInformation.wissue_Approved == "Y")
            //            {
            //                jsonResponse.IsSuccess = false;
            //                jsonResponse.Message = "Dividend already issued and Approved";
            //            }
            //            else if (cashDividendInformation.WIssued == true && cashDividendInformation.wissue_Approved == "N")
            //            {
            //                if (action == "A")
            //                {
            //                    jsonResponse.IsSuccess = false;
            //                    jsonResponse.Message = "Dividend already issued but Unposted";
            //                }
            //                else
            //                {
            //                    jsonResponse.IsSuccess = true;
            //                    jsonResponse.ResponseData = cashDividendInformation;
            //                }

            //            }
            //            else if(cashDividendInformation.WIssued == false)
            //            {
            //                jsonResponse.IsSuccess = true;
            //                jsonResponse.ResponseData = cashDividendInformation;
            //            }


            //        }
            //        else
            //        {
            //            jsonResponse.IsSuccess = false;
            //            jsonResponse.Message = "Cannot Find The Holder!!";
            //        }

            //    }
            //    catch (Exception ex)
            //    {
            //        jsonResponse.Message = ex.Message;
            //    }
            //    return jsonResponse;
            //}
            throw new NotImplementedException();
        }

        public JsonResponse GetDemateDividendTableList(string compcode)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@CompCode", compcode);
                    string sql = "select * from divtable where(divtype<>2) and(tablename2 is not null) and compcode = '" + compcode + "'  order by divcode";
                    List<ATTDividend> aTTDividendTables = connection.Query<ATTDividend>(sql: sql, param, commandType: null).ToList();
                    if (aTTDividendTables.Count > 0)
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = aTTDividendTables;
                    }

                }
                catch (Exception ex)
                {

                    jsonResponse.ResponseData = ex;
                    jsonResponse.IsSuccess = false;
                    jsonResponse.HasError = true;

                }
                return jsonResponse;
            }
        }
    }
}
