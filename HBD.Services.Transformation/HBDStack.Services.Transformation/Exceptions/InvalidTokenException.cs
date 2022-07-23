namespace HBDStack.Services.Transformation.Exceptions;

public sealed class InvalidTokenException : Exception
{
    //public InvalidTokenException()
    //{
    //}

    #region Constructors

    public InvalidTokenException(string token) : this(token, null)
    {
    }

    public InvalidTokenException(string token, Exception innerException) : base(token, innerException)
    {
    }

    #endregion Constructors
}