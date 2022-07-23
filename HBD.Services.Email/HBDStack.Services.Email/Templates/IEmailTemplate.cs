namespace HBDStack.Services.Email.Templates;

public interface IEmailTemplate
{
    /// <summary>
    /// The email Body.
    /// </summary>
    string Body { get; set; }

    /// <summary>
    /// The email Body file, it can be a html file. The file content will be loaded to Body before send email out.
    /// </summary>
    string BodyFile { get; set; }

    /// <summary>
    /// Indicate the Body is in Html format or not
    /// </summary>
    bool IsBodyHtml { get; set; }

    /// <summary>
    /// The name of Template
    /// </summary>
    string Name { get; }

    /// <summary>
    /// The email Subject
    /// </summary>
    string Subject { get; set; }
}