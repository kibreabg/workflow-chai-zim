using Chai.WorkflowManagment.Shared.MailSender;
using log4net;
using log4net.Config;
using System;
using System.Text;

namespace Chai.WorkflowManagment.Shared
{
    public sealed class ExceptionUtility
    {
        private static readonly ILog ErrorLog = LogManager.GetLogger("ErrorLog");
        // All methods are static, so this can be private 
        private ExceptionUtility()
        { }

        // Log an Exception 
        public static void LogException(Exception exc, string source)
        {
            XmlConfigurator.Configure();
            // Include enterprise logic for logging exceptions 

            if (exc.InnerException != null)
            {
                ErrorLog.ErrorFormat("Inner Exception Type: {0}", exc.InnerException.GetType().ToString());
                ErrorLog.ErrorFormat("Inner Exception: {0}", exc.InnerException.Message);
                if (exc.InnerException.InnerException != null)
                    ErrorLog.ErrorFormat("Second Level Exception: {0}", exc.InnerException.InnerException.Message);
                ErrorLog.ErrorFormat("Inner Source: {0}", exc.InnerException.Source);
                if (exc.InnerException.StackTrace != null)
                {
                    ErrorLog.ErrorFormat("Inner Stack Trace: {0}", exc.InnerException.StackTrace);
                }
            }
            ErrorLog.ErrorFormat("Exception Type: {0}", exc.GetType().ToString());
            ErrorLog.ErrorFormat("Exception: {0}", exc.Message);
            ErrorLog.ErrorFormat("Source: {0}", source);
            if (exc.StackTrace != null)
            {
                ErrorLog.Error("Stack Trace: ");
                ErrorLog.Error(exc.StackTrace);
            }
        }

        // Notify System Operators about an exception 
        public static void NotifySystemOps(Exception exc, string sourceUser)
        {
            StringBuilder body = new StringBuilder();
            body.AppendLine("<b>Error generated from</b>" + sourceUser + System.Environment.NewLine
                + "<b>Inner Exception</b> " + exc.InnerException + System.Environment.NewLine
                + "<b>Stacktrace</b> " + exc.StackTrace + System.Environment.NewLine + "<b>Source</b> "
                + exc.Source + System.Environment.NewLine + "  <b>Target Site</b>  " + exc.TargetSite);
            EmailSender.SendException("kgizatu@clintonhealthaccess.org,pmpofu@clintonhealthaccess.org", exc.Message, body.ToString());
        }
    }
}
