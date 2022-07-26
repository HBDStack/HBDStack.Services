using FluentAssertions;
using HBDStack.Services.FileStorage.Abstracts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;

namespace HBDStack.Services.FileStorage.Tests;

public class FileServiceTests
{
    private IFileService _service;

    [OneTimeSetUp]
    public void Setup()
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .Build();

        var service =
            new ServiceCollection()
                .AddLogging()
                .AddFileService(new FileServiceOptions
                {
                    MaxFileNameLength = 50,
                    MaxFileSizeInMb = 1,
                    IncludedExtensions = new[] { ".txt" }
                })
                .AddAzureStorageAdapter(config)
                .AddLocalFolderAdapter(config)
                .BuildServiceProvider();

        _service = service.GetRequiredService<IFileService>();
    }

    [Test]
    [Order(0)]
    public async Task SaveNewFile()
    {
        var file = BinaryData.FromBytes(await File.ReadAllBytesAsync("TestData/log.txt"));
        var info = new FileData(nameof(FileService), "log.txt", file);

        var rs = await _service.SaveFileAsync(info);

        rs.Should().Be($"{nameof(FileService)}/log.txt");
        (await _service.GetFileAsync(rs)).ToString().Should().Be(file.ToString());
    }

    [Test]
    [Order(0)]
    public async Task SaveNewFile_WithoutOwnerId()
    {
        var file = BinaryData.FromBytes(await File.ReadAllBytesAsync("TestData/log.txt"));
        var rs = () => new FileData(null, "log.txt", file);

        rs.Should().Throw<ArgumentNullException>();
    }

    [Test]
    [Order(0)]
    public async Task SaveNewFile_WithoutFileName()
    {
        var file = BinaryData.FromBytes(await File.ReadAllBytesAsync("TestData/log.txt"));
        var rs = () => new FileData(nameof(FileService), null, file);

        rs.Should().Throw<ArgumentNullException>();
    }

    [Test]
    [Order(0)]
    public async Task SaveNewFile_WithoutData()
    {
        var file = BinaryData.FromBytes(await File.ReadAllBytesAsync("TestData/log.txt"));
        var rs = () => new FileData(nameof(FileService), "log.txt", null);

        rs.Should().Throw<ArgumentNullException>();
    }

    [Test]
    [Order(1)]
    public async Task SaveExistedFile()
    {
        var file = BinaryData.FromBytes(await File.ReadAllBytesAsync("TestData/log.txt"));
        var info = new FileData(nameof(FileService), "log.txt", file);

        var action = () => _service.SaveFileAsync(info);

        await action.Should().ThrowAsync<Exception>();
    }


    [Test]
    [Order(1)]
    public async Task SaveExistedWithOverWriteFile()
    {
        var file = BinaryData.FromBytes(await File.ReadAllBytesAsync("TestData/log.txt"));
        var info = new FileData(nameof(FileService), "log.txt", file) { OverwriteIfExisted = true };

        var action = () => _service.SaveFileAsync(info);

        await action.Should().NotThrowAsync();
    }

    [Test]
    [Order(2)]
    public async Task GetFile()
    {
        var info = new FileArgs(nameof(FileService), "log.txt");
        (await _service.GetFileAsync(info)).Should().NotBeNull();
    }

    [Test]
    [Order(2)]
    public async Task CheckFileExisted()
    {
        (await _service.FileExistedAsync($"{nameof(FileService)}/log.txt")).Should().BeTrue();
    }

    [Test]
    [Order(2)]
    public async Task ListFile()
    {
        (await _service.ListObjectInfoAsync("/").ToListAsync()).Should().HaveCountGreaterOrEqualTo(1);

        var info = new FileArgs(nameof(FileService), "log.txt");
        (await _service.GetObjectInfoAsync(info)).Should().NotBeNull();
    }

    [Test]
    [Order(2)]
    public async Task GetNotExistedFile()
    {
        var info = new FileArgs(nameof(FileService), "hello.txt");
        (await _service.GetFileAsync(info)).Should().BeNull();
    }

    [Test]
    [Order(3)]
    public async Task DeleteFile()
    {
        var info = new FileArgs(nameof(FileService), "log.txt");

        (await _service.DeleteFileAsync(info)).Should().BeTrue();
        (await _service.GetFileAsync(info)).Should().BeNull();
    }

    [Test]
    [Order(3)]
    public async Task DeleteFolder()
    {
        var fileContent = BinaryData.FromBytes(await File.ReadAllBytesAsync($"TestData/log.txt"));

        var file = new FileData(nameof(FileService), "/Delete/log1.txt", fileContent)
        {
            OverwriteIfExisted = true
        };
        var path = await _service.SaveFileAsync(file);
        path.Should().NotBeNull();

        file = new FileData(nameof(FileService), "/Delete/log2.txt", fileContent)
        {
            OverwriteIfExisted = true
        };
        path = await _service.SaveFileAsync(file);
        path.Should().NotBeNull();

        file = new FileData(nameof(FileService), "/Delete/a/log3.txt", fileContent)
        {
            OverwriteIfExisted = true
        };
        path = await _service.SaveFileAsync(file);
        path.Should().NotBeNull();

        var result = await _service.DeleteFolderAsync($"/{nameof(FileService)}/Delete");
        Assert.True(result);
    }

    [Test]
    [Order(4)]
    public async Task FileName_Invalid()
    {
        var file = BinaryData.FromBytes(await File.ReadAllBytesAsync("TestData/log.txt"));
        var info = new FileData(nameof(FileService), "log-1234567890-1234567890-1234567890-1234567890.txt", file)
            { OverwriteIfExisted = true };

        var action = () => _service.SaveFileAsync(info);

        await action.Should().ThrowAsync<FileLoadException>();
    }

    [Test]
    [Order(4)]
    public async Task FileExtension_Invalid()
    {
        var file = BinaryData.FromBytes(await File.ReadAllBytesAsync("TestData/log.txt"));
        var info = new FileData(nameof(FileService), "log.csv", file) { OverwriteIfExisted = true };

        var action = () => _service.SaveFileAsync(info);

        await action.Should().ThrowAsync<FileLoadException>();
    }

    [Test]
    [Order(4)]
    public async Task FileData_Invalid()
    {
        var file = BinaryData.FromBytes(await File.ReadAllBytesAsync("TestData/big_log.txt"));
        var info = new FileData(nameof(FileService), "log-123.txt", file) { OverwriteIfExisted = true };

        var action = () => _service.SaveFileAsync(info);

        await action.Should().ThrowAsync<FileLoadException>();
    }
}