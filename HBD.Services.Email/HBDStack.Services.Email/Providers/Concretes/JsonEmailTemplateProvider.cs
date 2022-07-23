using System.Text.Json;
using System.Text.Json.Serialization;
using HBDStack.Services.Email.Templates;

namespace HBDStack.Services.Email.Providers.Concretes;

public class JsonEmailTemplateProvider : EmailTemplateProvider
{
    private readonly JsonSerializerOptions _options;
    private readonly string _configFile;
        

    public JsonEmailTemplateProvider(string configFile, JsonSerializerOptions options = null)
    {
        _options = options??new JsonSerializerOptions
        {
            Converters = { new JsonStringEnumConverter() },
            PropertyNameCaseInsensitive = true,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        _configFile = Path.GetFullPath(configFile);
    }

    protected override async System.Threading.Tasks.Task<IEnumerable<EmailTemplate>> LoadTemplatesAsync()
    {
        if (!File.Exists(_configFile))
            throw new FileNotFoundException(_configFile);

        var fileText = await ReadToAsync(_configFile);

        if (string.IsNullOrWhiteSpace(fileText))
            throw new InvalidDataException(_configFile);

        return JsonSerializer.Deserialize<EmailTemplate[]>(fileText,_options);
    }
}