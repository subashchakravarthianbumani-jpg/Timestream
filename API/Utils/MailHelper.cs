using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Model.ViewModel;
using System.Collections.Immutable;
using System.Net;
using System.Net.Mail;
using Utils.Interface;
using Utils.UtilModels;

namespace Utils
{
    public class MailHelper : IMailHelper
    {
        private MailConfiguration _confg;
        private readonly IConfiguration _configuration;

        public MailHelper(IOptions<MailConfiguration> confg, IConfiguration configuration)
        {
            _confg = confg.Value;
            _configuration = configuration;
        }

        public bool SendMail(EmailModel config, out string body, out string subject)
        {
            body = string.Empty;
            subject = string.Empty;

            using (SmtpClient client = new SmtpClient(_confg.SmtpServer, _confg.Port))
            {
                MailMessage mailMessage = new MailMessage();
                try
                {
                    client.DeliveryMethod = SmtpDeliveryMethod.Network;
                    client.EnableSsl = _confg.SSL;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential(_confg.Email, _confg.Password);
                    //client.Timeout = 5000;

                    mailMessage.From = new MailAddress(_confg.From);
                    mailMessage.IsBodyHtml = true;

                    if (config.Attachemnts != null)
                    {
                        foreach (var attachemnt in config.Attachemnts)
                        {
                            mailMessage.Attachments.Add(new Attachment(attachemnt.Value, attachemnt.Key));
                        }
                    }
                    foreach (var address in config.To)
                    {
                        mailMessage.To.Add(address);
                    }
                    foreach (var address in config.CC ?? new List<string>())
                    {
                        mailMessage.CC.Add(address);
                    }
                    foreach (var address in config.BCC ?? new List<string>())
                    {
                        mailMessage.Bcc.Add(address);
                    }
                    foreach (var dict in config.BodyPlaceHolders ?? new Dictionary<string, string>())
                    {
                        config.Body = config.Body.Replace(dict.Key, dict.Value);
                    }
                    foreach (var dict in config.SubjectPlaceHolders ?? new Dictionary<string, string>())
                    {
                        config.Subject = config.Subject.Replace(dict.Key, dict.Value);
                    }

                    body = config.Body ?? "";
                    subject = config.Subject ?? "";

                    mailMessage.Body = config.Body ?? "";
                    mailMessage.Subject = config.Subject ?? "";
                    client.Send(mailMessage);

                    return true;
                }
                catch (Exception ex)
                {
                    return false;
                }
                finally
                {
                    if (mailMessage != null)
                    {
                        ((IDisposable)mailMessage).Dispose();
                    }
                }
            }
        }

        public void SendMailAsync(EmailModel config)
        {
            SmtpClient client = new SmtpClient(_confg.SmtpServer, _confg.Port);
            MailMessage mailMessage = new MailMessage();
            try
            {
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = _confg.SSL;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(_confg.Email, _confg.Password);
                mailMessage.From = new MailAddress(_confg.From);
                mailMessage.IsBodyHtml = true;
                if (config.Attachemnts != null)
                {
                    foreach (var attachemnt in config.Attachemnts)
                    {
                        mailMessage.Attachments.Add(new Attachment(attachemnt.Value, attachemnt.Key));
                    }
                }
                foreach (var address in config.To ?? new List<string>())
                {
                    mailMessage.To.Add(address);
                }
                foreach (var address in config.CC ?? new List<string>())
                {
                    mailMessage.CC.Add(address);
                }
                foreach (var address in config.BCC ?? new List<string>())
                {
                    mailMessage.Bcc.Add(address);
                }
                foreach (var dict in config.BodyPlaceHolders ?? new Dictionary<string, string>())
                {
                    config.Body = config.Body.Replace(dict.Key, dict.Value);
                }
                foreach (var dict in config.SubjectPlaceHolders ?? new Dictionary<string, string>())
                {
                    config.Subject = config.Subject.Replace(dict.Key, dict.Value);
                }

                mailMessage.Body = config.Body;
                mailMessage.Subject = config.Subject;
                client.SendCompleted += new SendCompletedEventHandler(SendCompleted);
                client.SendMailAsync(mailMessage);
            }
            catch (Exception)
            {
                throw;
            }
        }

        private void SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                //"user Send canceled."
            }
            if (e.Error != null)
            {
                //"Send error."
            }
            else
            {
                //"Send success."
            }

            SmtpClient client = (SmtpClient)sender;
            client.Dispose();
        }
    }
}
