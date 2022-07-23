using FluentAssertions;
using HBDStack.Services.Transformation.TokenDefinitions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HBDStack.Services.Transform.Tests.TokenDefinitions;

[TestClass]
public class AngledBracketTokenDefinitionTests
{
    #region Methods

    [TestMethod]
    public void AngledBracketTokenDefinitionTest()
    {
        var t = new AngledBracketDefinition();

        t.IsToken("<Duy>")
            .Should().BeTrue();

        t.IsToken("[Duy]")
            .Should().BeFalse();

        t.IsToken("<Duy")
            .Should().BeFalse();

        t.IsToken("Duy>")
            .Should().BeFalse();
    }

    #endregion Methods
}