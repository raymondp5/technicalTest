using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace CXmlInvoiceGenerator
{

    
    public static class AppSettings
    {
       
        public static string  payLoadID { get; set; }
        public static string outputFilePath { get; set; }
        public static string logFilePath  { get; set; }
        public static string fromDomain  { get; set; }
        public static string toDomain  { get; set; }
        public static string senderDomain  { get; set; }
        public static string senderIdentity  { get; set; }
        public static string senderIdentitySharedSecret  { get; set; }

        

        public static void ReadAllSettings()
        {
            try
            {
                outputFilePath = null;
                payLoadID = ConfigurationManager.AppSettings["payLoadID"];
                outputFilePath = ConfigurationManager.AppSettings["OutputFilePath"];
                logFilePath = ConfigurationManager.AppSettings["LogFilePath"];
                fromDomain = ConfigurationManager.AppSettings["FromDomain"];
                toDomain = ConfigurationManager.AppSettings["ToDomain"];
                senderDomain = ConfigurationManager.AppSettings["SenderDomain"];
                senderIdentity = ConfigurationManager.AppSettings["SenderIdentity"];
                senderIdentitySharedSecret = ConfigurationManager.AppSettings["SenderIdentitySharedSecret"];

                Console.WriteLine($"Output File Path: {payLoadID}");
                Console.WriteLine($"Output File Path: {outputFilePath}");
                Console.WriteLine($"Log File Path: {logFilePath}");
                Console.WriteLine($"From Domain: {fromDomain}");
                Console.WriteLine($"To Domain: {toDomain}");
                Console.WriteLine($"Sender Domain: {senderDomain}");
                Console.WriteLine($"Sender Identity: {senderIdentity}");
                Console.WriteLine($"Sender Identity Shared Secret: {senderIdentitySharedSecret}");

            }
            catch(Exception ex) 
            {
                Console.WriteLine("Application Config file failed to load because " + ex.Message );
            }

         }
        static void ReadSetting(string key)
        {
            try
            {
                var appSettings = ConfigurationManager.AppSettings;
                string result = appSettings[key] ?? "Not Found";
                Console.WriteLine(result);
            }
            catch (ConfigurationErrorsException)
            {
                Console.WriteLine("Error reading app settings");
            }
        }

        
    }

}
