using System;
using System.Text;
using System.IO;


namespace Entity.Common
{
    /*********************************************************************************
    Copyright © SOSYS , 2020
    *********************************************************************************
    Project              :Social Security Information Management System (SOSYS)  
    File                 :AppLogger.cs
    Description          :This Page contain the functions for logging the application exception
    **********************************************************************************************/
    public static class AppLogger
    {
        //public static void LogErrorToEventViewer(this Exception ex, string refUser = null, string refId = null)
        //{
        //    try
        //    {
        //        StringBuilder sb = new StringBuilder();
        //        sb.Append("********************" + " Error Log - " + DateTime.Now + "*********************");
        //        sb.Append(Environment.NewLine);
        //        sb.Append("Refrence User and Id : " + refUser + " " + refId);
        //        sb.Append(Environment.NewLine);
        //        sb.Append("Exception Type : " + ex.GetType().Name);
        //        sb.Append(Environment.NewLine);
        //        sb.Append("Error Message : " + ex.Message);
        //        sb.Append(Environment.NewLine);
        //        sb.Append("Error Source : " + ex.Source);
        //        sb.Append(Environment.NewLine);
        //        if (ex.StackTrace != null)
        //        {
        //            sb.Append("Error Trace : " + ex.StackTrace);
        //        }
        //        Exception innerEx = ex.InnerException;
        //        while (innerEx != null)
        //        {
        //            sb.Append(Environment.NewLine);
        //            sb.Append(Environment.NewLine);
        //            sb.Append("Exception Type : " + innerEx.GetType().Name);
        //            sb.Append(Environment.NewLine);
        //            sb.Append("Error Message : " + innerEx.Message);
        //            sb.Append(Environment.NewLine);
        //            sb.Append("Error Source : " + innerEx.Source);
        //            sb.Append(Environment.NewLine);
        //            if (ex.StackTrace != null)
        //            {
        //                sb.Append("Error Trace : " + innerEx.StackTrace);
        //            }
        //            innerEx = innerEx.InnerException;
        //        }
        //        if (EventLog.SourceExists("SOSYS"))
        //        {
        //            EventLog eventlog = new EventLog("Exception Log");
        //            eventlog.Source = "SOSYS";
        //            eventlog.WriteEntry(sb.ToString(), EventLogEntryType.Error);
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        throw ex;
        //    }
        //}

       
        public static void LogErrorToText(this Exception ex, string refUser = null, string refId = null)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("********************" + " Error Log - " + DateTime.Now + "*********************");
                sb.Append(Environment.NewLine);
                sb.Append("Refrence User and Id : " + refUser + " " + refId);
                sb.Append(Environment.NewLine);
                sb.Append("Exception Type : " + ex.GetType().Name);
                sb.Append(Environment.NewLine);
                sb.Append("Error Message : " + ex.Message);
                sb.Append(Environment.NewLine);
                sb.Append("Error Source : " + ex.Source);
                sb.Append(Environment.NewLine);
                if (ex.StackTrace != null)
                {
                    sb.Append("Error Trace : " + ex.StackTrace);
                }
                Exception innerEx = ex.InnerException;

                while (innerEx != null)
                {
                    sb.Append(Environment.NewLine);
                    sb.Append(Environment.NewLine);
                    sb.Append("Exception Type : " + innerEx.GetType().Name);
                    sb.Append(Environment.NewLine);
                    sb.Append("Error Message : " + innerEx.Message);
                    sb.Append(Environment.NewLine);
                    sb.Append("Error Source : " + innerEx.Source);
                    sb.Append(Environment.NewLine);
                    if (ex.StackTrace != null)
                    {
                        sb.Append("Error Trace : " + innerEx.StackTrace);
                    }
                    innerEx = innerEx.InnerException;
                }

                if (LogFolder())
                {
                    var path = LogFile();
                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        StreamWriter writer = new StreamWriter(path, true);
                        writer.WriteLine(sb.ToString());
                        writer.Flush();
                        writer.Close();
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static void LogData(this string data, string dataFor = null, string refId = null)
        {
            try
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("********************" + " Data Log - " + DateTime.Now + "*********************");
                sb.Append(Environment.NewLine);
                sb.Append("Method Name and Remarks: " + dataFor + " " + refId);
                sb.Append(Environment.NewLine);
                sb.Append(data);
                if (LogFolder())
                {
                    var path = LogFile();
                    if (!string.IsNullOrWhiteSpace(path))
                    {
                        StreamWriter writer = new StreamWriter(path, true);
                        writer.WriteLine(sb.ToString());
                        writer.Flush();
                        writer.Close();
                    }
                }
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        //private readonly static string logFolderPath = AppDomain.CurrentDomain.BaseDirectory + "log\\";
        private readonly static string logFolderPath = Environment.CurrentDirectory+"\\app_logs\\";
        //private readonly static string logFilePath = logFolderPath + "logs.txt";

        private static bool LogFolder()
        {
            try
            {
                if (Directory.Exists(logFolderPath))
                {
                    return true;
                }
                Directory.CreateDirectory(logFolderPath);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static string LogFile()
        {
            try
            {
                string logFilePath = logFolderPath + "logs_" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm") + ".txt";
                if (File.Exists(logFilePath))
                {
                    return logFilePath;
                }
                else
                {
                    var logFile = File.Create(logFilePath);
                    logFile.Close();
                    return logFilePath;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        //public static void LogException(Exception ex)
        //{
        //    try
        //    {
        //        if (LogFolder())
        //        {
        //            if (LogFile())
        //            {
        //                File.AppendAllText(logFilePath, "---------------------------------------------------------------------------------------------------------" + Environment.NewLine);
        //                File.AppendAllText(logFilePath, DateTime.Now.ToString("yyyy-MM-dd") + "   Error occurred for the user: " + HttpContext.Current.Session["User"] + Environment.NewLine);
        //                File.AppendAllText(logFilePath, ex.Message + Environment.NewLine);
        //                File.AppendAllText(logFilePath, ex.Source + Environment.NewLine);
        //                File.AppendAllText(logFilePath, ex.StackTrace + Environment.NewLine);
        //                File.AppendAllText(logFilePath, "---------------------------------------------------------------------------------------------------------" + Environment.NewLine);
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}

    }
}