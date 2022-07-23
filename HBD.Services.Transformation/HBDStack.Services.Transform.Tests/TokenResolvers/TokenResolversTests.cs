using FluentAssertions;
using HBDStack.Services.Transformation.TokenDefinitions;
using HBDStack.Services.Transformation.TokenExtractors;
using HBDStack.Services.Transformation.TokenResolvers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HBDStack.Services.Transform.Tests.TokenResolvers;

[TestClass]
public class TokenResolversTests
{
    #region Methods

    [TestMethod]
    public void Test_TokenResolver()
    {
        var resolver = new TokenResolver();

        var val = resolver.Resolve(new TokenResult(new CurlyBracketDefinition(), "{A}", "{A} 123", 0), new object[]
        {
            null,
            new {A = (string) null},
            new {A = 123}
        });

        val.Should().Be(123);
    }

    [TestMethod]
    public async Task Test_TokenResolver_Async()
    {
        var resolver = new TokenResolver();

        var val = await resolver.ResolveAsync(new TokenResult(new CurlyBracketDefinition(), "{A}", "{A} 123", 0), new object[]
        {
            null,
            new {A = (string) null},
            new {A = 123}
        });

        val.Should().Be(123);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Test_TokenResolver_Data_Empty()
    {
        var resolver = new TokenResolver();

        var val = resolver.Resolve(new TokenResult(new CurlyBracketDefinition(), "{A}", "{A} 123", 0), new object[0]);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Test_TokenResolver_Data_Null()
    {
        var resolver = new TokenResolver();

        var val = resolver.Resolve(new TokenResult(new CurlyBracketDefinition(), "{A}", "{A} 123", 0), null);
    }

    [TestMethod]
    public async Task Test_TokenResolver_Dictionary()
    {
        var resolver = new TokenResolver();

        var val = await resolver.ResolveAsync(new TokenResult(new CurlyBracketDefinition(), "{A}", "{A} 123", 0), new Dictionary<string, object>
        {
            {"A", "Duy"}
        });

        val.Should().Be("Duy");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentException))]
    public async Task Test_TokenResolver_OnlyDictionary_String_Object_Is_Supportted()
    {
        var resolver = new TokenResolver();

        var val = await resolver.ResolveAsync(new TokenResult(new CurlyBracketDefinition(), "{A}", "{A} 123", 0), new Dictionary<object, object>
        {
            {"A", "Duy"}
        });

        val.Should().Be("Duy");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Test_TokenResolver_Token_Null()
    {
        var resolver = new TokenResolver();

        var val = resolver.Resolve(null, new object[]
        {
            null,
            new {A = (string) null},
            new {A = 123}
        });
    }

    #endregion Methods
}