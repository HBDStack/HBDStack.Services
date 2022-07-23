namespace HBDStack.Services.Transformation.TokenDefinitions;

public class TokenDefinition : ITokenDefinition
{
    public TokenDefinition(string begin, string end)
    {
        Begin = begin;
        End = end;
    }
        
    public string Begin { get; }
    public string End { get; }
        
    public bool IsToken(string value) => !string.IsNullOrWhiteSpace(value) && value.StartsWith(Begin) && value.EndsWith(End);

}