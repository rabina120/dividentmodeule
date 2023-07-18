
using Dapper;

using Entity.Common;
using Entity.Dividend;
using Interface.DividendProcessing;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Repository.DividendProcessing
{
    public class CashDematePaymentPosting : ICashDematePaymentPosting
    {
        IOptions<ReadConfig> connectionString;
        public CashDematePaymentPosting(IOptions<ReadConfig> _connectionString)
        {
            this.connectionString = _connectionString;
        }
        public JsonResponse GetCashDematePaymentForApproval(string CompCode, string Divcode)
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
                        param.Add("@TableName2", aTTDividend.tablename2);

                        List<ATTDivMasterCDS> aTTDivMasterCDs = SqlMapper.Query<ATTDivMasterCDS>(connection, sql: "Get_CashDematePayment_ForApproval", param: param, null, commandType: CommandType.StoredProcedure)?.ToList();
                        if (aTTDivMasterCDs.Count > 0)
                        {
                            jsonResponse.Message = aTTDividend.tablename2;
                            jsonResponse.ResponseData = aTTDivMasterCDs;
                            jsonResponse.IsSuccess = true;
                        }
                        else
                        {
                            jsonResponse.Message = "No Record Found";
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

        public JsonResponse PostCashDematePaymentRequest(List<ATTDivMasterCDS> attDivMasterCDS, string ActionType, string UserName)
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


                            foreach (ATTDivMasterCDS aTTDivMasterCDS in attDivMasterCDS)
                            {
                                DynamicParameters param = new DynamicParameters();
                                param.Add("@P_COMPCODE", aTTDivMasterCDS.Compcode);
                                param.Add("@TableName2", aTTDivMasterCDS.WTable2);
                                param.Add("@mdate", aTTDivMasterCDS.wissue_app_date);
                                param.Add("@GUserName", UserName);
                                param.Add("@remarks", aTTDivMasterCDS.wissue_auth_remarks);
                                param.Add("@P_Action", ActionType);
                                param.Add("@P_BO_idno", aTTDivMasterCDS.BO_idno);
                                param.Add("@Warrantno", aTTDivMasterCDS.warrantno);

                                jsonResponse = connection.Query<JsonResponse>("CASH_DEMATE_ISSUE_POSTING", param, trans, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
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
