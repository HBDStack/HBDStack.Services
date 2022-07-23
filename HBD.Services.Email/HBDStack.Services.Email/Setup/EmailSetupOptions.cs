using HBDStack.Services.Email.Builders;
using HBDStack.Services.Transformation;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public class EmailSetupOptions
{
    #region Properties

    internal string FromEmail { get; private set; }

    internal string JsonFile { get; private set; }

    internal IConfigurationSection ConfigSection { get; private set; }

    internal Func<SmtpClient> SmtpClientFactory { get; private set; }

    internal Action<IEmailTemplateBuilder> TemplateBuilder { get; private set; }

    internal Action<TransformOptions> TransformOptions { get; private set; }
    #endregion Properties

    #region Methods

    public EmailSetupOptions EmailTemplateFrom(Action<IEmailTemplateBuilder> builder)
    {
        TemplateBuilder = builder;
        return this;
    }

    public EmailSetupOptions EmailTemplateFromFile(string jsonFile)
    {
        JsonFile = jsonFile;
        return this;
    }

    public EmailSetupOptions FromEmailAddress(string email)
    {
        FromEmail = email;
        return this;
    }

    public EmailSetupOptions WithSmtp(Func<SmtpClient> smtpClientFactory)
    {
        SmtpClientFactory = smtpClientFactory;
        return this;
    }

    /// <summary>
    /// The Configuration need to be provided both SmtpClient and Templates info.
    /// </summary>
    /// <returns></returns>
    public EmailSetupOptions FromConfiguration(IConfigurationSection section)
    {
        SmtpClientFactory = null;
        ConfigSection = section;
        return this;
    }

    public EmailSetupOptions WithTransformer(Action<TransformOptions> transformOptions)
    {
        TransformOptions = transformOptions;
        return this;
    }
    #endregion Methods
}