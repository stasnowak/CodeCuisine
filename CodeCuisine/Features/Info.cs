namespace CodeCuisine.Features;

[Subcommand]
[Command("info", Description = "Show project setup status & suggestions")]
public class Info
{
    private readonly ISystemBroker systemBroker;
    private readonly IConsoleBroker consoleBroker;

    public Info(
        ISystemBroker systemBroker,
        IConsoleBroker consoleBroker)
    {
        this.systemBroker = systemBroker;
        this.consoleBroker = consoleBroker;
    }

    [DefaultCommand]
    public Task Default()
    {
        var projectRoot = systemBroker.GetProjectRootDirectoryPath();

        var checks = new (string Feature, string Path, string Command)[]
        {
            ("Build", Path.Combine(projectRoot, "Directory.Build.props"), "build"),
            ("Editorconfig", Path.Combine(projectRoot, ".editorconfig"), "editorconfig"),
            ("Gitignore", Path.Combine(projectRoot, ".gitignore"), "gitignore"),
            ("Global", Path.Combine(projectRoot, "global.json"), "global"),
            ("Packages", Path.Combine(projectRoot, "Directory.Packages.props"), "packages")
        };

        consoleBroker.WriteLine($"Project root: {projectRoot}");
        consoleBroker.WriteLine("Checking recommended files:");

        var missing = new List<(string Feature, string Path, string Command)>();

        foreach (var (feature, path, command) in checks)
        {
            var exists = systemBroker.FileExists(path);
            consoleBroker.WriteLine($"- {feature,-12} {(exists ? "OK" : "Missing"),-8} ({path})");
            if (!exists)
            {
                missing.Add((feature, path, command));
            }
        }

        consoleBroker.WriteLine("");
        if (missing.Count == 0)
        {
            consoleBroker.WriteLine("All recommended files are present. Great job!");
        }
        else
        {
            consoleBroker.WriteLine("Suggestions to improve your project:");
            foreach (var m in missing)
            {
                consoleBroker.WriteLine($"• Use the '{m.Command}' command to add: {Path.GetFileName(m.Path)}");
            }
        }

        return Task.CompletedTask;
    }
}