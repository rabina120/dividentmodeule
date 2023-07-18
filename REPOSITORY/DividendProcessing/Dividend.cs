
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
    public class Dividend : IDividend
    {
        IOptions<ReadConfig> _connectionString;

        public Dividend(IOptions<ReadConfig> connectionString)
        {
            this._connectionString = connectionString;
        }

        public JsonResponse GetDividendTableList(string CompCode)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();

                    DynamicParameters param = new DynamicParameters();
                    param.Add("@P_COMPCODE", CompCode);
                    List<ATTDividend> aTTDividendTables = connection.Query<ATTDividend>(sql: "GET_ALL_DIVIDEND_TABLE_LIST", param, commandType: CommandType.StoredProcedure).ToList();
                    if (aTTDividendTables.Count > 0)
                    {
                        jsonResponse.IsSuccess = true;
                        jsonResponse.ResponseData = aTTDividendTables;
                    }
                    else
                    {
                        jsonResponse.Message = ATTMessages.NO_RECORDS_FOUND;
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

        public JsonResponse GetDividendPaidSummary(string CompCode, string DivCode, string StartDate, string EndDate)
        {
            JsonResponse jsonResponse = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                try
                {
                    connection.Open();
                    ATTDividend aTTDividend = connection.Query<ATTDividend>(sql: "Select * from DivTable where compcode ='" + CompCode + "' and divcode='" + DivCode + "' and Divcalculated=1", null, commandType: null)?.FirstOrDefault();
                    if (aTTDividend != null)
                    {
                        DynamicParameters parameters = new DynamicParameters();
                        parameters.Add("@TableName1", aTTDividend.tablename1);
                        parameters.Add("@wamtpaiddt_from", StartDate);
                        parameters.Add("@wamtpaiddt_to", EndDate);


                        List<ATTDividendSummary> aTTDividendSummaries = connection.Query<ATTDividendSummary>("Dividend_Paid_Summmary", parameters, commandType: CommandType.StoredProcedure)?.ToList();

                        if (aTTDividendSummaries.Count > 0)
                        {
                            DynamicParameters param = new DynamicParameters();
                            param.Add("@TableName1", aTTDividend.tablename1);
                            ATTDividendSummary aTTDividendSummary = connection.Query<ATTDividendSummary>("Dividend_Paid_Summmary", param, commandType: CommandType.StoredProcedure)?.FirstOrDefault();
                            //aTTDividendSummary.WAmtPaidDt = "Opening Bal On " + Convert.ToDateTime(aTTDividendSummaries[0].WAmtPaidDt).ToString("yyyy-MM-dd");

                            aTTDividendSummary.WAmtPaidDt = aTTDividendSummaries[0].WAmtPaidDt;
                            if (aTTDividendSummary.WarrantAmt != null)
                            {
                                aTTDividendSummary.Balance = Convert.ToString(RoundtoSixDigitDecimal(Convert.ToDouble(aTTDividendSummary.WarrantAmt) - Convert.ToDouble(aTTDividendSummary.BonusTax) - Convert.ToDouble(aTTDividendSummary.TaxDAmt) - Convert.ToDouble(aTTDividendSummary.BonusAdj)));

                            }
                            else
                            {
                                aTTDividendSummary.Balance = "";
                            }
                            aTTDividendSummary.BonusAdj = "-";
                            aTTDividendSummary.WarrantAmt = "-";
                            aTTDividendSummary.BonusTax = "-";
                            aTTDividendSummary.TaxDAmt = "-";
                            aTTDividendSummary.ShholderNo = "-";

                            aTTDividendSummaries.ForEach(
                                    x =>
                                    x.NetAmountPaid = Convert.ToString(RoundtoSixDigitDecimal(Convert.ToDouble(x.WarrantAmt) - Convert.ToDouble(x.BonusTax) - Convert.ToDouble(x.TaxDAmt) - Convert.ToDouble(x.BonusAdj)))
                                );
                            aTTDividendSummaries.Insert(0, aTTDividendSummary);
                            for (int i = 1; i < aTTDividendSummaries.Count(); i++)
                            {
                                aTTDividendSummaries[i].BonusTax = Convert.ToString(RoundtoSixDigitDecimal(Convert.ToDouble(aTTDividendSummaries[i].BonusTax)));
                                aTTDividendSummaries[i].WarrantAmt = Convert.ToString(RoundtoSixDigitDecimal(Convert.ToDouble(aTTDividendSummaries[i].WarrantAmt)));
                                aTTDividendSummaries[i].TaxDAmt = Convert.ToString(RoundtoSixDigitDecimal(Convert.ToDouble(aTTDividendSummaries[i].TaxDAmt)));
                                aTTDividendSummaries[i].NetAmountPaid = Convert.ToString(RoundtoSixDigitDecimal(Convert.ToDouble(aTTDividendSummaries[i].NetAmountPaid)));

                                aTTDividendSummaries[i].Balance = Convert.ToString(RoundtoSixDigitDecimal(Convert.ToDouble(aTTDividendSummaries[i - 1].Balance) - Convert.ToDouble(aTTDividendSummaries[i].NetAmountPaid)));
                            }
                            jsonResponse.ResponseData = aTTDividendSummaries;
                            jsonResponse.IsSuccess = true;
                        }

                    }
                }
                catch (Exception ex)
                {
                    jsonResponse.ResponseData = ex;
                    jsonResponse.IsSuccess = false;
                    jsonResponse.HasError = true;
                }
            }
            return jsonResponse;
        }


        public double RoundtoSixDigitDecimal(double value)
        {
            return Math.Round(value, 6);
        }

    }
}
