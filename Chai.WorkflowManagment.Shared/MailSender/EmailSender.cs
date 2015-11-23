using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Net.Configuration;
using System.Configuration;
namespace Chai.WorkflowManagment.Shared.MailSender
{

    public class EmailSender
    {

        public static bool Send(string to, string subject, string body)
        {
            string from = string.Empty;
            string localIP = "http://10.139.1.25/ZWFM/UserLogin.aspx";
            string publicIp = "http://197.211.216.65/ZWFM/UserLogin.aspx";
        

            SmtpSection section = (SmtpSection)ConfigurationManager.GetSection("system.net/mailSettings/smtp");

            try
            {
                using (SmtpClient client = new SmtpClient(section.Network.Host, section.Network.Port))
                {
                    client.EnableSsl = section.Network.EnableSsl;
                    client.Timeout = 2000000;
                    client.Credentials = new System.Net.NetworkCredential(section.Network.UserName, section.Network.Password);
                    client.Send(section.From, to, subject, body + " Local: " + localIP + " Outside the office: " + publicIp);
                    client.Dispose();
                }

            }
            catch (Exception ex)
            {
                return false;
            }

            return false;
        }



    }
}

