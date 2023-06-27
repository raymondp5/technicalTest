using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;
using System.IO;
using System.Windows;

namespace CXmlInvoiceGenerator
{
  

    public class Logger
    {
        private readonly bool writeToConsole;
        private readonly string logFilePath;

        public Logger(bool writeToConsole, string logFilePath = null)
        {
            this.writeToConsole = writeToConsole;
            this.logFilePath = logFilePath;
        }

        public void LogError(string message)
        {
            Log(LogLevel.Error, message);
        }

        public void LogInfo(string message)
        {
            Log(LogLevel.Info, message);
        }

        public void LogDebug(string message)
        {
            Log(LogLevel.Debug, message);
        }

        private void Log(LogLevel logLevel, string message)
        {
            string logEntry = $"{DateTime.Now} [{logLevel}] - {message}";

            if (logLevel == LogLevel.Error)
            {

                if (!string.IsNullOrEmpty(logFilePath))
                {
                    try
                    {
                        using (StreamWriter writer = File.AppendText(logFilePath))
                        {
                            writer.WriteLine(logEntry);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error writing to log file: {ex.Message}");
                    }
                }
            }

            if ((writeToConsole) || (logLevel == LogLevel.Info))

            {
                Console.WriteLine(logEntry);
                
               
            }

            if (logLevel == LogLevel.Error)

            {
                Console.WriteLine("Press Any Key");
                Console.ReadKey();
                System.Environment.Exit(1);
            }

        }
    }

    public enum LogLevel
    {
        Error,
        Info,
        Debug
    }
}
