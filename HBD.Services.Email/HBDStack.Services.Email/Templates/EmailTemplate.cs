namespace HBDStack.Services.Email.Templates;

public class EmailTemplate : IEmailTemplate
{
    /// <inheritdoc />
    public string Body { get; set; }

    /// <inheritdoc />
    public string BodyFile { get; set; }

    /// <inheritdoc />
    public bool IsBodyHtml { get; set; } = true;

    public bool IsValid
    {
        get
        {
            if (string.IsNullOrWhiteSpace(BodyFile))
                return !string.IsNullOrWhiteSpace(Body) && !string.IsNullOrWhiteSpace(Subject);

            return !string.IsNullOrWhiteSpace(Subject) && File.Exists(BodyFile);
        }
    }
        
    public string Name { get; set; }
        
    public string Subject { get; set; }
        
}