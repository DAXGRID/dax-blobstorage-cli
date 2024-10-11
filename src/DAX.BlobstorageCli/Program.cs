using System.CommandLine;

namespace DAX.BlobstorageCli;

internal sealed class Program
{
   static async Task<int> Main(string[] args)
    {
        var rootCommand = new RootCommand("Sample app for System.CommandLine");

        var uploadCommandOptions = new Option<bool>(
            name: "--file",
            description: "The file to upload.");

        var uploadCommand = new Command("upload", "Upload a file to blobstorage");
        uploadCommand.Add(uploadCommandOptions);

        var downloadCommandOptions = new Option<string>(
            name: "--output",
            description: "The output path where the file should be downloaded to.");

        var downloadCommand = new Command("download", "Download a file from blobstorage");
        downloadCommand.Add(downloadCommandOptions);

        rootCommand.Add(downloadCommand);
        rootCommand.Add(uploadCommand);

        uploadCommand.SetHandler((outputPath) =>
        {
            Console.WriteLine($"Output path: {outputPath}");
        },
        downloadCommandOptions);

        return await rootCommand.InvokeAsync(args).ConfigureAwait(false);
    }
}
