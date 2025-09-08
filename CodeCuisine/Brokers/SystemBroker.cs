namespace CodeCuisine.Brokers;

public class SystemBroker : ISystemBroker
{
    public string GetProjectRootDirectoryPath()
    {
        var currentDirectory = new DirectoryInfo(Directory.GetCurrentDirectory());

        while (true)
        {
            if (currentDirectory.EnumerateFileSystemInfos("*", SearchOption.TopDirectoryOnly)
                .Any(f => f.Extension.Equals(".sln", StringComparison.OrdinalIgnoreCase) |
                          f.Name.Equals(".git", StringComparison.OrdinalIgnoreCase) |
                          f.Name.Equals("global.json", StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine($"Found solution at: {currentDirectory.FullName}");
                return currentDirectory.FullName;
            }

            currentDirectory = currentDirectory.Parent;

            ArgumentNullException.ThrowIfNull(currentDirectory);
        }
    }
    
    public bool FileExists(string path)
    {
        return File.Exists(path);
    }
}