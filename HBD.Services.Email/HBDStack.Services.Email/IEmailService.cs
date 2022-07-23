using HBDStack.Services.Email.Builders;
using HBDStack.Services.Email.Exceptions;

namespace HBDStack.Services.Email;

public interface IEmailService : IDisposable
{
    #region Methods

    /// <summary>
    ///
    /// </summary>
    /// <exception cref="TemplateNotFoundException">Template not found</exception>
    /// <exception cref="ArgumentNullException">when templateName is null</exception>
    /// <returns></returns>
    Task SendAsync(EmailInfo emailInfo);

    /// <summary>
    /// Send Email
    /// </summary>
    /// <param name="email"></param>
    /// <exception cref="ArgumentNullException">when email is null</exception>
    /// <returns></returns>
    Task SendAsync(System.Net.Mail.MailMessage email);

    #endregion Methods
}