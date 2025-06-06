using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Mail;
using System.Web;
using System.Web.Services.Description;

namespace Hotel.Common
{
    public class Common
    {
        private static string password = ConfigurationManager.AppSettings["PasswordEmail"];
        public static string email = ConfigurationManager.AppSettings["Email"];
        public static bool sendEmail(string name, string subject, string content, string toMail)
        {
            bool rs = false;
            try
            {
                MailMessage mail = new MailMessage();
                var smtp = new System.Net.Mail.SmtpClient();
                {
                    smtp.Host = "smtp.gmail.com";
                    smtp.Port = 587;
                    smtp.EnableSsl = true;
                    smtp.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
                    smtp.Credentials = new System.Net.NetworkCredential(email, password);
                    smtp.Timeout = 20000;
                    MailAddress fromAddress = new MailAddress(email, name);
                    mail.From = fromAddress;
                    mail.To.Add(toMail);
                    mail.Subject = subject;
                    mail.IsBodyHtml = true;
                    mail.Body = content;
                    smtp.Send(mail);
                    rs = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                rs = false;
            }
            return rs;
        }
    }
}