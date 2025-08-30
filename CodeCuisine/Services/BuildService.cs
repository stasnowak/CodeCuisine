using System.Text;

using CodeCuisine.Brokers;

using CommandDotNet;

namespace CodeCuisine.Services;

public class BuildService : IBuildService
{
    private readonly ISystemBroker systemBroker;
    private readonly IConsoleBroker consoleBroker;

    public BuildService(ISystemBroker systemBroker,
        IConsoleBroker consoleBroker)
    {
        this.systemBroker = systemBroker;
        this.consoleBroker = consoleBroker;
    }

    public async Task WriteAsync()
    {
        var projectRootDirectoryPath = this.systemBroker.ReturnProjectRootDirectoryPath();
        var buildFilePath = Path.Combine(projectRootDirectoryPath, "Directory.Build.props");
        var stringBuilder = new StringBuilder()
            .Append(
                """
                <?xml version="1.0" encoding="utf-8" standalone="yes"?>
                <Project>
                  <PropertyGroup>
                    <TargetFramework>net9.0</TargetFramework>
                    <Nullable>enable</Nullable>
                    <ImplicitUsings>enable</ImplicitUsings>
                  </PropertyGroup>
                </Project>
                """);
        await using var fs = new FileStream(buildFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
        await using var writer = new StreamWriter(fs, new UTF8Encoding(false));

        foreach (var chunk in stringBuilder.GetChunks())
        {
            await writer.WriteAsync(chunk);
        }

        await writer.FlushAsync();

        this.consoleBroker.WriteLine($"Directory.Build.props file generated at: {buildFilePath}");
    }
}