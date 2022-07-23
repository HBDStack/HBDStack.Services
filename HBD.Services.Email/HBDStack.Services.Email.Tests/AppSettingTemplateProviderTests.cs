using FluentAssertions;
using HBDStack.Services.Email.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HBDStack.Services.Email.Providers.Concretes;

namespace HBDStack.Services.Email.Tests;

[TestClass]
public class AppSettingTemplateProviderTests
{
    #region Methods

    [TestMethod]
    public async Task ConfigTemplateAsync()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("TestData/appsettings.json")
            .Build();

        var section = config.GetSection("Smtp");

        var s = new ServiceCollection()
            .AddEmailService(op => op.FromConfiguration(section))
            .BuildServiceProvider();

        var p = s.GetService<IEmailTemplateProvider>();
        p.Should().NotBeNull();
        p.Should().BeOfType<AppSettingTemplateProvider>();
        (await p.GetTemplate("Duy2")).Should().NotBeNull();
    }

    #endregion Methods
}