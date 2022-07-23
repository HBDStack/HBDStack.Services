namespace HBDStack.Services.Email.Builders;

public class EmailInfo
{
    public EmailInfo(string templateName) => TemplateName = templateName;
        
    public string[] ToEmails { get; set; }
    public string[] CcEmails { get; set; }
    public string[] BccEmails { get; set; }
    public string TemplateName { get; }
    /// <summary>
    /// The Parameters can be a dynamic object or IDictionary<string,object>
    /// </summary>
    public object Parameters { get; set; }
}