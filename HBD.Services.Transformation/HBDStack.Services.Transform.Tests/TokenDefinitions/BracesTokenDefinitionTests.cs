using FluentAssertions;
using HBDStack.Services.Transformation.TokenDefinitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HBDStack.Services.Transform.Tests.TokenDefinitions;

[TestClass]
public class BracesTokenDefinitionTests
{
    #region Methods

    [TestMethod]
    public void BracesTokenDefinitionTest()
    {
        var t = new SquareBracketDefinition();

        t.IsToken("[Duy]")
            .Should().BeTrue();

        t.IsToken("{Duy}")
            .Should().BeFalse();

        t.IsToken("<Duy")
            .Should().BeFalse();

        t.IsToken("Duy>")
            .Should().BeFalse();
    }

    #endregion Methods
}