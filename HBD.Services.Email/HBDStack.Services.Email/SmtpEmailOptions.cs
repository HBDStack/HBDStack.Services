using System.Net.Mail;

namespace HBDStack.Services.Email;

public class SmtpEmailOptions
{
    #region Properties

    public MailAddress FromEmailAddress { get; set; }

    public Func<SmtpClient> SmtpClientFactory { get; set; } = () => new SmtpClient();

    #endregion Properties
}