namespace CodeCuisine.Options;

public class DefaultOption : IArgumentModel
{
    [Option( 'f', Description = "Force overwrite existing files.")] 
    public bool Force { get; set; }

    [Option('d', Description = "Dry run without writing files.")] 
    public bool DryRun { get; set; }
}