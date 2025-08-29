namespace CodeCuisine.Brokers;

public class SystemBroker : ISystemBroker
{
    public string ReturnProjectRootDirectoryPath()
    {
        var currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());

        while (true)
        {
            if (currentDirectory.EnumerateFileSystemInfos("*", SearchOption.TopDirectoryOnly)
                .Any(f => f.Extension.Equals(".sln", StringComparison.OrdinalIgnoreCase) |
                          f.Name.Equals(".git", StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine($"Found solution at: {currentDirectory.FullName}");
                return currentDirectory.FullName;
            }

            currentDirectory = currentDirectory.Parent;

            if (currentDirectory == null)
            {
                throw new Exception("Project root not found.");
            }
        }
    }
}