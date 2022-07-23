using HBDStack.Services.Email.Templates;

namespace HBDStack.Services.Email.Providers;

public interface IEmailTemplateProvider : IDisposable
{
    #region Methods

    Task<IEmailTemplate> GetTemplate(string templateName);

    #endregion Methods
}