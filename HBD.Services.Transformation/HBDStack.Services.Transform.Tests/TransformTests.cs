using FluentAssertions;
using HBDStack.Services.Transformation;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HBDStack.Services.Transform.Tests;

[TestClass]
public class TransformTests
{
    #region Methods

    [TestMethod]
    public async Task Transform_HugeTemplate_Async__DisableCache_Test()
    {
        var d = Path.GetDirectoryName(typeof(TransformTests).Assembly.Location);
        var template = await File.ReadAllTextAsync(d + "/TestData/Data.txt");

        var t = new TransformerService(op => op.DisabledLocalCache = true);
        var s = await t.TransformAsync(template, new {A = "Hoang", B = "Bao", C = "Duy", D = "HBD"});

        s.Should().ContainAll("Hoang", "Bao", "Duy", "HBD")
            .And.Subject.Should().NotContainAny("{", "[", "<");
    }

    [TestMethod]
    public async Task Transform_HugeTemplate_Async_DataProvider_Test()
    {
        var d = Path.GetDirectoryName(typeof(TransformTests).Assembly.Location);
        var template = await File.ReadAllTextAsync(d + "/TestData/Data.txt");

        var t = new TransformerService();
        var s = await t.TransformAsync(template, token => Task.FromResult("Duy" as object));

        s.Should().Contain("Duy")
            .And.Subject.Should().NotContainAny("{", "[", "<");
    }

    [TestMethod]
    public async Task Transform_HugeTemplate_Async_Test()
    {
        var d = Path.GetDirectoryName(typeof(TransformTests).Assembly.Location);
        var template = await File.ReadAllTextAsync(d + "/TestData/Data.txt");

        var t = new TransformerService();
        var s = await t.TransformAsync(template, new {A = "Hoang", B = "Bao", C = "Duy", D = "HBD"});

        s.Should().ContainAll("Hoang", "Bao", "Duy", "HBD")
            .And.Subject.Should().NotContainAny("{", "[", "<");
    }

    [TestMethod]
    public async Task TransformAsync_DefaultData_Test()
    {
        var t = new TransformerService(op => op.GlobalParameters = new object[] {new {A = "Bao"}});
        var s = await t.TransformAsync("Hoang [A] Duy");
        s.Should().Be("Hoang Bao Duy");
    }

    [TestMethod]
    public async Task TransformAsync_Test()
    {
        var t = new TransformerService();
        var s = await t.TransformAsync("Hoang [A] Duy", new {A = "Bao"});
        s.Should().Be("Hoang Bao Duy");
    }

        
    [TestMethod]
    public async Task TransformAsync_Custom_Test()
    {
        var t = new TransformerService();
        var s = await t.TransformAsync("Hoang [[A]] Duy", new {A = "Bao"});
        s.Should().Be("Hoang [Bao] Duy");
    }
    #endregion Methods
}