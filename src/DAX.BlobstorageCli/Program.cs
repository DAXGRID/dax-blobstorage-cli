using System.CommandLine;

namespace DAX.BlobstorageCli;

internal sealed class Program
{
   static async Task<int> Main(string[] args)
    {
        var fileOption = new Option<bool>(
            name: "--file",
            description: "The file to read and display on the console.");

        var rootCommand = new RootCommand("Sample app for System.CommandLine");

        var uploadCommand = new Command("upload", "Upload a file to blobstorage");
        rootCommand.Add(uploadCommand);

        var downloadCommand = new Command("download", "Download a file from blobstorage");
        rootCommand.Add(downloadCommand);

        return await rootCommand.InvokeAsync(args).ConfigureAwait(false);
    }
}
