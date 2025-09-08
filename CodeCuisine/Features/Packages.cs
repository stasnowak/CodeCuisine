using System.Text;
using System.Xml.Linq;
using CodeCuisine.Options;

namespace CodeCuisine.Features;

[Subcommand]
[Command("packages", Description = "Add default Directory.Packages.props")]
public class Packages
{
    private readonly ISystemBroker systemBroker;
    private readonly IConsoleBroker consoleBroker;

    public Packages(
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
        var directoryPackagesPropsFilePath = Path.Combine(projectRootDirectoryPath, "Directory.Packages.props");

        if (systemBroker.FileExists(directoryPackagesPropsFilePath) && !option.Force)
        {
            consoleBroker.WriteLine(
                "Directory.Packages.props file already exists. Please select the force option to overwrite.");
            return;
        }

        if (!option.DryRun)
        {
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


            var xmlWriterSettings = new System.Xml.XmlWriterSettings
            {
                Encoding = new UTF8Encoding(true),
                Indent = true,
                OmitXmlDeclaration = false,
                Async = true
            };

            
            await using var fs = new FileStream(directoryPackagesPropsFilePath, FileMode.Create, FileAccess.Write,
                FileShare.None);
            await using var writer = System.Xml.XmlWriter.Create(fs, xmlWriterSettings);
            await packagesDocument.SaveAsync(writer, CancellationToken.None);
            await writer.FlushAsync();
        }

        this.consoleBroker.WriteLine($"Directory.Packages.props file generated at: {directoryPackagesPropsFilePath}");
    }
}