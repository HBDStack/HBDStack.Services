using FluentAssertions;
using HBDStack.Services.Transformation.Exceptions;
using HBDStack.Services.Transformation.TokenDefinitions;
using HBDStack.Services.Transformation.TokenExtractors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HBDStack.Services.Transform.Tests.TokenExtractors;

[TestClass]
public class TokenResultTests
{
    #region Methods

    [TestMethod]
    public void Create_TokenResult()
    {
        var t = new TokenResult(new CurlyBracketDefinition(), "{A}", "123 {A}", 4);

        t.Definition.Should().BeOfType<CurlyBracketDefinition>();
        t.Key.Should().Be("A");
        t.Key.Should().Be("A");
        t.Index.Should().Be(4);
        t.OriginalString.Should().Be("123 {A}");
    }
        
    [TestMethod]
    public void Create_Custom_TokenResult()
    {
        var t = new TokenResult(new TokenDefinition("{{","}}"), "{{A}}", "123 {{A}}", 4);

        t.Definition.Should().BeOfType<TokenDefinition>();
        t.Key.Should().Be("A");
        t.Key.Should().Be("A");
        t.Index.Should().Be(4);
        t.OriginalString.Should().Be("123 {{A}}");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Create_TokenResult_Definition_Is_Null()
    {
        var t = new TokenResult(null, "[A]", "123 [A]", 1);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Create_TokenResult_In_Correct_Index()
    {
        var t = new TokenResult(new CurlyBracketDefinition(), "{A}", "123 {A}", -1);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentOutOfRangeException))]
    public void Create_TokenResult_In_Correct_Index2()
    {
        var t = new TokenResult(new CurlyBracketDefinition(), "{A}", "123 {A}", 100);
    }

    [TestMethod]
    [ExpectedException(typeof(InvalidTokenException))]
    public void Create_TokenResult_In_Correct_Token()
    {
        var t = new TokenResult(new CurlyBracketDefinition(), "{A", "123 {A}", 1);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Create_TokenResult_OriginalString_Is_Null()
    {
        var t = new TokenResult(new SquareBracketDefinition(), "[A]", null, 1);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Create_TokenResult_Token_Is_Null()
    {
        var t = new TokenResult(new SquareBracketDefinition(), null, "123 [A]", 1);
    }

    #endregion Methods
}