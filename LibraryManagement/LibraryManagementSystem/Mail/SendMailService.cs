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

            var bodyBuilder = new BodyBuilder { HtmlBody = htmlMessage };
            message.Body = bodyBuilder.ToMessageBody();

            using var smtp = new SmtpClient();

            try
            {
                Console.WriteLine("Connecting to email server...");
                await smtp.ConnectAsync(mailSettings.Host, mailSettings.Port, MailKit.Security.SecureSocketOptions.StartTls);

                Console.WriteLine("Authenticating...");
                await smtp.AuthenticateAsync(mailSettings.Mail, mailSettings.Password);

                Console.WriteLine($"Sending email to {email}...");
                await smtp.SendAsync(message);

                Console.WriteLine("Email sent successfully!");
            }
            catch (SmtpCommandException smtpEx)
            {
                Console.WriteLine($"SMTP Error: {smtpEx.Message} (Code: {smtpEx.StatusCode})");
                Console.WriteLine(smtpEx.StackTrace);
            }
            catch (SmtpProtocolException protocolEx)
            {
                Console.WriteLine($"Protocol Error: {protocolEx.Message}");
                Console.WriteLine(protocolEx.StackTrace);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected Error: {ex.Message}");
                Console.WriteLine(ex.StackTrace);

                if (!Directory.Exists("mailssave"))
                {
                    Directory.CreateDirectory("mailssave");
                }
                var emailSaveFile = $"mailssave/{Guid.NewGuid()}.eml";
                await message.WriteToAsync(emailSaveFile);
                Console.WriteLine($"Email saved to {emailSaveFile}");
            }
            finally
            {
                await smtp.DisconnectAsync(true);
            }
        }


    }
}
