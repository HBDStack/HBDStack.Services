using HBDStack.Services.Email;
using HBDStack.Services.Email.Providers;
using HBDStack.Services.Email.Templates;
using HBDStack.Services.Transformation;
using Microsoft.Extensions.Options;
using System.Net;
using HBDStack.Services.Email.Providers.Concretes;

// ReSharper disable CheckNamespace

namespace Microsoft.Extensions.DependencyInjection;

public static class SetupExtensions
{
    public static IServiceCollection AddEmailService(this IServiceCollection services,
        Action<EmailSetupOptions> options)
    {
        var op = new EmailSetupOptions();
        options?.Invoke(op);

        if (op.SmtpClientFactory != null)
            services.AddSingleton(new SmtpEmailOptions
            {
                FromEmailAddress = new System.Net.Mail.MailAddress(op.FromEmail),
                SmtpClientFactory = op.SmtpClientFactory
            });

        if (op.ConfigSection != null)
        {
            services.Configure<EmailTemplateSection>(op.ConfigSection);

            if (op.SmtpClientFactory == null)
                services.AddSingleton(p =>
                {
                    var config = p.GetRequiredService<IOptions<EmailTemplateSection>>().Value;

                    return new SmtpEmailOptions
                    {
                        FromEmailAddress = new System.Net.Mail.MailAddress(config.FromEmail),
                        SmtpClientFactory = () => new System.Net.Mail.SmtpClient(config.Host, config.Port)
                        {
                            Credentials = string.IsNullOrWhiteSpace(config.UserName)
                                ? null
                                : new NetworkCredential(config.UserName, config.Password),
                            EnableSsl = config.EnableSsl
                        }
                    };
                });
        }

        return services.AddSingleton(op)
            .AddTemplateProvider(op)
            .AddEmailServiceOnly(op.TransformOptions);
    }

    private static IServiceCollection AddEmailServiceOnly(this IServiceCollection services,
        Action<TransformOptions> transformOptions)
        => services.AddScoped<IMailMessageProvider, MailMessageProvider>()
            .AddTransformerService(transformOptions)
            .AddScoped<IEmailService>(p =>
            {
                var mail = p.GetRequiredService<IMailMessageProvider>();
                var options = p.GetService<SmtpEmailOptions>();

                return new SmtpEmailService(mail, options);
            });

    private static IServiceCollection AddTemplateProvider(this IServiceCollection services,
        EmailSetupOptions options)
    {
        if (options.TemplateBuilder != null)
            services.AddSingleton<IEmailTemplateProvider>(new InlineEmailTemplateProvider(options.TemplateBuilder));

        if (!string.IsNullOrWhiteSpace(options.JsonFile))
            services.AddSingleton<IEmailTemplateProvider>(new JsonEmailTemplateProvider(options.JsonFile));

        if (options.ConfigSection != null)
            services.AddSingleton<IEmailTemplateProvider, AppSettingTemplateProvider>();

        return services;
    }
}