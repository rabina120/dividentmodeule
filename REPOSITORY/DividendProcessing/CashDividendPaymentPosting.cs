
using Dapper;
using Entity.Common;
using Entity.Dividend;
using Entity.ShareHolder;
using Interface.DividendProcessing;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.DividendProcessing
{
    public class CashDividendPaymentPosting : ICashDividendPaymentPosting
    {
        IOptions<ReadConfig> connectionString;

        public CashDividendPaymentPosting(IOptions<ReadConfig> _connectionString)
        {
            this.connectionString = _connectionString;
        }
        public JsonResponse GetCashDividendPaymentForApproval(string CompCode, string Divcode)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    ATTDividend aTTDividend = connection.Query<ATTDividend>(sql: "Select * from DivTable where compcode ='" + CompCode + "' and divcode='" + Divcode + "' and Divcalculated=1", null, commandType: null)?.FirstOrDefault();
                    if (aTTDividend != null)
                    {
                        DynamicParameters param = new DynamicParameters();
                        param.Add("@CompCode", CompCode);
                        param.Add("@TableName1", aTTDividend.tablename1);

                        List<ATTCashDividend> aTTCashDividend = SqlMapper.Query<ATTCashDividend, ATTShHolder, ATTCashDividend>(connection, sql: "Get_CashDividendPayment_ForApproval",
                                (attCashDividend, attShHolder) =>
                                {
                                    attCashDividend.attShholder = attShHolder;
                                    return attCashDividend;
                                },
                                param: param, null, splitOn: "SpHolder", commandType: CommandType.StoredProcedure)?.ToList();
                        if (aTTCashDividend.Count > 0)
                        {
                            jsonResponse.Message = aTTDividend.tablename1;
                            jsonResponse.ResponseData = aTTCashDividend;
                            jsonResponse.IsSuccess = true;
                        }
                        else
                        {
                            jsonResponse.Message = "Record Not Found";
                        }
                    }
                }
                catch (Exception ex)
                {
                    jsonResponse.Message = ex.Message;
                }
            }
            return jsonResponse;
        }

        public JsonResponse PostCashDividendPaymentRequest(List<ATTCashDividend> aTTCashDividends, string ActionType, string UserName)
        {
            JsonResponse jsonResponse = new JsonResponse();

            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    using (SqlTransaction trans = connection.BeginTransaction())
                    {
                        try
                        {
                            foreach (ATTCashDividend aTTCashDividend in aTTCashDividends)
                            {
                                DynamicParameters param = new DynamicParameters();
                                param.Add("@P_COMPCODE", aTTCashDividend.compcode);
                                param.Add("@TableName1", aTTCashDividend.WTable1);
                                param.Add("@mdate", aTTCashDividend.wissue_app_date);
                                param.Add("@GUserName", UserName);
                                param.Add("@remarks", aTTCashDividend.wissue_auth_remarks);
                                param.Add("@P_SHHOLDERNO", (aTTCashDividend.ShHolderNo));
                                param.Add("@P_Action", ActionType);
                                param.Add("@Warrantno", aTTCashDividend.WarrantNo);

                                jsonResponse = connection.Query<JsonResponse>("Cash_Dividend_Payment_Posting", param, trans, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                            }
                            trans.Commit();
                        }
                        catch (Exception ex)
                        {
                            trans.Rollback();
                            jsonResponse.Message = ex.Message;
                        }
                    }
                }
                catch (Exception ex)
                {
                    jsonResponse.Message = ex.Message;
                }
            }
            return jsonResponse;
        }
    }
}
