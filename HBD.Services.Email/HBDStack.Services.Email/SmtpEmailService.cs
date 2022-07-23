using System.Net.Mail;
using HBDStack.Services.Email.Builders;
using HBDStack.Services.Email.Exceptions;
using HBDStack.Services.Email.Providers;

namespace HBDStack.Services.Email;

public class SmtpEmailService : IEmailService
{
 

    private readonly IMailMessageProvider _mailMessageProvider;
    private readonly SmtpEmailOptions _options;
    private MailAddress _fromEmail;
    private bool _initialized;
    private SmtpClient _smtpClient;

    public SmtpEmailService(IMailMessageProvider mailMessageProvider)
        : this(mailMessageProvider, null)
    {
    }

    public SmtpEmailService(IMailMessageProvider mailMessageProvider, SmtpEmailOptions options)
    {
        _mailMessageProvider = mailMessageProvider ?? throw new ArgumentNullException(nameof(mailMessageProvider));
        _options = options;
    }

    public void Dispose()
    {
        _smtpClient?.Dispose();
        _fromEmail = null;
    }

    public virtual async Task SendAsync(EmailInfo emailInfo)
    {
        var email = await _mailMessageProvider.GetMailMessageAsync(emailInfo)
            .ConfigureAwait(false);

        if (email == null)
            throw new TemplateNotFoundException(emailInfo.TemplateName);

        await SendAsync(email).ConfigureAwait(false);
    }

    public virtual Task SendAsync(MailMessage email)
    {
        EnsureInitialized();
        return _smtpClient.SendMailAsync(ConsolidateEmail(email));
    }

    private MailMessage ConsolidateEmail(MailMessage mailMessage)
    {
        EnsureInitialized();

        if (mailMessage.From == null)
            mailMessage.From = _fromEmail;

        if (mailMessage.From == null)
            throw new ArgumentException(nameof(mailMessage.From));

        return mailMessage;
    }

    private void EnsureInitialized()
    {
        if (_initialized) return;

        _smtpClient = _options?.SmtpClientFactory() ?? new SmtpClient();
        _fromEmail = _options?.FromEmailAddress;

        _initialized = true;
    }
}