using FluentAssertions;
using HBDStack.Services.Transformation.TokenDefinitions;
using HBDStack.Services.Transformation.TokenExtractors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HBDStack.Services.Transform.Tests.TokenExtractors;

[TestClass]
public class TokenExtractorTests
{
    #region Methods

    [TestMethod]
    public void TokenExtractor_AngledBracket_Test()
    {
        var t = new TokenExtractor(new AngledBracketDefinition());

        t.Extract("Hoang <Duy> Bao").ToList()
            .Should().HaveCount(1)
            .And.Subject.First().Token.Should().Be("<Duy>");

        t.Extract("Hoang Duy Bao").ToList()
            .Should().HaveCount(0);

        t.Extract("").ToList()
            .Should().HaveCount(0);

        t.Extract((string) null).ToList()
            .Should().HaveCount(0);

        var list = t.Extract(
                "Hoang <Duy> Bao, Hoang <Duy> Bao, Hoang <Duy> Bao, Hoang <Duy> Bao, Hoang <Duy> Bao, Hoang <Duy> Bao, Hoang <Duy> Bao, Hoang <Duy> Bao, Hoang <Duy> Bao, Hoang <Duy> Bao, Hoang <Duy> Bao, Hoang <Duy>")
            .ToList();

        list.Should().HaveCount(12)
            .And.Subject.First().Key.Should().Be("Duy");
    }

    [TestMethod]
    public async Task TokenExtractor_Async_Test()
    {
        var t = new TokenExtractor(new CurlyBracketDefinition());

        (await t.ExtractAsync("Hoang {{Duy} Bao")).ToList()
            .Should().HaveCount(1);
    }

    [TestMethod]
    public void TokenExtractor_BracketsToken_Test()
    {
        var t = new TokenExtractor(new CurlyBracketDefinition());

        t.Extract("Hoang {Duy} Bao").ToList()
            .Should().HaveCount(1)
            .And.Subject.First().Token.Should().Be("{Duy}");

        t.Extract("Hoang Duy Bao").ToList()
            .Should().HaveCount(0);

        t.Extract("").ToList()
            .Should().HaveCount(0);

        t.Extract((string) null).ToList()
            .Should().HaveCount(0);

        var list = t.Extract(
                "Hoang {Duy} Bao, Hoang {Duy} Bao, Hoang {Duy} Bao, Hoang {Duy} Bao, Hoang {Duy} Bao, Hoang {Duy} Bao, Hoang {Duy} Bao, Hoang {Duy} Bao, Hoang {Duy} Bao, Hoang {Duy} Bao, Hoang {Duy} Bao, Hoang {Duy}")
            .ToList();

        list.Should().HaveCount(12)
            .And.Subject.First().Key.Should().Be("Duy");
    }

    [TestMethod]
    public void TokenExtractor_InCorrect_Token_Test()
    {
        var t = new TokenExtractor(new CurlyBracketDefinition());

        t.Extract("Hoang ]Duy[ Bao").ToList()
            .Should().HaveCount(0);

        t.Extract("Hoang Duy Bao[[").ToList()
            .Should().HaveCount(0);
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void TokenExtractor_NullArgument_Test()
    {
        var t = new TokenExtractor(null);
    }

    [TestMethod]
    public void TokenExtractor_Support_DuplicateOfToken_Test()
    {
        var t = new TokenExtractor(new CurlyBracketDefinition());

        t.Extract("Hoang [{Duy}] Bao").ToList()
            .Should().HaveCount(1)
            .And.Subject.First().Token.Should().Be("{Duy}");
    }

    #endregion Methods
}