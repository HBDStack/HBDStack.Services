using HBDStack.Services.Email.Templates;

namespace HBDStack.Services.Email.Exceptions;

public sealed class InvalidTemplateException : Exception
{
    #region Constructors

    public InvalidTemplateException(EmailTemplate template) : base($"The template {template.Name} is invalid.") => Template = template;

    #endregion Constructors

    #region Properties

    public EmailTemplate Template { get; }

    #endregion Properties
}