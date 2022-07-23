using System.Net.Mail;
using HBDStack.Services.Transformation;
using HBDStack.Services.Email.Builders;
using HBDStack.Services.Email.Exceptions;
using HBDStack.Services.Email.Templates;

namespace HBDStack.Services.Email.Providers.Concretes;

public class MailMessageProvider : IMailMessageProvider
{
    private readonly IList<IEmailTemplateProvider> _emailTemplateProvider;
    private readonly ITransformerService _transformer;

    /// <summary>
    /// Use custom ITransformer
    /// </summary>
    /// <param name="emailTemplateProvider"></param>
    /// <param name="transformer"></param>
    public MailMessageProvider(IEnumerable<IEmailTemplateProvider> emailTemplateProvider,
        ITransformerService transformer)
    {
        _emailTemplateProvider = emailTemplateProvider.ToList();
        _transformer = transformer ?? throw new ArgumentNullException(nameof(transformer));
    }

    /// <summary>
    /// Use custom ITransformer
    /// </summary>
    /// <param name="emailTemplateProvider"></param>
    /// <param name="transformer"></param>
    protected internal MailMessageProvider(IEmailTemplateProvider emailTemplateProvider,
        ITransformerService transformer)
        : this(new[] { emailTemplateProvider }, transformer)
    {
    }

    public void Dispose()
    {
    }

    public virtual async Task<MailMessage> GetMailMessageAsync(EmailInfo emailInfo)
    {
        var template = await GetTemplate(emailInfo.TemplateName).ConfigureAwait(false);
        if (template == null)
            throw new TemplateNotFoundException(emailInfo.TemplateName);

        return await GetMailMessageAsync(template, emailInfo).ConfigureAwait(false);
    }

    protected async Task<MailMessage> GetMailMessageAsync(IEmailTemplate template, EmailInfo emailInfo)
    {
        var mail = new MailMessage();

        if (emailInfo.ToEmails != null)
            mail.To.AddRange(emailInfo.ToEmails.Select(e=>new MailAddress(e)));
        if (emailInfo.CcEmails != null)
            mail.To.AddRange(emailInfo.CcEmails.Select(e=>new MailAddress(e)));
        if (emailInfo.BccEmails != null)
            mail.To.AddRange(emailInfo.BccEmails.Select(e=>new MailAddress(e)));
            
        mail.Subject = await _transformer.TransformAsync(template.Subject, emailInfo.Parameters).ConfigureAwait(false);
        mail.Body = await _transformer.TransformAsync(template.Body, emailInfo.Parameters).ConfigureAwait(false);
        mail.IsBodyHtml = template.IsBodyHtml;

        return mail;
    }

    private async Task<IEmailTemplate> GetTemplate(string templateName)
    {
        foreach (var p in _emailTemplateProvider)
        {
            var t = await p.GetTemplate(templateName).ConfigureAwait(false);
            if (t != null) return t;
        }

        return null;
    }
}