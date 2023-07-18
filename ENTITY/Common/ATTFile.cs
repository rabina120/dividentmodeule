using System;
using System.IO;


namespace Entity.Common
{
    public class ATTFile
    {


        private readonly static string logFolderPath = "c:\\logs\\";

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
                string logFilePath = logFolderPath + "logs_" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
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


    }
}
