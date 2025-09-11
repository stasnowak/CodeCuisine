using System.Text;
using CodeCuisine.Options;

namespace CodeCuisine.Features;

[Subcommand]
[Command("global", Description = "Add default global.json file")]
public class Global
{
    private readonly ISystemBroker systemBroker;
    private readonly IConsoleBroker consoleBroker;

    public Global(
        ISystemBroker systemBroker,
        IConsoleBroker consoleBroker)
    {
        this.systemBroker = systemBroker;
        this.consoleBroker = consoleBroker;
    }

    [DefaultCommand]
    public async Task Default(DefaultOption option)
    {
        var projectRootDirectoryPath = systemBroker.GetProjectRootDirectoryPath();
        var globalJsonFilePath = Path.Combine(projectRootDirectoryPath, "global.json");

        if (systemBroker.FileExists(globalJsonFilePath) && !option.Force)
        {
            consoleBroker.WriteLine(
                "global.json file already exists. Please select the force option to overwrite.");
            return;
        }

        if (!option.DryRun)
        {
            await using var fs = new FileStream(globalJsonFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
            await using var writer = new StreamWriter(fs, new UTF8Encoding(false));
            await writer.WriteAsync(globalJsonFileContent);
            await writer.FlushAsync();
        }

        this.consoleBroker.WriteLine($"global.json file generated at: {globalJsonFilePath}");
    }

    private readonly string globalJsonFileContent = """
                                                    {
                                                      "sdk": {
                                                        "version": "9.0.300",
                                                        "rollForward": "latestPatch",
                                                        "allowPrerelease": false
                                                      }
                                                    }
                                                    """;
}