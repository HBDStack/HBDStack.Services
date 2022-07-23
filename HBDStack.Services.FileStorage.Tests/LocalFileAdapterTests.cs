using FluentAssertions;
using HBDStack.Services.FileStorage.Abstracts;
using HBDStack.Services.FileStorage.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace HBDStack.Services.FileStorage.Tests;

public class LocalFileAdapterTests
{
    private IFileAdapter _adapter;
    private LocalFolderOptions _options;

    [OneTimeSetUp]
    public void Setup()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        var service =
            new ServiceCollection()
                .AddLogging()
                .AddSingleton<IFileAdapter, LocalFolderFileAdapter>()
                .Configure<LocalFolderOptions>(o => config.GetSection(LocalFolderOptions.Name).Bind(o))
                .BuildServiceProvider();

        _adapter = service.GetRequiredService<IFileAdapter>();
        _options = service.GetRequiredService<IOptions<LocalFolderOptions>>().Value;

        //Delete All Files and folder in Root Folder
        _options.RootFolder!.DeleteAll();
    }

    [Test]
    [Order(0)]
    public async Task SaveNewFile()
    {
        var file = BinaryData.FromBytes(await File.ReadAllBytesAsync("TestData/log.txt"));
        await _adapter.SaveFileAsync("log.txt", file);

        File.Exists(Path.Combine(_options.RootFolder!, "log.txt"))
            .Should().BeTrue();
    }

    [Test]
    [Order(1)]
    public async Task SaveExistedFile()
    {
        var file = BinaryData.FromBytes(await File.ReadAllBytesAsync("TestData/log.txt"));
        var action = () => _adapter.SaveFileAsync("log.txt", file);

        await action.Should().ThrowAsync<InvalidOperationException>();
    }
    
    [Test]
    [Order(1)]
    public async Task SaveExistedWithOverWriteFile()
    {
        var file = BinaryData.FromBytes(await File.ReadAllBytesAsync("TestData/log.txt"));
        var action = () => _adapter.SaveFileAsync("log.txt", file,true);

        await action.Should().NotThrowAsync();
    }
    
    [Test]
    [Order(2)]
    public async Task GetFile()
    {
        var oldfile = BinaryData.FromBytes(await File.ReadAllBytesAsync("TestData/log.txt"));
        var file =await _adapter.GetFileAsync("log.txt");

        oldfile.ToString().Should().Be(file.ToString());
    }
    
    [Test]
    [Order(2)]
    public async Task GetNotExistedFile()
    {
        var file =await _adapter.GetFileAsync("hello.txt");
        file.Should().BeNull();
    }
    
    [Test]
    [Order(3)]
    public async Task DeleteFile()
    {
        var rs =await _adapter.DeleteFileAsync("log.txt");
        rs.Should().BeTrue();
        
        rs =await _adapter.FileExistedAsync("log.txt");
        rs.Should().BeFalse();
    }
    
    [Test]
    [Order(0)]
    public async Task SaveNewFileSubFolder()
    {
        var file = BinaryData.FromBytes(await File.ReadAllBytesAsync("TestData/log.txt"));
        await _adapter.SaveFileAsync("/sub/folder/log.txt", file);

        (await _adapter.FileExistedAsync("/sub/folder/log.txt"))
            .Should().BeTrue();
    }
}