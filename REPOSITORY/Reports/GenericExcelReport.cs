using ClosedXML.Excel;
using Entity.Common;
using Entity.Dividend;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Repository.Reports
{
    public class GenericExcelReport
    {
        public JsonResponse GenerateExcelReport(JsonResponse data, string ExcelSheetName, string SelectedReportType, string CompEnName, string CompCode, string ShareType)
        {
            JsonResponse response = new JsonResponse();
            if (data.IsSuccess)
            {
                using (var workbook = new XLWorkbook())
                {
                    try
                    {
                        var worksheet = workbook.Worksheets.Add((ExcelSheetName.Length >= 30 ? ExcelSheetName.Substring(0, 30) : ExcelSheetName.Substring(0, ExcelSheetName.Length)));


                        if (SelectedReportType == "SRIP")
                        {
                            ATTSummaryReportDividend SummaryDetails = (ATTSummaryReportDividend)data.ResponseData;
                            var currentColumn = 1;
                            var currentRow = 1;

                            //Creat The Headers of the excel

                            CreateCellStyle(worksheet, currentRow, currentColumn, 15, true, "Times New Roman", CompEnName + " " + CompCode);


                            currentRow = 2;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 15, true, "Times New Roman", ShareType == "D" ? "DEMAT Summary Report of Issued and Paid Dividend warrants " : "PHYSICAL Summary Report of Issued and Paid Dividend warrants");

                            currentRow = 4;
                            currentColumn = 2;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 13, true, "Times New Roman", "Details of total dividend warrants:");

                            currentRow = 5;
                            currentColumn = 2;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 13, true, "Times New Roman", ShareType == "D" ? "No. Of BOID : " : "No. of Holder : ");
                            currentColumn = 3;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, false, "Times New Roman", SummaryDetails.aTTTotalDividendWarrants.totalshhno);
                            currentColumn = 5;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, true, "Times New Roman", "Total Amount : ");
                            currentColumn = 6;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, false, "Times New Roman", SummaryDetails.aTTTotalDividendWarrants.totalamt);
                            currentColumn = 8;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, true, "Times New Roman", "Total Kitta : ");
                            currentColumn = 9;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, false, "Times New Roman", SummaryDetails.aTTTotalDividendWarrants.totalkitta);
                            currentColumn = 11;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, true, "Times New Roman", "Net Amount : ");
                            currentColumn = 12;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, false, "Times New Roman", SummaryDetails.aTTTotalDividendWarrants.netamt);

                            currentRow = 7;
                            currentColumn = 2;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 13, true, "Times New Roman", "Details of issued (Posted and Unposted) dividend warrants:");

                            currentRow = 8;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, true, "Times New Roman", ShareType == "D" ? "No. Of BOID : " : "No. of Holder : ");
                            currentColumn = 3;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, false, "Times New Roman", SummaryDetails.aTTIssuedDividendWarrants.totalshhno1);
                            currentColumn = 5;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, true, "Times New Roman", "Total Amount : ");
                            currentColumn = 6;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, false, "Times New Roman", SummaryDetails.aTTIssuedDividendWarrants.totalamt1);
                            currentColumn = 8;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, true, "Times New Roman", "Total Kitta : ");
                            currentColumn = 9;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, false, "Times New Roman", SummaryDetails.aTTIssuedDividendWarrants.totalkitta1);
                            currentColumn = 11;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, true, "Times New Roman", "Net Amount : ");
                            currentColumn = 12;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, false, "Times New Roman", SummaryDetails.aTTIssuedDividendWarrants.netamt1);


                            currentRow = 10;
                            currentColumn = 2;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 13, true, "Times New Roman", "Details of issued and Posted dividend warrants:");

                            currentRow = 11;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, true, "Times New Roman", ShareType == "D" ? "No. Of BOID : " : "No. of Holder : ");
                            currentColumn = 3;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, false, "Times New Roman", SummaryDetails.aTTIssuedPostedDividendWarrants.totalshhno2);
                            currentColumn = 5;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, true, "Times New Roman", "Total Amount : ");
                            currentColumn = 6;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, false, "Times New Roman", SummaryDetails.aTTIssuedPostedDividendWarrants.totalamt2);
                            currentColumn = 8;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, true, "Times New Roman", "Total Kitta : ");
                            currentColumn = 9;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, false, "Times New Roman", SummaryDetails.aTTIssuedPostedDividendWarrants.totalkitta2);
                            currentColumn = 11;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, true, "Times New Roman", "Net Amount : ");
                            currentColumn = 12;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, false, "Times New Roman", SummaryDetails.aTTIssuedPostedDividendWarrants.netamt2);


                            currentRow = 13;
                            currentColumn = 2;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 13, true, "Times New Roman", "Details of issued but Unposted dividend warrants:");
                            currentRow = 14;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, true, "Times New Roman", ShareType == "D" ? "No. Of BOID : " : "No. of Holder : ");
                            currentColumn = 3;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, false, "Times New Roman", SummaryDetails.aTTIssuedUnpostedDividendWarrants.totalshhno3);
                            currentColumn = 5;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, true, "Times New Roman", "Total Amount : ");
                            currentColumn = 6;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, false, "Times New Roman", SummaryDetails.aTTIssuedUnpostedDividendWarrants.totalamt3);
                            currentColumn = 8;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, true, "Times New Roman", "Total Kitta : ");
                            currentColumn = 9;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, false, "Times New Roman", SummaryDetails.aTTIssuedUnpostedDividendWarrants.totalkitta3);
                            currentColumn = 11;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, true, "Times New Roman", "Net Amount : ");
                            currentColumn = 12;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, false, "Times New Roman", SummaryDetails.aTTIssuedUnpostedDividendWarrants.netamt3);

                            currentRow = 16;
                            currentColumn = 2;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 13, true, "Times New Roman", "Details of Unissued  dividend warrants:");
                            currentRow = 17;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, true, "Times New Roman", ShareType == "D" ? "No. Of BOID : " : "No. of Holder : ");
                            currentColumn = 3;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, false, "Times New Roman", SummaryDetails.aTTUnIssuedDividendWarrants.totalshhno4);
                            currentColumn = 5;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, true, "Times New Roman", "Total Amount : ");
                            currentColumn = 6;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, false, "Times New Roman", SummaryDetails.aTTUnIssuedDividendWarrants.totalamt4);
                            currentColumn = 8;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, true, "Times New Roman", "Total Kitta : ");
                            currentColumn = 9;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, false, "Times New Roman", SummaryDetails.aTTUnIssuedDividendWarrants.totalkitta4);
                            currentColumn = 11;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, true, "Times New Roman", "Net Amount : ");
                            currentColumn = 12;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, false, "Times New Roman", SummaryDetails.aTTUnIssuedDividendWarrants.netamt4);


                            currentRow = 19;
                            currentColumn = 2;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 13, true, "Times New Roman", "Details of Paid Posted  dividend warrants:");
                            currentRow = 20;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, true, "Times New Roman", ShareType == "D" ? "No. Of BOID : " : "No. of Holder : ");
                            currentColumn = 3;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, false, "Times New Roman", SummaryDetails.aTTPaidPostedDividendWarrants.totalshhno5);
                            currentColumn = 5;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, true, "Times New Roman", "Total Amount : ");
                            currentColumn = 6;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, false, "Times New Roman", SummaryDetails.aTTPaidPostedDividendWarrants.totalamt5);
                            currentColumn = 8;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, true, "Times New Roman", "Total Kitta : ");
                            currentColumn = 9;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, false, "Times New Roman", SummaryDetails.aTTPaidPostedDividendWarrants.totalkitta5);
                            currentColumn = 11;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, true, "Times New Roman", "Net Amount : ");
                            currentColumn = 12;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, false, "Times New Roman", SummaryDetails.aTTPaidPostedDividendWarrants.netamt5);



                            currentRow = 22;
                            currentColumn = 2;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 13, true, "Times New Roman", "Details of Paid Unposted  dividend warrants:");
                            currentRow = 23;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, true, "Times New Roman", ShareType == "D" ? "No. Of BOID : " : "No. of Holder : ");
                            currentColumn = 3;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, false, "Times New Roman", SummaryDetails.aTTPaidUnpostedDividendWarrants.totalshhno6);
                            currentColumn = 5;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, true, "Times New Roman", "Total Amount : ");
                            currentColumn = 6;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, false, "Times New Roman", SummaryDetails.aTTPaidUnpostedDividendWarrants.totalamt6);
                            currentColumn = 8;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, true, "Times New Roman", "Total Kitta : ");
                            currentColumn = 9;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, false, "Times New Roman", SummaryDetails.aTTPaidUnpostedDividendWarrants.totalkitta6);
                            currentColumn = 11;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, true, "Times New Roman", "Net Amount : ");
                            currentColumn = 12;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, false, "Times New Roman", SummaryDetails.aTTPaidUnpostedDividendWarrants.netamt6);


                            currentRow = 25;
                            currentColumn = 2;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 13, true, "Times New Roman", "Details of Unpaid  dividend warrants:");
                            currentRow = 26;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, true, "Times New Roman", ShareType == "D" ? "No. Of BOID : " : "No. of Holder : ");
                            currentColumn = 3;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, false, "Times New Roman", SummaryDetails.aTTUnpaidDividendWarrants.totalshhno7);
                            currentColumn = 5;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, true, "Times New Roman", "Total Amount : ");
                            currentColumn = 6;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, false, "Times New Roman", SummaryDetails.aTTUnpaidDividendWarrants.totalamt7);
                            currentColumn = 8;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, true, "Times New Roman", "Total Kitta : ");
                            currentColumn = 9;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, false, "Times New Roman", SummaryDetails.aTTUnpaidDividendWarrants.totalkitta7);
                            currentColumn = 11;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, true, "Times New Roman", "Net Amount : ");
                            currentColumn = 12;
                            CreateCellStyle(worksheet, currentRow, currentColumn, 12, false, "Times New Roman", SummaryDetails.aTTUnpaidDividendWarrants.netamt7);

                        }
                        else
                        {
                            DataSet ds = new DataSet();
                            ds = (DataSet)data.ResponseData;
                            DataTable dt = new DataTable();
                            dt = ds.Tables[0];
                            string[] columnNames = dt.Columns.Cast<DataColumn>()
                                         .Select(x => x.ColumnName)
                                         .ToArray();

                            CreateCellStyle(worksheet, 1, 1, 12, true, "Times New Roman", CompEnName + " " + CompCode);
                            CreateCellStyle(worksheet, 2, 1, 12, true, "Times New Roman", ExcelSheetName);
                            for (int i = 0; i < columnNames.Length; i++)
                            {
                                worksheet.Cell(5, i + 1).Value = columnNames[i];
                            }

                            worksheet.Cell(6, 1).InsertData(dt);


                        }
                        using (var stream = new MemoryStream())
                        {
                            workbook.SaveAs(stream);
                            var content = stream.ToArray();
                            String file = Convert.ToBase64String(stream.ToArray());
                            response.IsSuccess = true;
                            response.ResponseData = file;
                        }
                    }
                    catch (Exception ex)
                    {
                        response.Message = ex.Message;
                        response.IsSuccess = false;
                    }

                }

            }
            else { response = data; }
            return response;
        }


        public static DataTable DynamicToDT(List<dynamic> objects)
        {
            DataTable dt = new DataTable("NewDataTable"); // Runtime Datatable  
            dt.Columns.Add("Values", typeof(string)); // Adding dynamic column  
            if (objects != null && objects.Count > 0)
            {
                for (int i = 0; i < objects.Count; i++)
                {
                    DataRow dr = dt.NewRow(); // Adding values to Datatable  
                    dr["Values"] = objects[i];
                    dt.Rows.Add(dr);
                }

                return dt; // Converted Dynamic list to Datatable  
            }
            return null;
        }

        private void CreateCellStyle(IXLWorksheet ws, int currentRow, int currentColumn, int fontSize, bool bold, string fontName, string Text)
        {
            ws.Cell(currentRow, currentColumn).Value = Text;
            ws.Cell(currentRow, currentColumn).Style.Font.Bold = bold;
            ws.Cell(currentRow, currentColumn).Style.Font.FontSize = fontSize;
            ws.Cell(currentRow, currentColumn).Style.Font.FontName = fontName;
        }

        private void CreateCellStyle(IXLWorksheet ws, int currentRow, int currentColumn, int fontSize, bool bold, string fontName, Double Text)
        {
            ws.Cell(currentRow, currentColumn).Value = Text;
            ws.Cell(currentRow, currentColumn).Style.Font.Bold = bold;
            ws.Cell(currentRow, currentColumn).Style.Font.FontSize = fontSize;
            ws.Cell(currentRow, currentColumn).Style.Font.FontName = fontName;
        }

    }
}
