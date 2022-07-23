using FluentAssertions;
using HBDStack.Services.Email.Providers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HBDStack.Services.Email.Providers.Concretes;

namespace HBDStack.Services.Email.Tests;

[TestClass]
public class JsonEmailTemplateProviderTests
{
    #region Methods

    [TestMethod]
    public async Task ConfigTemplateAsync()
    {
        var s = new ServiceCollection()
            .AddEmailService(op =>
            {
                op.FromEmailAddress("system@hbd.com")
                    .WithSmtp(() => new System.Net.Mail.SmtpClient())
                    .EmailTemplateFromFile("TestData/Emails.json");
            })
            .BuildServiceProvider();

        var p = s.GetService<IEmailTemplateProvider>();
        p.Should().NotBeNull();
        p.Should().BeOfType<JsonEmailTemplateProvider>();
        (await p.GetTemplate("Duy2")).Should().NotBeNull();
    }

    [TestMethod]
    public async Task TestJsonProvider()
    {
        using var p = new JsonEmailTemplateProvider("TestData/Emails.json");
        var template = await p.GetTemplate("Duy1");

        template.Should().NotBeNull();
        template.Name.Should().NotBeNull();
        template.Subject.Should().NotBeNull();

        template = await p.GetTemplate("Duy2");

        template.Should().NotBeNull();
        template.Name.Should().NotBeNull();
        template.Subject.Should().NotBeNull();
    }

    #endregion Methods
}