using System.Net.Mail;
using HBDStack.Services.Email.Builders;

namespace HBDStack.Services.Email.Providers;

public interface IMailMessageProvider : IDisposable
{
    Task<MailMessage> GetMailMessageAsync(EmailInfo emailInfo);
}