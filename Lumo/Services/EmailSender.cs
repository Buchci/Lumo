using Microsoft.AspNetCore.Identity.UI.Services;
using MailKit.Net.Smtp;
using MimeKit;

namespace Lumo.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        public EmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var emailSettings = _config.GetSection("EmailSettings");

            var host = emailSettings["Host"];
            var port = int.Parse(emailSettings["Port"] ?? "587");
            var username = emailSettings["Username"];
            var password = emailSettings["Password"];
            var from = emailSettings["From"];

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("Lumo Team", from));
            message.To.Add(new MailboxAddress("", email));
            message.Subject = subject;

            var styledBody = $@"
    <div style='background-color: #f9fbf9; padding: 50px 20px; font-family: ""Segoe UI"", Tahoma, Geneva, Verdana, sans-serif;'>
        <div style='max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 16px; overflow: hidden; box-shadow: 0 4px 12px rgba(0,0,0,0.05); border: 1px solid #e0e8e0;'>
            
            <div style='background-color: #28a745; padding: 30px; text-align: center;'>
                <h1 style='color: #ffffff; margin: 0; font-size: 28px; letter-spacing: 1px;'>Lumo 🌿</h1>
            </div>

            <div style='padding: 40px; color: #333333; line-height: 1.6;'>
                <h2 style='color: #2d4a2d; margin-top: 0;'>Witaj!</h2>
                <p style='font-size: 16px;'>Cieszymy się, że chcesz dołączyć do społeczności Lumo. Aby w pełni korzystać z możliwości swojego cyfrowego pamiętnika, musimy potwierdzić Twój adres e-mail.</p>
                
                <div style='text-align: center; margin: 35px 0;'>
                    <div style='background-color: #28a745; color: white; padding: 14px 30px; text-decoration: none; border-radius: 8px; font-weight: bold; display: inline-block;'>
                        {htmlMessage.Replace("href=", "style='color: white; text-decoration: none;' href=")}
                    </div>
                </div>

                <p style='font-size: 14px; color: #666;'>Jeśli przycisk nie działa, skopiuj poniższy link do przeglądarki:</p>
                <p style='font-size: 12px; color: #888; word-break: break-all;'>{htmlMessage}</p>
            </div>

            <div style='background-color: #f1f4f1; padding: 20px; text-align: center; font-size: 12px; color: #777;'>
                &copy; {DateTime.Now.Year} Lumo - Twoje miejsce na wspomnienia.<br>
                Zatrzymaj chwile, które uciekają.
            </div>
        </div>
    </div>";

            var bodyBuilder = new BodyBuilder { HtmlBody = styledBody };
            message.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();

            await client.ConnectAsync(host, port, MailKit.Security.SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(username, password);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}