namespace CodeCuisine.Features;

[Subcommand]
[Command("build", Description = "Add default Directory.Build.props file")]
public class Build
{
    private readonly ISystemBroker systemBroker;
    private readonly IConsoleBroker consoleBroker;

    public Build(
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
        var directoryBuildPropsFilePath = Path.Combine(projectRootDirectoryPath, "Directory.Build.props");

        if (systemBroker.FileExists(directoryBuildPropsFilePath) && !option.Force)
        {
            consoleBroker.WriteLine(
                "Directory.Build.props file already exists. Please select the force option to overwrite.");
            return;
        }

        if (!option.DryRun)
        {
            await using var fs = new FileStream(directoryBuildPropsFilePath, FileMode.Create, FileAccess.Write,
                FileShare.None);
            await using var writer = new StreamWriter(fs, new UTF8Encoding(false));
            await writer.WriteAsync(directoryBuildPropsFileContent);
            await writer.FlushAsync();
        }

        this.consoleBroker.WriteLine($"Directory.Build.props file generated at: {directoryBuildPropsFilePath}");
    }

    private readonly string directoryBuildPropsFileContent = """
                                                             <?xml version="1.0" encoding="utf-8" standalone="yes"?>
                                                             <Project>
                                                               <PropertyGroup>
                                                                 <TargetFramework>net9.0</TargetFramework>
                                                                 <Nullable>enable</Nullable>
                                                                 <ImplicitUsings>enable</ImplicitUsings>
                                                               </PropertyGroup>
                                                             </Project>
                                                             """;
}