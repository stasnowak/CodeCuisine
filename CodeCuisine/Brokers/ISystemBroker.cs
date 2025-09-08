namespace CodeCuisine.Brokers;

public interface ISystemBroker
{
    string GetProjectRootDirectoryPath();
    bool FileExists(string path);
}