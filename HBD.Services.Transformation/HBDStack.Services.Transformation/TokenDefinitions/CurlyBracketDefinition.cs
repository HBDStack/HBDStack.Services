namespace HBDStack.Services.Transformation.TokenDefinitions;

/// <summary>
/// Defination of Curly Brackets {token}
/// </summary>
public sealed class CurlyBracketDefinition : TokenDefinition
{
    public CurlyBracketDefinition() : base("{", "}")
    {
    }
}