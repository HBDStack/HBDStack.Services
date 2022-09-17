using FluentAssertions;
using HBDStack.Services.FileStorage.AzureAdapters;
using HBDStack.Services.FileStorage.Abstracts;
using HBDStack.Services.FileStorage.Adapters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace HBDStack.Services.FileStorage.Tests;

public class AzureStorageAdapterTest
{
    private IFileAdapter _adapter;

    [OneTimeSetUp]
    public void Setup()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        var service =
            new ServiceCollection()
                .AddLogging()
                .AddSingleton<IFileAdapter, AzureStorageAdapter>()
                .AddAzureStorageAdapter(config)
                .BuildServiceProvider();

        _adapter = service.GetRequiredService<IFileAdapter>();
    }

    [Test]
    [Order(0)]
    public async Task SaveNewFile()
    {
        var file = BinaryData.FromBytes(await File.ReadAllBytesAsync("TestData/log.txt"));
        await _adapter.SaveFileAsync("log.txt", file);

        (await _adapter.FileExistedAsync("log.txt")).Should().BeTrue();
    }

    [Test]
    [Order(1)]
    public async Task SaveExistedFile()
    {
        var file =BinaryData.FromBytes(await File.ReadAllBytesAsync("TestData/log.txt"));
        var action = () => _adapter.SaveFileAsync("log.txt", file);

        await action.Should().ThrowAsync<Exception>();
    }

    [Test]
    [Order(1)]
    public async Task SaveExistedWithOverWriteFile()
    {
        var file = BinaryData.FromBytes(await File.ReadAllBytesAsync("TestData/log.txt"));
        var action = () => _adapter.SaveFileAsync("log.txt", file, true);

        await action.Should().NotThrowAsync();
    }

    [Test]
    [Order(2)]
    public async Task GetFile()
    {
        var oldfile = BinaryData.FromBytes(await File.ReadAllBytesAsync("TestData/log.txt"));
        var file = await _adapter.GetFileAsync("log.txt");

        oldfile.ToString().Should().Be(file.ToString());
    }

    [Test]
    [Order(2)]
    public async Task GetNotExistedFile()
    {
        var file = await _adapter.GetFileAsync("/sub/hello_log.txt");
        file.Should().BeNull();
    }

    [Test]
    [Order(3)]
    public async Task DeleteFile()
    {
        var rs = await _adapter.DeleteFileAsync("log.txt");
        rs.Should().BeTrue();

        rs = await _adapter.FileExistedAsync("log.txt");
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

    [Test]
    [Order(1)]
    public async Task DeleteFileSubFolder()
    {
        await _adapter.DeleteFileAsync("/sub/folder/log.txt");

        (await _adapter.FileExistedAsync("/sub/folder/log.txt"))
            .Should().BeFalse();
    }
    
    [Test]
    [Order(4)]
    public async Task DeleteFolder()
    {
        await _adapter.SaveFileAsync("/folderToDelete/log1.txt",
            BinaryData.FromBytes(await File.ReadAllBytesAsync($"TestData/log.txt")), true);
        
        await _adapter.SaveFileAsync("/folderToDelete/log2.txt",
            BinaryData.FromBytes(await File.ReadAllBytesAsync($"TestData/log.txt")), true);
        
        await _adapter.SaveFileAsync("/folderToDelete/subfolderToDelete/log3.txt",
            BinaryData.FromBytes(await File.ReadAllBytesAsync($"TestData/log.txt")), true);

        var result = await _adapter.DeleteFolderAsync("/folderToDelete/");
        Assert.True(result);
    }
}