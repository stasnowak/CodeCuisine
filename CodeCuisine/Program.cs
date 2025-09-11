using CodeCuisine.Features;
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

    [Subcommand] public Build Build { get; set; }

    [Subcommand] public Editorconfig Editorconfig { get; set; }

    [Subcommand] public Gitignore Gitignore { get; set; }

    [Subcommand] public Global Global { get; set; }

    [Subcommand] public Packages Packages { get; set; }
}