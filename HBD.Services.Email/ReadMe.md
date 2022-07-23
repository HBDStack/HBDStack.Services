# HBD.Services.Email

The library which allows sending email to the pre-defined template and provided data.
Based on prived data the tokens from the template will be transformed before send.

## How to config template and setting

### 1. With App Setting

**appSetting file**

```json
{
 ...
 "Smtp": {
  "Host": "smtp.mailtrap.io",
  "Port": 465,
  "UserName": "94fa19cc3d3881",
  "Password": "3aa3c2f3754bde",
  "EnableSsl": true,
  "FromEmail": "abc@hbd.com",

  "Templates": [
   {
    "Name": "Template Name 1",
    "ToEmails": "email or [token] separate by comma",
    "CcEmails": "email or [token] separate by comma",
    "BccEmails": "email or [token] separate by comma",
    "Subject": "Test Email",
    "Body": "Hello [Name]",
    "BodyFile": "The path to html file."
   },
   ...
  ]
 }
}
```

**Dependency injection**
```cshap
 var config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("TestData/appsettings.json")
        .Build();

 var section = config.GetSection("Smtp");

 var s = new ServiceCollection()
        .AddEmailService(op => op.FromConfiguration(section))
        .BuildServiceProvider();
```

### 2. With Separate Json File.

**Json Config file**

```json
[
 {
   "Name": "Template Name 1",
   "ToEmails": "email or [token] separate by comma",
   "CcEmails": "email or [token] separate by comma",
   "BccEmails": "email or [token] separate by comma",
   "Subject": "Test Email",
   "Body": "Hello [Name]",
   "BodyFile": "The path to html file."
 },
 ...
]
```

**Dependency injection**
```csharp
 var s = new ServiceCollection()
        .AddEmailService(op =>
        {
          op.FromEmailAddress("system@hbd.com")
            .WithSmtp(() => new System.Net.Mail.SmtpClient())
            .EmailTemplateFromFile("TestData/Emails.json");
        })
        .BuildServiceProvider();
```

### 3. Inline configuration.

```csharp
 var s = new ServiceCollection()
        .AddEmailService(op =>
        {
          op.EmailTemplateFrom(builder => builder.Add("Template 1")
          .To("duy@hbd.com")
          .With()
          .Subject("AA")
          .Body("BBB"));
        })
        .BuildServiceProvider();
```


## How to Send Email

```csharp
 var sender = s.GetService<IEmailService>();
 await sender.SendAsync("Template Name", new object[] { /*Array data object or dictionary for token transforming*/ }, string[]{/*Array of attchement files*/});
```