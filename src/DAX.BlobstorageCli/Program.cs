using System.CommandLine;
using Azure.Storage.Blobs;

namespace DAX.BlobstorageCli;

internal sealed class Program
{
    static async Task<int> Main(string[] args)
    {
        var connectionString = Environment.GetEnvironmentVariable("DAX_BLOB_STORAGE_CLI_CONNECTION_STRING");

        var rootCommand = new RootCommand("DAX Blobstorage CLI.");

        var blobStorageContainerNameOption = new Option<string>(
            name: "--container-name",
            description: "The blob storagae container name.")
        {
            IsRequired = true,
        };

        blobStorageContainerNameOption.AddAlias("-cn");

        var uploadFileOption = new Option<string>(
            name: "--upload-file-path",
            description: "The file path to the file you want to upload to blobstorage.")
        {
            IsRequired = true,
        };

        uploadFileOption.AddAlias("-f");

        var downloadOutputFilePathOption = new Option<string>(
            name: "--output-file-path",
            description: "The output path where the file should be downloaded to.")
        {
            IsRequired = true
        };

        downloadOutputFilePathOption.AddAlias("-o");

        var downloadFileNameOption = new Option<string>(
            name: "--file",
            description: "The name of the file that should be downloaded.")
        {
            IsRequired = true
        };

        downloadFileNameOption.AddAlias("-f");

        // Upload command
        var uploadCommand = new Command("upload", "Upload a file to blobstorage");
        uploadCommand.Add(uploadFileOption);
        uploadCommand.Add(blobStorageContainerNameOption);

        uploadCommand.SetHandler(async (uploadFilePath, blobStorageContainerName) =>
        {
            var client = new BlobContainerClient(connectionString, blobStorageContainerName);
            Console.WriteLine($"Starting uploading the file '{uploadFilePath}' to the container '{blobStorageContainerName}'");
            await UploadFromFileAsync(client, uploadFilePath).ConfigureAwait(false);
            Console.WriteLine($"Finished uploading the file '{uploadFilePath}' to the container '{blobStorageContainerName}'");
        },
        uploadFileOption,
        blobStorageContainerNameOption);

        // Download command
        var downloadCommand = new Command("download", "Download a file from blobstorage");
        downloadCommand.Add(blobStorageContainerNameOption);
        downloadCommand.Add(downloadFileNameOption);
        downloadCommand.Add(downloadOutputFilePathOption);

        downloadCommand.SetHandler(async (blobStorageContainerName, downloadFileName, downloadOutputPath) =>
        {
            Console.WriteLine($"Starting download the file '{downloadFileName}' from the container '{blobStorageContainerName}' to local path '{downloadOutputPath}'.");
            var client = new BlobContainerClient(connectionString, blobStorageContainerName);
            await DownloadFileAsync(client, downloadFileName, downloadOutputPath).ConfigureAwait(false);
            Console.WriteLine($"Finished downloading the file '{downloadFileName}' from the container '{blobStorageContainerName}' to local path '{downloadOutputPath}'.");
        },
        blobStorageContainerNameOption,
        downloadFileNameOption,
        downloadOutputFilePathOption);

        rootCommand.Add(downloadCommand);
        rootCommand.Add(uploadCommand);

        return await rootCommand.InvokeAsync(args).ConfigureAwait(false);
    }

    public static async Task UploadFromFileAsync(
        BlobContainerClient containerClient,
        string localFilePath)
    {
        string fileName = Path.GetFileName(localFilePath);
        var blobClient = containerClient.GetBlobClient(fileName);
        await blobClient.UploadAsync(localFilePath, true).ConfigureAwait(false);
    }

    public static async Task DownloadFileAsync(
        BlobContainerClient containerClient,
        string fileName,
        string downloadOutputFilePath)
    {

        var blobClient = containerClient.GetBlobClient(fileName);
        await blobClient
            .DownloadToAsync(Path.Combine(downloadOutputFilePath, fileName))
            .ConfigureAwait(false);
    }
}
