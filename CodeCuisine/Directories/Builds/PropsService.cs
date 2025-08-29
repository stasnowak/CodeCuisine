using System.Text;

using CodeCuisine.Brokers;

namespace CodeCuisine.Directories.Builds;

public class PropsService : IPropsService
{
    private readonly ISystemBroker systemBroker;

    public PropsService(ISystemBroker systemBroker)
    {
        this.systemBroker = systemBroker;
    }

    public async Task Generate()
    {
        var projectRootDirectoryPath = this.systemBroker.ReturnProjectRootDirectoryPath();
        var propsFilePath = Path.Combine(projectRootDirectoryPath, "Directory.Build.props");
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
        await using var fs = new FileStream(propsFilePath, FileMode.Create, FileAccess.Write, FileShare.None);
        await using var writer = new StreamWriter(fs, new UTF8Encoding(false));

        foreach (var chunk in stringBuilder.GetChunks())
        {
            await writer.WriteAsync(chunk);
        }

        await writer.FlushAsync();
    }
}