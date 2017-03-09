using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Configuration;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Softensity.Hatley.Common;

namespace Softensity.Hatley.DAL
{
    public class EmailService :IIdentityMessageService
    {
        public static EmailConfigurationSection EmailConfigurationSection =
            ConfigurationManager.GetSection("CustomConfigurationGroup/EmailConfiguration") as EmailConfigurationSection;
        public Task SendAsync(IdentityMessage message)
        {
            var mail = new MailMessage
            {
                From = new MailAddress(EmailConfigurationSection.EmailFrom),
                Subject = message.Subject,
                Body = message.Body,
                IsBodyHtml = true
            };
            mail.To.Add(message.Destination);

            SmtpClient client = new SmtpClient(EmailConfigurationSection.EmailHost, EmailConfigurationSection.EmailPort)
			{
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = true,
                Credentials = new System.Net.NetworkCredential(EmailConfigurationSection.EmailFrom, EmailConfigurationSection.EmailPass),
				EnableSsl = EmailConfigurationSection.EnableSsl,
                Host = EmailConfigurationSection.EmailHost,
                Port = EmailConfigurationSection.EmailPort,
                Timeout = 20000
			};

            //return client.SendMailAsync(EmailConfigurationSection.EmailFrom, message.Destination, message.Subject, message.Body);
            var res = client.SendMailAsync(mail);;
            return res;
        }
    }
}
