
//using System;
//using System.IO;

//namespace REPOSITORY.FundTransfer.Service
//{
//    public static class Logger
//    {
//        // Path to the log file
//        private static string logFilePath;
//        // Static constructor to ensure log directory exists
//        static Logger()
//        {
//            // Define the folder path for logs
//            string folderPath = @"D:\EFCoreProjects\EFCoreCodeFirstDemo\EFCoreCodeFirstDemo\Logs"; // Update this path as needed
//            // Ensure the directory exists; if not, it will be created
//            Directory.CreateDirectory(folderPath);
//            // Get the current date and format it
//            string currentDate = DateTime.Now.ToString("yyyyMMdd"); // e.g., 20240922 for September 22, 2024
//            // Define the file name with the current date
//            string fileName = $"Log_{currentDate}.txt";
//            // Combine the folder path and file name to create the full file path
//            logFilePath = Path.Combine(folderPath, fileName);
//        }
//        // Logs a message with a timestamp to the log file.
//        public static void Log(string message)
//        {
//            try
//            {
//                // Prepare the log message with timestamp
//                var logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}";
//                // Append the log message to the log file
//                File.AppendAllText(logFilePath, logMessage + Environment.NewLine);
//            }
//            catch (Exception ex)
//            {
//                // In case logging fails, log to console or a separate error handling system.
//                Console.WriteLine($"Logging failed: {ex.Message}");
//            }
//        }
//    }
//}
//}
