namespace HBDStack.Services.Transformation.TokenExtractors;

public interface ITokenExtractor
{
    #region Methods

    /// <summary>
    /// Extract token from string.
    /// </summary>
    /// <param name="template"></param>
    /// <returns></returns>
    IEnumerable<IToken> Extract(string template);

    /// <summary>
    /// Extract token from string.
    /// </summary>
    /// <param name="template"></param>
    /// <returns></returns>
    Task<IEnumerable<IToken>> ExtractAsync(string template);

    #endregion Methods
}