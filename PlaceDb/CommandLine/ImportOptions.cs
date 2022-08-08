using CommandLine;

namespace PlaceDb.CommandLine
{
    [Verb("import", HelpText = "Imports data to the database.")]
    public class ImportOptions
    {
        [Option('p', "path", Required = true, HelpText = "The path of archives to import")]
        public string Path { get; set; }
    }
}