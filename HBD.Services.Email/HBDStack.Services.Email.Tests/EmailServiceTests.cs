using FluentAssertions;
using HBDStack.Services.Email.Providers;
using HBDStack.Services.Email.Templates;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using netDumbster.smtp;
using System.Net.Mail;
using HBDStack.Services.Email.Builders;
using HBDStack.Services.Email.Providers.Concretes;
using HBDStack.Services.Transformation;

namespace HBDStack.Services.Email.Tests;

[TestClass]
public class EmailServiceTests
{
    #region Fields

    private static SimpleSmtpServer _smtpServer;

    #endregion Fields

    #region Methods

    [AssemblyInitialize]
    public static void AppDomainSetup(TestContext context)
        => _smtpServer = SimpleSmtpServer.Start(25);

    [AssemblyCleanup]
    public static void Cleanup() => _smtpServer.Stop();

    [TestMethod]
    public async Task Test_MailMessageProvider_ByTemplate()
    {
        var emailTemplateProviderMoq = new Mock<IEmailTemplateProvider>();
        emailTemplateProviderMoq.Setup(e => e.GetTemplate(It.IsAny<string>()))
            .ReturnsAsync(new EmailTemplate
            {
                Name = "Duy",
                Body = "Hello [Name]",
                Subject = "Hi, [Name]"
            }).Verifiable();

        using (var mailProvider = new MailMessageProvider(emailTemplateProviderMoq.Object, new TransformerService()))
        {
            var mail = await mailProvider.GetMailMessageAsync(new EmailInfo("Duy1")
            {
                ToEmails = new []{"duy@hbd.com"},
                Parameters =  new { Name = "Duy" }
            });

            mail.Should().NotBeNull();
            mail.Subject.Should().Contain("Duy");
            mail.Body.Should().Contain("Duy");
        }

        emailTemplateProviderMoq.VerifyAll();
    }

    [TestMethod]
    public async Task Test_EmptyAddress_ShouldBe_Ignore()
    {
        var emailTemplateProviderMoq = new Mock<IEmailTemplateProvider>();
        emailTemplateProviderMoq.Setup(e => e.GetTemplate(It.IsAny<string>()))
            .ReturnsAsync(new EmailTemplate
            {
                Name="Duy",
                Body = "Hello [Name]",
                Subject = "Hi, [Name]"
            }).Verifiable();

        using (var mailProvider = new MailMessageProvider(emailTemplateProviderMoq.Object, new TransformerService()))
        {
            var mail = await mailProvider.GetMailMessageAsync(new EmailInfo("Duy1")
            {
                ToEmails = new []{"duy@hbd.com"},
                Parameters = new { Name = "Duy"  }
            });

            mail.Should().NotBeNull();
        }

        emailTemplateProviderMoq.VerifyAll();
    }

    [TestMethod]
    public async Task Test_TransformTheSameTemplate_ButDifferentData_EmailDifference()
    {
        var emailTemplateProviderMoq = new Mock<IEmailTemplateProvider>();
        emailTemplateProviderMoq.Setup(e => e.GetTemplate(It.IsAny<string>()))
            .ReturnsAsync(new EmailTemplate
            {
                Name="Duy",
                Body = "Hello [Name]",
                Subject = "Hi, [Name]"
            }).Verifiable();

        using (var mailProvider = new MailMessageProvider(emailTemplateProviderMoq.Object, new TransformerService(o=>o.DisabledLocalCache=true)))
        {
            var mail1 = await mailProvider.GetMailMessageAsync(new EmailInfo("Duy1")
            {
                ToEmails = new []{"duy1@hbd.com"},
                Parameters =  new { Name = "Duy1" }
            });

            var mail2 = await mailProvider.GetMailMessageAsync(new EmailInfo("Duy1")
            {
                ToEmails = new []{"duy2@hbd.com"},
                Parameters =  new { Name = "Duy2" }
            });

            mail1.To.First().Address.Should().NotBe(mail2.To.First().Address);
            mail1.Subject.Should().NotBe(mail2.Subject);
            mail1.Body.Should().NotBe(mail2.Body);
        }

        emailTemplateProviderMoq.VerifyAll();
    }
    [TestMethod]
    public async Task Test_SendEmail_ByTemplate()
    {
        _smtpServer.ClearReceivedEmail();

        var emailTemplateProviderMoq = new Mock<IEmailTemplateProvider>();
        emailTemplateProviderMoq.Setup(e => e.GetTemplate(It.IsAny<string>())).ReturnsAsync(new EmailTemplate
        {
            Name="Duy",
            Body = "Hello [Name]",
        });

        using (var mailService = new SmtpEmailService(
                   new MailMessageProvider(emailTemplateProviderMoq.Object, new TransformerService()), new SmtpEmailOptions
                   {
                       FromEmailAddress = new MailAddress("drunkcoding@outlook.net"),
                       SmtpClientFactory = () => new SmtpClient("localhost", 25)
                   }))
        {
            await mailService.SendAsync(new EmailInfo("Duy1")
            {
                ToEmails = new []{"duy@hbd.com"},
                Parameters =  new { Name = "Duy" } 
            });
        }

        _smtpServer.ReceivedEmailCount.Should().BeGreaterOrEqualTo(1);
    }

    [TestMethod]
    public async Task Test_SendEmail_ByTemplate_WithDictionary()
    {
        _smtpServer.ClearReceivedEmail();

        var emailTemplateProviderMoq = new Mock<IEmailTemplateProvider>();
        emailTemplateProviderMoq.Setup(e => e.GetTemplate(It.IsAny<string>())).ReturnsAsync(new EmailTemplate
        {
            Name="Duy",
            Body = "Hello [Name]",
        });

        using (var mailService = new SmtpEmailService(
                   new MailMessageProvider(emailTemplateProviderMoq.Object, new TransformerService()), new SmtpEmailOptions
                   {
                       FromEmailAddress = new MailAddress("drunkcoding@outlook.net"),
                       SmtpClientFactory = () => new SmtpClient("localhost", 25)
                   }))
        {
            await mailService.SendAsync(new EmailInfo("Duy1")
            {
                ToEmails = new []{"duy@hbd.com"},
                Parameters = new Dictionary<string,object> { ["Name"]="Duy" }
            });
        }

        _smtpServer.ReceivedEmailCount.Should().BeGreaterOrEqualTo(1);
    }

    #endregion Methods
}