using HBDStack.Services.Transformation.TokenDefinitions;

// ReSharper disable MemberCanBePrivate.Global

namespace HBDStack.Services.Transformation.TokenExtractors;

/// <summary>
/// The extractor of &lt;token&gt;
/// </summary>
public class AngledBracketTokenExtractor : TokenExtractor
{
    public AngledBracketTokenExtractor() : base(new AngledBracketDefinition())
    {
    }
}

/// <summary>
/// The extractor of {token}
/// </summary>
public class CurlyBracketExtractor : TokenExtractor
{
    public CurlyBracketExtractor() : base(new CurlyBracketDefinition())
    {
    }
}

/// <summary>
/// The extractor of [token]
/// </summary>
public class SquareBracketExtractor : TokenExtractor
{
    public SquareBracketExtractor() : base(new SquareBracketDefinition())
    {
    }
}

public class TokenExtractor : ITokenExtractor
{
    public TokenExtractor(ITokenDefinition definition) => Definition = definition ?? throw new ArgumentNullException(nameof(definition));

    protected ITokenDefinition Definition { get; }

    public IEnumerable<IToken> Extract(string template) => ExtractCore(template);

    public Task<IEnumerable<IToken>> ExtractAsync(string template) => Task.Run(() => ExtractCore(template));

    protected virtual IEnumerable<IToken> ExtractCore(string template)
    {
        if (string.IsNullOrWhiteSpace(template)) yield break;

        var length = template.Length;
        var si = template.IndexOf(Definition.Begin, StringComparison.Ordinal);

        while (si >= 0 && si < length)
        {
            //If next is begin of Token then move to next
            if (si < length - 1 && template.Substring(si + Definition.Begin.Length, Definition.Begin.Length) == Definition.Begin)
            {
                si += Definition.Begin.Length;
                continue;
            }

            var li = template.IndexOf(Definition.End, si, StringComparison.Ordinal);
            if (li <= si) break;

            yield return new TokenResult(Definition, template.Substring(si, li - si + Definition.Begin.Length), template, si);
            si = template.IndexOf(Definition.Begin, li, StringComparison.Ordinal);
        }
    }
}