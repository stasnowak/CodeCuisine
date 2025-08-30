using CodeCuisine.Brokers;
using CodeCuisine.Services;

using CommandDotNet;
using CommandDotNet.IoC.SimpleInjector;
using CommandDotNet.Rendering;

namespace CodeCuisine;

public static class AppRunnerConfigExtensions
{
    public static AppRunner RegisterSimpleInjector(this AppRunner appRunner)
    {
        var container = new SimpleInjector.Container();

        container.Register<IConsoleBroker, ConsoleBroker>();
        container.Register<ISystemBroker, SystemBroker>();
        container.Register<IBuildService, BuildService>();
        container.Register<IGitIgnoreService, GitIgnoreService>();
        container.Register<IPackagesService, PackagesService>();

        foreach ((Type type, SubcommandAttribute? subcommandAttr) in appRunner.GetCommandClassTypes())
        {
            container.Register(type);
        }

        return appRunner.UseSimpleInjector(container);
    }
}