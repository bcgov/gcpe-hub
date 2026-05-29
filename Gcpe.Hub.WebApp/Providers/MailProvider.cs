using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Gcpe.Hub.WebApp.Providers
{
    public class MailProvider
    {      
        private MailProviderSettings settings;

        public MailProvider(IOptions<MailProviderSettings> settings)
        {
            this.settings = settings.Value;

        }
        public async Task SendAsync(string subject, string bodyHtml, MailAddress from, IEnumerable<MailAddress> to = null, IEnumerable<MailAddress> cc = null)
        {

            to = to == null ? new MailAddress[0] : to.Distinct();

            cc = cc == null ? new MailAddress[0] : cc.Distinct().Except(to);

            var message = new MailMessage();

            message.From = from;

            foreach (MailAddress mailAddress in to)
            {
                message.To.Add(mailAddress);
            }

            foreach (MailAddress mailAddress in cc)
            {
                message.CC.Add(mailAddress);
            }
            

            if (!to.Any(e => e.Address == from.Address) && !cc.Any(e => e.Address == from.Address))
            {
                message.Bcc.Add(from);
            }

            message.Subject = subject;
            message.BodyEncoding = Encoding.UTF8;
            message.SubjectEncoding = Encoding.UTF8;
            message.Body = bodyHtml;
            message.IsBodyHtml = true;

            if (!string.IsNullOrEmpty(settings.SmtpHost))
            {
                using (var client = new System.Net.Mail.SmtpClient(settings.SmtpHost, settings.SmtpPort ?? 25))
                {
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.EnableSsl = settings.EnableSsl;
                    client.UseDefaultCredentials = settings.UseDefaultCredentials;
                    await client.SendMailAsync(message);
                }
            }
            else if (!string.IsNullOrEmpty(settings.PickupDirectory))
            {
                using (var client = new System.Net.Mail.SmtpClient())
                {
                    client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    client.PickupDirectoryLocation = settings.PickupDirectory;
                    await client.SendMailAsync(message);
                }
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

    }

    public class MailProviderSettings
    {
        public string SmtpHost { get; set; }

        public int? SmtpPort { get; set; }

        public bool EnableSsl { get; set; }

        public bool UseDefaultCredentials { get; set; }

        public string PickupDirectory { get; set; }
    }
}
