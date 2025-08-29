using CommandDotNet;

namespace CodeCuisine.Brokers;

public interface ISystemBroker
{
    string ReturnProjectRootDirectoryPath();
}