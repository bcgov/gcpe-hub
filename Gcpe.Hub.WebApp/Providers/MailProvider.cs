using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
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
            message.BodyEncoding = System.Text.Encoding.Default;
            message.Body = bodyHtml;
            message.IsBodyHtml = true;

            if (!string.IsNullOrEmpty(settings.SmtpHost))
            {
                using (var client = new System.Net.Mail.SmtpClient(settings.SmtpHost))
                {
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

        public string PickupDirectory { get; set; }
    }
}