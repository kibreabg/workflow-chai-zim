using System;
using System.Configuration;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Text;

namespace Chai.WorkflowManagment.Shared.MailSender
{
    public static class EmailSender
    {
        public static bool Send(string to, string subject, string body)
        {
            string localIP = "http://10.139.1.25/ZWFM/UserLogin.aspx";
            string publicIp = "http://zimops.clintonhealthaccess.org:444/ZWFM/UserLogin.aspx";

            SmtpSection section = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");

            try
            {
                using (SmtpClient client = new SmtpClient(section.Network.Host, section.Network.Port))
                {
                    client.EnableSsl = section.Network.EnableSsl;
                    client.Timeout = 2000000;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(section.Network.UserName, section.Network.Password);
                    client.Send(section.From, to, subject, body + " Click here if you're in the office: " + localIP + " or Click here if you're outside the office: " + publicIp);
                    client.Dispose();
                }
            }
            catch (Exception ex)
            {
                ExceptionUtility.LogException(ex, ex.Source);
                return false;
            }

            return false;
        }

        public static bool SendEmails(string from, string to, string subject, string body)
        {
            SmtpSection section = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");

            try
            {
                using (SmtpClient client = new SmtpClient(section.Network.Host, section.Network.Port))
                {
                    client.EnableSsl = section.Network.EnableSsl;
                    client.Timeout = 2000000;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(section.Network.UserName, section.Network.Password);
                    client.Send(section.From, to, subject, body);
                    client.Dispose();
                }
            }
            catch (Exception ex)
            {
                ExceptionUtility.LogException(ex, ex.Source);
                return false;
            }

            return false;
        }

        public static bool SendException(string to, string subject, string body)
        {
            SmtpSection section = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");

            //create the mail message
            MailMessage mail = new MailMessage
            {
                //set the addresses
                From = new MailAddress(section.From)
            };
            mail.To.Add(to);

            //set the content
            mail.Subject = SanitizeSubject(subject);
            mail.Body = body;
            mail.IsBodyHtml = true;

            try
            {
                using (SmtpClient client = new SmtpClient(section.Network.Host, section.Network.Port))
                {
                    client.EnableSsl = section.Network.EnableSsl;
                    client.Timeout = 2000000;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(section.Network.UserName, section.Network.Password);
                    client.Send(mail);
                    client.Dispose();
                }
            }
            catch (Exception ex)
            {
                ExceptionUtility.LogException(ex, ex.Source);
                return false;
            }

            return false;
        }

        private static string SanitizeSubject(string subject)
        {
            if (string.IsNullOrWhiteSpace(subject))
                return "Application Exception";

            var sb = new StringBuilder(subject.Length);
            foreach (char c in subject)
            {
                // replace CR/LF with space; drop other control characters
                if (c == '\r' || c == '\n')
                    sb.Append(' ');
                else if (!char.IsControl(c))
                    sb.Append(c);
            }

            var result = sb.ToString().Trim();
            if (result.Length == 0)
                result = "Application Exception";

            const int MaxSubjectLength = 255;
            if (result.Length > MaxSubjectLength)
                result = result.Substring(0, MaxSubjectLength);

            return result;
        }
    }
}