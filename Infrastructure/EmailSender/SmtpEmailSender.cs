using Application.Common.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.EmailSender
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        public SmtpEmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpServer = _config["EmailSettings:SmtpServer"];
            var port = int.Parse(_config["EmailSettings:Port"]);
            var fromEmail = _config["EmailSettings:FromEmail"];
            var username = _config["EmailSettings:Username"];
            var password = _config["EmailSettings:Password"];

            using var client = new SmtpClient(smtpServer, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = true
            };

            var mail = new MailMessage(fromEmail, toEmail, subject, body)
            {
                IsBodyHtml = true
            };

            await client.SendMailAsync(mail);
        }


        public string HtmlTemplateForOnlineOrder(string orderNumber, string customerName,string customerAddress, string phoneNumber)
        {
            var emailBody = $@"
    <div style='font-family: Arial, sans-serif; color: #333;'>
        <h2 style='color: #3b71ca;'>🛒 New Online Order Placed</h2>
        <p>Dear Admin,</p>
        <p>A new order has just been placed. Below are the details:</p>

        <table style='border-collapse: collapse; margin-top: 10px;'>
            <tr>
                <td style='padding: 8px; font-weight: bold;'>Order Number:</td>
                <td style='padding: 8px; color: #3b71ca;'>{orderNumber ?? "N/A"}</td>
            </tr>
            <tr>
                <td style='padding: 8px; font-weight: bold;'>Order Date:</td>
                <td style='padding: 8px;'>{DateTime.Now:dddd, MMMM dd yyyy hh:mm tt}</td>
            </tr>
            <tr>
                <td style='padding: 8px; font-weight: bold;'>Customer Name:</td>
                <td style='padding: 8px;'>{customerName ?? "N/A"}</td>
            </tr>
            <tr>
                <td style='padding: 8px; font-weight: bold;'>Address:</td>
                <td style='padding: 8px;'>{customerAddress ?? "N/A"}</td>
            </tr>
            <tr>
                <td style='padding: 8px; font-weight: bold;'>Phone Number:</td>
                <td style='padding: 8px;'>{phoneNumber ?? "N/A"}</td>
            </tr>
        </table>

        <p style='margin-top: 20px;'>Please log in to the admin dashboard to review and process this order.</p>

        <hr style='margin: 20px 0;' />
        <p style='font-size: 12px; color: #888;'>This is an automated message from your e-commerce system.</p>
    </div>";

            return emailBody;
        }

    }

}
