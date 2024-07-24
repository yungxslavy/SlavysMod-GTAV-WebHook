using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlavysMod
{
    class Logger
    {
        private readonly static string logName = "SlavysMod.log";

        // Logs to the file
        public static void Log(string message)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(logName, true))
                {
                    string logEntry = $"{DateTime.Now}: {message}";
                    writer.WriteLine(logEntry);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to write to log: {ex.Message}");
            }
        }

        // Optional: Method to clear the log file
        public static void ClearLog()
        {
            try
            {
                File.WriteAllText(logName, string.Empty);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to clear log: {ex.Message}");
            }
        }

        public static string GetLogs()
        {
            try
            {
                StreamReader reader = new StreamReader(logName);
                string logTxt;
                
                logTxt = reader.ReadToEnd();
                reader.Close();
                return logTxt;
            }
            catch (Exception ex)
            {
                Logger.Log($"Failed to read log: {ex.Message}");
                return $"Failed to read log error: {ex.Message}";
            }
        }
    }
}
