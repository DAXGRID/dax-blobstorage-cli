using System.CommandLine;

namespace DAX.BlobstorageCli;

internal sealed class Program
{
    static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("Sample app for System.CommandLine");

        var fileCommandOption = new Option<bool>(
            name: "--file",
            description: "The file to upload.")
        {
            IsRequired = true,
        };

        fileCommandOption.AddAlias("-f");

        var uploadCommand = new Command("upload", "Upload a file to blobstorage");
        uploadCommand.Add(fileCommandOption);

        var outputCommandOption = new Option<string>(
            name: "--output",
            description: "The output path where the file should be downloaded to.")
        {
            IsRequired = true
        };

        outputCommandOption.AddAlias("-o");

        var downloadCommand = new Command("download", "Download a file from blobstorage");

        downloadCommand.Add(outputCommandOption);

        rootCommand.Add(downloadCommand);
        rootCommand.Add(uploadCommand);

        uploadCommand.SetHandler((outputPath) =>
        {
            Console.WriteLine($"Output path: {outputPath}");
        },
        outputCommandOption);

        return await rootCommand.InvokeAsync(args).ConfigureAwait(false);
    }
}
