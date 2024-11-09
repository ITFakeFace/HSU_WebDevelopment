using MailKit;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using MimeKit;

namespace LibraryManagementSystem.Mail
{
    public class MailSettings
    {
        public string? Mail { get; set; }
        public string? DisplayName { get; set; }
        public string? Password { get; set; }
        public string? Host { get; set; }
        public int Port { get; set; }

    }
    public class SendMailService : IEmailSender
    {
        readonly MailSettings mailSettings;

        public SendMailService(IOptions<MailSettings> settings)
        {
            mailSettings = settings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var message = new MimeMessage();
            message.Sender = new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail);
            message.From.Add(new MailboxAddress(mailSettings.DisplayName, mailSettings.Mail));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = subject;

            // body
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = htmlMessage;
            message.Body = bodyBuilder.ToMessageBody();

            using var smtp = new SmtpClient();

            try
            {
                smtp.Connect(mailSettings.Host, mailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);
                // xác thực
                smtp.Authenticate(mailSettings.Mail, mailSettings.Password);
                // gửi mail
                await smtp.SendAsync(message);
            }
            catch (Exception ex)
            {
                // lỗi gửi mail => lưu lại nội dung bức mail
                if (!Directory.Exists("mailssave"))
                {
                    Directory.CreateDirectory("mailssave");
                }
                var emailSaveFile = $"mailssave/{Guid.NewGuid()}.eml";
                await message.WriteToAsync(emailSaveFile);
            }

            smtp.Disconnect(true);
        }

    }
}
