namespace HBDStack.Services.Transformation.Exceptions;

public sealed class UnResolvedTokenException : Exception
{
    //public UnResolvedTokenException()
    //{
    //}

    #region Constructors

    public UnResolvedTokenException(string token) : this(token, null)
    {
    }

    public UnResolvedTokenException(string token, Exception innerException) : base(token, innerException)
    {
    }

    #endregion Constructors
}