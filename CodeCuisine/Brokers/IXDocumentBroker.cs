using System.Xml.Linq;

namespace CodeCuisine.Brokers;

public interface IXDocumentBroker
{
    public string Get();
}

public class XDocumentBroker : IXDocumentBroker
{
    public string Get() => new XDocument(
        new XDeclaration("1.0", "utf-8", "yes"),
        new XElement("Project",
            new XElement("PropertyGroup",
                new XElement("TargetFramework", "net9.0"),
                new XElement("Nullable", "enable"),
                new XElement("ImplicitUsings", "enable")))).ToString();
}