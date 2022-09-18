using FluentAssertions;
using HBDStack.Services.FileStorage.Abstracts;
using HBDStack.Services.FileStorage.AwsS3Adapters;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace HBDStack.Services.FileStorage.Tests;

public class S3AdapterTest
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
                .AddSingleton<IFileAdapter, S3Adapter>()
                .AddS3Adapter(config)
                .BuildServiceProvider();

        _adapter = service.GetRequiredService<IFileAdapter>();
    }

    [Test]
    [Order(0)]
    public async Task SaveNewFile()
    {
        var file = BinaryData.FromBytes(await File.ReadAllBytesAsync("TestData/log.txt"));
        await _adapter.SaveFileAsync("log.txt", file, true);

        (await _adapter.FileExistedAsync("log.txt")).Should().BeTrue();
    }

    [Test]
    [Order(1)]
    public async Task SaveExistedFile()
    {
        var file = BinaryData.FromBytes(await File.ReadAllBytesAsync("TestData/log.txt"));
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
    public async Task ListFile()
    {
        (await _adapter.ListObjectInfoAsync("/").ToListAsync()).Should().HaveCountGreaterOrEqualTo(1);
    }
    
    [Test]
    [Order(2)]
    public async Task GetNotExistedFile()
    {
        var file = await _adapter.GetFileAsync("/sub/hello_log.txt");
        file.Should().BeNull();
    }


    [Test]
    [Order(0)]
    public async Task SaveNewFileSubFolder()
    {
        var file = BinaryData.FromBytes(await File.ReadAllBytesAsync("TestData/log.txt"));
        await _adapter.SaveFileAsync("/sub/folder/log.txt", file, true);

        (await _adapter.FileExistedAsync("/sub/folder/log.txt"))
            .Should().BeTrue();
    }

    [Test]
    [Order(3)]
    public async Task DeleteFile()
    {
        const string fileName = "delete_log.txt";
        await _adapter.SaveFileAsync(fileName,
            BinaryData.FromBytes(await File.ReadAllBytesAsync($"TestData/{fileName}")), true);

        var rs = await _adapter.DeleteFileAsync(fileName);
        rs.Should().BeTrue();

        rs = await _adapter.FileExistedAsync(fileName);
        rs.Should().BeFalse();
    }

    [Test]
    [Order(4)]
    public async Task DeleteFileSubFolder()
    {
        const string fileName = "delete_sub_folder_log.txt";
        const string filePath = $"/sub/folder/{fileName}";

        await _adapter.SaveFileAsync(filePath,
            BinaryData.FromBytes(await File.ReadAllBytesAsync($"TestData/{fileName}")), true);

        await _adapter.DeleteFileAsync(filePath);

        (await _adapter.FileExistedAsync(filePath))
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