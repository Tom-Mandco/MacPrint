namespace MacPrint.Classes
{
    using Interfaces;
    using System;
    using System.IO;
    using System.Net.Mail;

    public class EmailSender : IEmailSender
    {
        private readonly int ClientPort;
        private readonly string ClientHost;
        private readonly ILog logger;

        public EmailSender(ILog log, int clientPort, string clientHost)
        {
            logger = log;
            ClientPort = clientPort;
            ClientHost = clientHost;
        }


        public void SendEmail(string from, string to, string subject, string body, string attachmentLocation = "")
        {
            var mail = new MailMessage(from, to);
            var client = new SmtpClient
            {
                Port = ClientPort,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Host = ClientHost
            };

            mail.Subject = subject;
            mail.Body = body;

            if (attachmentLocation != "")
            {
                AddAttachment(mail, attachmentLocation);
            }

            try
            {
                client.Send(mail);
            }
            catch (Exception e)
            {
                logger.Error(e.ToString());
            }

        }

        public void AddAttachment(MailMessage mail, string attachmentLocation)
        {
            using (var inputFile = new FileStream(
                     attachmentLocation,
                     FileMode.Open,
                     FileAccess.Read,
                     FileShare.ReadWrite))
            {
                var fileName = Path.GetFileName(attachmentLocation);

                if (File.Exists(fileName))
                {
                    File.Delete(fileName);
                }

                File.Copy(attachmentLocation, fileName);

                var attachment = new Attachment(fileName);

                mail.Attachments.Add(attachment);
            }
        }
    }


}
