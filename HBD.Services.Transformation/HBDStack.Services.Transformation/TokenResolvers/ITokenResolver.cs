using HBDStack.Services.Transformation.TokenExtractors;

namespace HBDStack.Services.Transformation.TokenResolvers;

public interface ITokenResolver
{
    #region Methods

    /// <summary>
    /// Get value from data based on toke <see cref="IToken"/>
    /// </summary>
    /// <param name="token"><see cref="IToken"/></param>
    /// <param name="data"> data object or IDictionary[string,object]</param>
    /// <returns>value found from data or NULL</returns>
    object Resolve(IToken token, params object[] data);

    /// <summary>
    /// Get value from data based on toke <see cref="IToken"/>
    /// </summary>
    /// <param name="token"><see cref="IToken"/></param>
    /// <param name="data"> data object or IDictionary[string,object]</param>
    /// <returns>value found from data or NULL</returns>
    Task<object> ResolveAsync(IToken token, params object[] data);

    #endregion Methods
}