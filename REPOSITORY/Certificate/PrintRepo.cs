using Dapper;
using Entity.Certificate;
using Entity.Common;
using ENTITY.Certificate;
using Interface.Certificate;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace REPOSITORY.Certificate
{
    public class PrintRepo : IPrint
    {
        IOptions<ReadConfig> _connectionString;
        private readonly IHostingEnvironment _webHostEnvironment;

        public PrintRepo(IOptions<ReadConfig> connectionString, IHostingEnvironment webHostEnvironment)
        {
            _connectionString = connectionString;
            _webHostEnvironment = webHostEnvironment;
        }

        public JsonResponse PrintCertificates(List<ATTDuplicateCertificate> list, string CompCode, string Username, string IPAddress)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                connection.Open();
                try
                {
                    //JsonResponse json = new JsonResponse();
                    string folderName = "PDFReports";
                    string webRootPath = _webHostEnvironment.WebRootPath;
                    string newPath = Path.Combine(webRootPath, folderName);
                    List<ATTCompanyCoordinatesForPrint> coordinateList = new List<ATTCompanyCoordinatesForPrint>();

                    DataTable dt = new DataTable();
                    dt.Columns.Add("column1");
                    list.ForEach(x => dt.Rows.Add(x.Certno));

                    SqlCommand cmd = new SqlCommand("GET_CERTIFICATE_FOR_PRINT", connection);
                    cmd.CommandType = CommandType.StoredProcedure;
                    SqlParameter param = cmd.Parameters.AddWithValue("@udt", dt);
                    param = cmd.Parameters.AddWithValue("@P_COMPCODE", CompCode);
                    param = cmd.Parameters.AddWithValue("@P_USERNAME", Username);
                    param = cmd.Parameters.AddWithValue("@P_IP_ADDRESS", IPAddress);
                    param = cmd.Parameters.AddWithValue("@P_DATE_NOW", DateTime.Now);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            coordinateList.Add(new ATTCompanyCoordinatesForPrint
                            {
                                CertNo = (int)reader[0],
                                ColumnName = reader.IsDBNull(1) ? "----": (string)reader[1],
                                x = (string)reader[2],
                                y = (string)reader[3],
                                Side = (string)reader[4]
                            });
                        }
                    }
                 response = PrintCertificateWithCoordinates(list, coordinateList, CompCode, Username, IPAddress);

                }
                catch (Exception ex)
                {
                    response.Message = ex.Message;
                    response.IsSuccess = false;
                    response.HasError = true;
                    response.ResponseData = ex;
                }
                return response;
            }
        }

        public JsonResponse PrintCertificateWithCoordinates(List<ATTDuplicateCertificate> list, List<ATTCompanyCoordinatesForPrint> coordList, string CompCode, string Username, string IPAddress)
        {
            JsonResponse response = new JsonResponse();
           
            string folderName = "PDFReports";
            string webRootPath = _webHostEnvironment.WebRootPath;
            string newPath = Path.Combine(webRootPath, folderName);
            if (!Directory.Exists(newPath))
            {
                Directory.CreateDirectory(newPath);
            }
            FileStream fs = new FileStream(webRootPath + "\\PDFReports\\" + DateTime.Now.ToString("dd-MM-yyyy hh_mm_s_tt") + "Report.pdf", FileMode.Create, FileAccess.ReadWrite,
            FileShare.None, 4096, FileOptions.DeleteOnClose);
            BaseFont customfont = BaseFont.CreateFont("PCSnepal.ttf", BaseFont.CP1252, BaseFont.EMBEDDED);
            iTextSharp.text.Font nepali = new iTextSharp.text.Font(customfont, 12);
            var bf = BaseFont.CreateFont(BaseFont.TIMES_BOLD, BaseFont.CP1250, BaseFont.NOT_EMBEDDED);

            ATTCompanyCoordinatesForPrint pageInfo = coordList.Find(x => x.ColumnName == "page");
            coordList.Remove(pageInfo);

            using (MemoryStream ms = new MemoryStream())
            {
                //foreach(ATTCompanyCoordinatesForPrint coords in coordList)
                //{
                    var pgSize = new Rectangle(float.Parse(pageInfo.x) * 73, float.Parse(pageInfo.y) * 73);
                    Document doc = new Document(pgSize);
                    doc.SetMargins(0f, 0f, 0f, 0f);
                    PdfWriter writer = PdfWriter.GetInstance(doc, ms);
                    doc.Open();
                    List<int> certNoList = coordList.Select(x => x.CertNo).Distinct().ToList();
                    for( int i = 0; i < certNoList.Count; i++ )
                    {
                        doc.NewPage();
                        PdfContentByte cb = writer.DirectContent;
                        //Set the font and size
                        cb.SetFontAndSize(customfont, 12);
                        List<ATTCompanyCoordinatesForPrint> cordwithcertno = coordList.FindAll(x => x.CertNo == certNoList[i]).ToList();
                        cb.BeginText();
                        foreach(ATTCompanyCoordinatesForPrint lst in cordwithcertno)
                        {
                        int MaxWidth = 30;
                        if (lst.Side == "L")
                        {
                            if (lst.ColumnName.Length > MaxWidth)
                            {
                                string value = lst.ColumnName;
                                float y = 0f;
                                while (value.Length > 0)
                                {
                                    cb.ShowTextAligned(0, (value).ToString().Substring(0, Math.Min(value.Length, MaxWidth)), float.Parse(lst.x) * 73, (float.Parse(lst.y) * 73 - y), 0);
                                    value = value.Remove(0, Math.Min(value.Length, MaxWidth));
                                    y = y + 12f;
                                }


                            }
                            else
                            {

                                cb.ShowTextAligned(0, (lst.ColumnName).ToString(), float.Parse(lst.x) * 73, float.Parse(lst.y) * 73, 0);
                            }
                        }
                        else
                        {
                            cb.ShowTextAligned(0, (lst.ColumnName).ToString(), float.Parse(lst.x) * 73, float.Parse(lst.y) * 73, 0);

                        }

                    }
                        cb.EndText();
                        
                    }
                    doc.Close();
                    writer.Close();
                    writer.Dispose();
                    String file = Convert.ToBase64String(ms.ToArray());
                    response.ResponseData = file;
                    response.IsSuccess = true;
                    response.HasError = false;
                    response.Message = "Company_" + CompCode + "_" + DateTime.Now.ToString() + "_Certificates.pdf";
                    fs.Close();
                    fs.Dispose();
                }
            //}
            return response;
        }

        public JsonResponse GetAllPrintFields()
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                connection.Open();
                try
                {
                    var sp = "SELECT * from CertPrintColumns";
                    dynamic list = connection.Query<dynamic>(sql: sp, null, commandType: null).ToList();
                    List<dynamic> datalist = (List<dynamic>)list;
                    var headings = ((IDictionary<string, object>)datalist.FirstOrDefault()).ToList();

                    List<(string, string)> newlist = headings.Select(x=> (x.Key,x.Value.ToString())).ToList();
                    response.ResponseData = newlist;
                    response.IsSuccess = true;
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.Message = ex.Message;
                }
            }
            return response;
        }

        public JsonResponse GetAllPrintFieldsWithCoordinates(string CompCode)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                connection.Open();
                try
                {
                    var sp = "GET_CERTIFICATE_PRINT_OPTIONS";
                    DynamicParameters param = new DynamicParameters();
                    param.Add("P_COMPCODE", CompCode);
                    List<ATTCompanyCoordinates> list = connection.Query<ATTCompanyCoordinates>(sp, param, commandType: System.Data.CommandType.StoredProcedure).ToList();
                    response.ResponseData = list;
                    response.IsSuccess = true;
                }
                catch (Exception ex)
                {
                    response.IsSuccess = false;
                    response.Message = ex.Message;
                }
            }
            return response;
        }

        public JsonResponse SaveCompanyCoordinates(List<ATTCompanyCoordinates> cordList, string CompCode)
        {
            JsonResponse response = new JsonResponse();
            using (SqlConnection connection = new SqlConnection(Crypto.Decrypt(_connectionString.Value.DefaultConnection)))
            {
                connection.Open();
                using (SqlTransaction tran = connection.BeginTransaction())
                {
                    try
                    {
                        var sp = "SAVE_CERTIFICATE_PRINT_OPTIONS";
                        DataTable dt = new DataTable();
                        dt.Columns.Add("columnname");
                        dt.Columns.Add("x");
                        dt.Columns.Add("y");
                        dt.Columns.Add("Side");
                        cordList.ForEach(x => dt.Rows.Add(x.ColumnValue, x.XCoordinate == null ? 0: x.XCoordinate, x.YCoordinate == null ? 0 : x.YCoordinate, x.Side == null ? "L" : x.Side));
                        SqlCommand cmd = new SqlCommand(sp, connection);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Transaction = tran;
                        SqlParameter param = cmd.Parameters.AddWithValue("@udt", dt);
                        param = cmd.Parameters.AddWithValue("@P_COMPCODE", CompCode);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                if (reader.GetString(0) == "1")
                                {
                                    response.IsSuccess = true;
                                    response.Message = "Data Saved Successfully!!!!";
                                }
                                else
                                {
                                    response.IsSuccess = false;
                                    response.Message = "Something went wrong!!";
                                }
                            }
                        }
                        if (response.IsSuccess) tran.Commit();
                        else tran.Rollback();
                        //DynamicParameters param = new DynamicParameters();
                        //param.Add("P_COMPCODE", CompCode);
                        //List<ATTCompanyCoordinates> list = connection.Query<ATTCompanyCoordinates>(sp, param, commandType: System.Data.CommandType.StoredProcedure).ToList();
                        //response.ResponseData = list;
                        //response.IsSuccess = true;
                    }
                    catch (Exception ex)
                    {
                        response.IsSuccess = false;
                        response.Message = ex.Message;
                        tran.Rollback();
                    }
                }
            }
            return response;
        }
    }
}
