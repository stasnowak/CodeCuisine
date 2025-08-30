using System.Xml.Linq;

using CodeCuisine.Brokers;
using CodeCuisine.Services;

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
    public async Task All(
        IBuildService buildService,
        IGitIgnoreService gitignoreService,
        IPackagesService packagesService)
    {
        await buildService.WriteAsync();
        await gitignoreService.WriteAsync();
        await packagesService.WriteAsync();
    }

    [Command(Description = "Add default Directory.Build.props")]
    public async Task Props(IBuildService buildService)
    {
        await buildService.WriteAsync();
    }

    [Command(Description = "Add default .gitignore")]
    public async Task Gitignore(IGitIgnoreService gitignoreService)
    {
        await gitignoreService.WriteAsync();
    }

    [Command(Description = "Add default Directory.Packages.props")]
    public async Task Packages(IPackagesService packagesService)
    {
        await packagesService.WriteAsync();
    }
}