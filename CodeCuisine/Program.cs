using System.Xml.Linq;

using CodeCuisine.Brokers;
using CodeCuisine.Directories.Builds;
using CodeCuisine.Gits;

using CommandDotNet;
using CommandDotNet.NameCasing;

namespace CodeCuisine;

public class Program
{
    public static int Main(string[] args)
    {
        return new AppRunner<Program>()
            .UseNameCasing(Case.KebabCase)
            .UseDefaultMiddleware()
            .RegisterSimpleInjector()
            .Run(args);
    }

    [DefaultCommand]
    [Command(Description = "Run everything i have to do to setup a new project.")]
    public async Task All(IGitIgnoreService gitignoreService)
    {
        await gitignoreService.WriteGitIgnoreAsync();
    }

    [Command(Description = "Add default .gitignore")]
    public async Task Gitignore(IGitIgnoreService gitignoreService)
    {
        await gitignoreService.WriteGitIgnoreAsync();
    }

    [Command(Description =
        "Add or modify version entries to Directory.Packages.props file from all .csproj files in the solution.")]
    public void Packages(IConsole console)
    {
        var solutionPath = new SystemBroker().ReturnProjectRootDirectoryPath();

        var projectFiles =
            Directory.GetFiles(Path.GetDirectoryName(solutionPath) !, "*.csproj", SearchOption.AllDirectories);

        if (projectFiles.Length == 0)
        {
            console.WriteLine("No .csproj files found in the solution.");
            return;
        }

        var packages = new Dictionary<string, string>();

        foreach (var projectFile in projectFiles)
        {
            try
            {
                var document = XDocument.Load(projectFile);

                var packageReferences = document.Descendants("PackageReference");

                foreach (var packageReference in packageReferences)
                {
                    var packageName = packageReference.Attribute("Include")?.Value;
                    var version = packageReference.Attribute("Version")?.Value;

                    if (packageName == null || version == null) continue;
                    packages.TryAdd(packageName, version);

                    // Remove Version attribute from .csproj
                    packageReference.Attribute("Version")?.Remove();
                }

                document.Save(projectFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing {projectFile}: {ex.Message}");
            }
        }

        GenerateDirectoryPackagesProps(Path.GetDirectoryName(solutionPath) !, packages);
    }

    [Command(Description = "Add or modify Directory.Build.props file.")]
    public void Build()
    {
        var solutionPath = new SystemBroker().ReturnProjectRootDirectoryPath();

        var projectFiles =
            Directory.GetFiles(Path.GetDirectoryName(solutionPath) !, "*.csproj", SearchOption.AllDirectories);

        if (projectFiles.Length == 0)
        {
            Console.WriteLine("No .csproj files found in the solution.");
            return;
        }

        //GenerateDirectoryBuildProps(Path.GetDirectoryName(solutionPath) !);
    }

    private void GenerateDirectoryPackagesProps(string solutionDirectory, Dictionary<string, string> packages)
    {
        var propsFilePath = Path.Combine(solutionDirectory, "Directory.Packages.props");

        if (File.Exists(propsFilePath))
        {
            var existingDocument = XDocument.Load(propsFilePath);
            var packageReferences = existingDocument.Descendants("PackageVersion");

            foreach (var packageReference in packageReferences)
            {
                var packageName = packageReference.Attribute("Include")?.Value;
                var version = packageReference.Attribute("Version")?.Value;

                if (packageName == null || version == null) continue;
                packages.TryAdd(packageName, version);

                packageReference.Attribute("Version")?.Remove();
            }
        }

        var document = new XDocument(
            new XDeclaration("1.0", "utf-8", "yes"),
            new XElement(
                "Project",
                new XElement(
                    "PropertyGroup",
                    new XElement("ManagePackageVersionsCentrally", "true")),
                new XElement(
                    "ItemGroup",
                    packages.Select(package =>
                        new XElement(
                            "PackageVersion",
                            new XAttribute("Include", package.Key),
                            new XAttribute("Version", package.Value))))));

        document.Save(propsFilePath);

        Console.WriteLine($"Directory.Packages.props file generated at: {propsFilePath}");
    }

    private async Task GenerateDirectoryBuildProps(IPropsService propsService)
    {
        await propsService.Generate();
    }
}