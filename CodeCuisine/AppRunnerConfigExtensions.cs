namespace CodeCuisine;

using CommandDotNet.IoC.SimpleInjector;

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

        foreach ((Type type, SubcommandAttribute? _) in appRunner.GetCommandClassTypes())
        {
            container.Register(type);
        }

        return appRunner.UseSimpleInjector(container);
    }
}