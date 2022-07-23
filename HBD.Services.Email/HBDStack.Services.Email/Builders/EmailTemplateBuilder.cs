using HBDStack.Services.Email.Exceptions;
using HBDStack.Services.Email.Templates;

namespace HBDStack.Services.Email.Builders;

public interface IEmailBuilder
{
    #region Methods

    /// <summary>
    /// Add more template
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    IEmailBuilder Add(string name);

    /// <summary>
    /// Non Html body
    /// </summary>
    /// <param name="body"></param>
    /// <returns></returns>
    IEmailBuilder Body(string body);

    /// <summary>
    /// Load body from Html file
    /// </summary>
    /// <param name="file"></param>
    /// <returns></returns>
    IEmailBuilder BodyFrom(string file);

    /// <summary>
    /// Build Templates
    /// </summary>
    /// <returns></returns>
    ICollection<IEmailTemplate> Build();

    IEmailBuilder Subject(string subject);

    #endregion Methods
}

public interface IEmailTemplateBuilder
{
    IEmailBuilder Add(string name);

    ICollection<IEmailTemplate> Build();
        
}

public class EmailTemplateBuilder : IEmailTemplateBuilder, IEmailBuilder
{
    private EmailTemplate _current;

    private readonly IDictionary<string, IEmailTemplate> _templates;
        
    internal EmailTemplateBuilder(IDictionary<string, IEmailTemplate> container = null) => _templates = container ?? new Dictionary<string, IEmailTemplate>();
        
    public static IEmailBuilder New(string name) => new EmailTemplateBuilder().Add(name);

    public IEmailBuilder Add(string name)
    {
        if (_current is { IsValid: false })
            throw new InvalidTemplateException(_current);

        _current = new EmailTemplate{Name = name};
        _templates.Add(_current.Name.ToUpper(), _current);

        return this;
    }

    public IEmailBuilder Body(string body)
    {
        if (string.IsNullOrEmpty(body)) throw new ArgumentNullException(nameof(body));
        _current.Body = body;
        _current.IsBodyHtml = false;
        return this;
    }

    public IEmailBuilder BodyFrom(string file)
    {
        if (!System.IO.File.Exists(file)) throw new ArgumentNullException(nameof(file));
        _current.BodyFile = file;
        _current.IsBodyHtml = true;
        return this;
    }

    public ICollection<IEmailTemplate> Build()
    {
        var invalid = _templates.Values.OfType<EmailTemplate>().FirstOrDefault(v => !v.IsValid);

        if (invalid != null)
            throw new InvalidTemplateException(invalid);

        return _templates.Values;
    }

    public IEmailBuilder Subject(string subject)
    {
        if (string.IsNullOrEmpty(subject)) throw new ArgumentNullException(nameof(subject));
        _current.Subject = subject;

        return this;
    }
        
    public IEmailBuilder With() => this;
}