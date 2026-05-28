using System.Net;
using System.Net.Mail;
using BadmintonApp.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BadmintonApp.Infrastructure.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration config, ILogger<EmailService> logger)
    {
        _config = config;
        _logger = logger;
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body)
    {
        var smtpServer = _config["EmailSettings:SmtpServer"] ?? "smtp.gmail.com";
        var smtpPortStr = _config["EmailSettings:SmtpPort"] ?? "587";
        var senderName = _config["EmailSettings:SenderName"] ?? "Ghép Kèo Cầu Lông Biên Hòa";
        var senderEmail = _config["EmailSettings:SenderEmail"] ?? "noreply.badmintonapp@gmail.com";
        var username = _config["EmailSettings:Username"] ?? "";
        var password = _config["EmailSettings:Password"] ?? "";
        var enableSslStr = _config["EmailSettings:EnableSsl"] ?? "true";

        int.TryParse(smtpPortStr, out int smtpPort);
        bool.TryParse(enableSslStr, out bool enableSsl);

        // Ghi log email ra console
        _logger.LogInformation("==================================================");
        _logger.LogInformation("GỬI EMAIL THÔNG BÁO:");
        _logger.LogInformation($"To: {toEmail}");
        _logger.LogInformation($"Subject: {subject}");
        _logger.LogInformation($"Body:\n{body}");
        _logger.LogInformation("==================================================");

        // Đồng thời ghi ra file trong thư mục App Data hoặc Workspace scratch để kiểm tra
        try
        {
            var emailLogDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "EmailLogs");
            if (!Directory.Exists(emailLogDir))
            {
                Directory.CreateDirectory(emailLogDir);
            }
            var logPath = Path.Combine(emailLogDir, $"email_{DateTime.Now:yyyyMMdd_HHmmss}_{Guid.NewGuid().ToString().Substring(0, 8)}.html");
            var fileContent = $"<h3>To: {toEmail}</h3><h3>Subject: {subject}</h3><hr/><div>{body.Replace("\n", "<br/>")}</div>";
            await File.WriteAllTextAsync(logPath, fileContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Không thể ghi file log email.");
        }

        // Nếu có điền username và password thì tiến hành gửi email thật qua SMTP
        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        {
            try
            {
                using var client = new SmtpClient(smtpServer, smtpPort)
                {
                    Credentials = new NetworkCredential(username, password),
                    EnableSsl = enableSsl
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(senderEmail, senderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(toEmail);

                await client.SendMailAsync(mailMessage);
                _logger.LogInformation($"Đã gửi email thành công qua SMTP tới {toEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Gửi email thật qua SMTP thất bại. Chi tiết: {ex.Message}");
            }
        }
        else
        {
            _logger.LogWarning("Chưa cấu hình tài khoản Email SMTP (Username/Password trống). Email thật sẽ không được gửi đi, xem log email ở console hoặc thư mục EmailLogs.");
        }
    }
}
