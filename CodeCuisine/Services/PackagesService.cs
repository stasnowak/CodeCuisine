using System.Xml.Linq;

namespace CodeCuisine.Services;

public class PackagesService : IPackagesService
{
    private readonly ISystemBroker systemBroker;
    private readonly IConsoleBroker consoleBroker;

    public PackagesService(
        ISystemBroker systemBroker,
        IConsoleBroker consoleBroker)
    {
        this.systemBroker = systemBroker;
        this.consoleBroker = consoleBroker;
    }

    public Task WriteAsync()
    {
        var projectRootDirectoryPath = this.systemBroker.ReturnProjectRootDirectoryPath();
        var projectRootDirectoryInfo = new DirectoryInfo(projectRootDirectoryPath);
        var fileSystemInfos = projectRootDirectoryInfo.EnumerateFileSystemInfos("*", SearchOption.AllDirectories)
            .Where(f => f.Extension.Equals(".csproj", StringComparison.OrdinalIgnoreCase) |
                        f.Name.Equals("Directory.Packages.props", StringComparison.OrdinalIgnoreCase));

        var packages = new Dictionary<string, Version>();

        foreach (var fileSystemInfo in fileSystemInfos)
        {
            var document = XDocument.Load(fileSystemInfo.FullName);

            var packageReferences = document.Descendants()
                .Where(x => x.Name.LocalName is "PackageReference" or "PackageVersion");

            foreach (var packageReference in packageReferences)
            {
                var packageName = packageReference.Attribute("Include")?.Value ??
                                  throw new InvalidOperationException();
                var value = packageReference.Attribute("Version")?.Value;
                if (value != null)
                {
                    var version = Version.Parse(value);

                    if (!packages.TryAdd(packageName, version))
                    {
                        packages[packageName] = version > packages[packageName] ? version : packages[packageName];
                    }
                }
                else
                {
                    continue;
                }

                if (packageReference.Name.LocalName == "PackageReference")
                {
                    packageReference.Attribute("Version")?.Remove();
                }
            }

            if (fileSystemInfo.Name == "Directory.Packages.props")
            {
                File.Delete(fileSystemInfo.FullName);
            }
            else
            {
                document.Save(fileSystemInfo.FullName);
            }
        }

        var packagesFilePath = Path.Combine(projectRootDirectoryPath, "Directory.Packages.props");
        var packagesDocument = new XDocument(
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

        packagesDocument.Save(packagesFilePath);

        this.consoleBroker.WriteLine($"Directory.Packages.props file generated at: {packagesFilePath}");

        return Task.CompletedTask;
    }
}