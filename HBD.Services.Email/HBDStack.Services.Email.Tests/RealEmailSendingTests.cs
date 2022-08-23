using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using HBDStack.Services.Email.Builders;

namespace HBDStack.Services.Email.Tests;

[TestClass]
public class RealEmailSendingTests
{

    [TestMethod]
    public async System.Threading.Tasks.Task SendEmail_UsingMailTrap_Async()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("TestData/appsettings.json")
            .Build();

        var s = new ServiceCollection()
            .AddEmailService(op => op.FromConfiguration(config.GetSection("Smtp")))
            .BuildServiceProvider();

        var sender = s.GetRequiredService<IEmailService>();

        await sender.SendAsync(new EmailInfo("Duy1")
        {
            ToEmails = new []{"baoduy2412@yahoo.com.com"},
            Parameters =  new { Name = "Duy" } 
        });
    }

    [TestMethod]
    public async System.Threading.Tasks.Task SendEmail_SMTP4DEV_Async()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("TestData/sendgridsettings.json")
            .Build();

        var s = new ServiceCollection()
            .AddEmailService(op => op.FromConfiguration(config.GetSection("Smtp")))
            .BuildServiceProvider();

        var sender = s.GetRequiredService<IEmailService>();

        await sender.SendAsync(new EmailInfo("Duy1")
        {
            ToEmails = new []{"baoduy2412@yahoo.com"},
            Parameters =  new { Name = "Duy" } 
        });
    }
}