using System.Xml.Linq;

namespace CodeCuisine.Tests.Snapshot;

public class UnitTest1
{
    [Fact]
    public async Task Test1()
    {
        var document = new XDocument(
            new XDeclaration("1.0", "utf-8", "yes"),
            new XElement("Project",
                new XElement("PropertyGroup",
                    new XElement("TargetFramework", "net9.0"),
                    new XElement("Nullable", "enable"),
                    new XElement("ImplicitUsings", "enable")
                )
            )
        );
        
        await Verify(document);
    }
    
    
}