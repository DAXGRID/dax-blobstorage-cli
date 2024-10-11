using System.CommandLine;
using Azure.Storage.Blobs;

namespace DAX.BlobstorageCli;

internal sealed class Program
{
    static async Task<int> Main(string[] args)
    {
        const string connectionString = "";

        var rootCommand = new RootCommand("DAX Blobstorage CLI.");

        var uploadFileOption = new Option<string>(
            name: "--upload-file-path",
            description: "The file path to the file you want to upload to blobstorage.")
        {
            IsRequired = true,
        };

        uploadFileOption.AddAlias("-f");

        var uploadCommand = new Command("upload", "Upload a file to blobstorage");
        uploadCommand.Add(uploadFileOption);

        var downloadOutputPathCommandOption = new Option<string>(
            name: "--output-file-path",
            description: "The output path where the file should be downloaded to.")
        {
            IsRequired = true
        };

        downloadOutputPathCommandOption.AddAlias("-o");

        var downloadCommand = new Command("download", "Download a file from blobstorage");

        downloadCommand.Add(downloadOutputPathCommandOption);

        uploadCommand.SetHandler(async (uploadFilePath) =>
        {
            var client = new BlobContainerClient(connectionString, "input");
            await UploadFromFileAsync(client, uploadFilePath).ConfigureAwait(false);

            Console.WriteLine($"Upload file path: {uploadFilePath}");
        },
        uploadFileOption);

        downloadCommand.SetHandler((downloadPath) =>
        {
            var client = new BlobContainerClient(connectionString, "output");
            Console.WriteLine($"Download file path: {downloadPath}");
        },
        downloadOutputPathCommandOption);

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
}
