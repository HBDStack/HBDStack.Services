using FluentAssertions;
using HBDStack.Services.Transformation.Convertors;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HBDStack.Services.Transform.Tests.Convertors;

[TestClass]
public class ConvertorTests
{
    #region Methods

    [TestMethod]
    public void Convertor_Tests()
    {
        var c = new ValueFormatter();

        c.Convert(null, null).Should().Be("");
        c.Convert(null, 123456).Should().Be("123,456");
        c.Convert(null, 1234.56m).Should().Be("1,234.56");
        c.Convert(null, DateTime.Now).Should().Contain(DateTime.Now.ToString("dd/MM/yyyy"));
        c.Convert(null, DateTimeOffset.Now).Should().Contain(DateTime.Now.ToString("dd/MM/yyyy"));
        c.Convert(null, true).Should().Be("Yes");
        c.Convert(null, 123456L).Should().Be("123,456");
        c.Convert(null, 123456.78d).Should().Be("123,456.78");
        c.Convert(null, (float) 123456.70).Should().Be("123,456.70");
    }

    #endregion Methods
}