namespace MacPrint.Interfaces
{
    using System.Net.Mail;

    public interface IEmailSender
    {
        void SendEmail(string from, string to, string subject, string body, string attachmentLocation = "");
        void AddAttachment(MailMessage mail, string attachmentLocation);
    }
}
