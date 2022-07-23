using System.Diagnostics;
using System.Runtime.CompilerServices;
using HBDStack.Services.Transformation.Exceptions;
using HBDStack.Services.Transformation.TokenDefinitions;

[assembly: InternalsVisibleTo("HBDStack.Services.Transform.Tests")]

namespace HBDStack.Services.Transformation.TokenExtractors;

[DebuggerDisplay("Token = {" + nameof(Token) + "}")]
internal sealed class TokenResult : IToken
{
    private string _key;
        
    /// <summary>
    /// The Token result
    /// </summary>
    /// <param name="definition"></param>
    /// <param name="token"></param>
    /// <param name="originalString"></param>
    /// <param name="index"></param>
    public TokenResult(ITokenDefinition definition, string token, string originalString, int index)
    {
        Index = index;
        OriginalString = originalString ?? throw new ArgumentNullException(nameof(originalString));
        Definition = definition ?? throw new ArgumentNullException(nameof(definition));
        Token = token ?? throw new ArgumentNullException(nameof(token));

        if (!Definition.IsToken(Token))
            throw new InvalidTokenException(token);

        if (index < 0 || index > originalString.Length)
            throw new ArgumentOutOfRangeException(nameof(index));
    }
        
    public ITokenDefinition Definition { get; }

    public int Index { get; }

    public string Key
    {
        get
        {
            if (!string.IsNullOrWhiteSpace(_key)) return _key;
            _key = Token.Replace(Definition.Begin, string.Empty).Replace(Definition.End, string.Empty);
            return _key;
        }
    }

    public string OriginalString { get; }

    public string Token { get; }
}