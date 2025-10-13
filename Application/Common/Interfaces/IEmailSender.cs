using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Interfaces
{
    public interface IEmailSender
    {
        string HtmlTemplateForOnlineOrder(string orderNumber, string customerName, string customerAddress, string phoneNumber);
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}
