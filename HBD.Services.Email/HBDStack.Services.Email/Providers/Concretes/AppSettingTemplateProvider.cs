using HBDStack.Services.Email.Templates;
using Microsoft.Extensions.Options;

namespace HBDStack.Services.Email.Providers.Concretes;

public class AppSettingTemplateProvider : EmailTemplateProvider
{
    private readonly IOptions<EmailTemplateSection> _options;
    public AppSettingTemplateProvider(IOptions<EmailTemplateSection> options) => _options = options;
        
    protected override Task<IEnumerable<EmailTemplate>> LoadTemplatesAsync()
        => Task.FromResult((IEnumerable<EmailTemplate>)_options.Value.Templates);
}