namespace HBDStack.Services.Email.Exceptions;

public sealed class TemplateNotFoundException : Exception
{
    #region Constructors

    public TemplateNotFoundException()
    {
    }

    public TemplateNotFoundException(string message) : base(message)
    {
    }

    public TemplateNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }

    #endregion Constructors
}