using System.Xml.Linq;
using CliWrap;
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
            .Run(args);
    }

    [Command(Description = "Run everything i have to do to setup a new project.")]
    public async Task All(IConsole console)
    {
        Build();
        await Gitignore();
        Packages(console);
    }
    
    [Command(Description = "Add default .gitignore")]
    public async Task Gitignore([Option]bool force = false)
    {
        if (File.Exists(".gitignore") && !force)
        {
            return;
        }
        
        var result = await Cli.Wrap("dotnet")
            .WithArguments("new gitignore --force")
            .ExecuteAsync();

        if (!result.IsSuccess)
        {
            throw new Exception("Failed to create .gitignore");
        }
    }
    
    [Command(Description = "Add or modify version entries to Directory.Packages.props file from all .csproj files in the solution.")]
    public void Packages(IConsole console)
    {
        var solutionPath = FindSolutionPath();
        if (solutionPath == null)
        {
            console.WriteLine("Solution file not found.");
            return;
        }

        var projectFiles =
            Directory.GetFiles(Path.GetDirectoryName(solutionPath)!, "*.csproj", SearchOption.AllDirectories);

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

        GenerateDirectoryPackagesProps(Path.GetDirectoryName(solutionPath)!, packages);
    }

    [Command(Description = "Add or modify Directory.Build.props file.")]
    public void Build()
    {
        var solutionPath = FindSolutionPath();
        if (solutionPath == null)
        {
            Console.WriteLine("Solution file not found.");
            return;
        }

        var projectFiles =
            Directory.GetFiles(Path.GetDirectoryName(solutionPath)!, "*.csproj", SearchOption.AllDirectories);

        if (projectFiles.Length == 0)
        {
            Console.WriteLine("No .csproj files found in the solution.");
            return;
        }

        GenerateDirectoryBuildProps(Path.GetDirectoryName(solutionPath)!);
    }

    private string? FindSolutionPath()
    {
        var currentDirectory = Directory.GetCurrentDirectory();

        while (currentDirectory != null)
        {
            var solutionFiles = Directory.GetFiles(currentDirectory, "*.sln");
            if (solutionFiles.Length > 0)
            {
                return solutionFiles.FirstOrDefault();
            }

            currentDirectory = Directory.GetParent(currentDirectory)?.FullName;
        }

        return null;
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
            new XElement("Project",
                new XElement("PropertyGroup",
                    new XElement("ManagePackageVersionsCentrally", "true")
                ),
                new XElement("ItemGroup",
                    packages.Select(package =>
                        new XElement("PackageVersion",
                            new XAttribute("Include", package.Key),
                            new XAttribute("Version", package.Value)
                        )
                    )
                )
            )
        );

        document.Save(propsFilePath);

        Console.WriteLine($"Directory.Packages.props file generated at: {propsFilePath}");
    }

    private void GenerateDirectoryBuildProps(string solutionDirectory)
    {
        var propsFilePath = Path.Combine(solutionDirectory, "Directory.Build.props");

        var document = new XDocument(
            new XDeclaration("1.0", "utf-8", "yes"),
            new XElement("Project",
                new XElement("PropertyGroup",
                    new XElement("TargetFramework", "net9.0"),
                    new XElement("Nullable", "enable"),
                    new XElement("ImplicitUsings", "enable")
                )
            )
        );

        document.Save(propsFilePath);

        Console.WriteLine($"Directory.Build.props file generated at: {propsFilePath}");
    }
    
}