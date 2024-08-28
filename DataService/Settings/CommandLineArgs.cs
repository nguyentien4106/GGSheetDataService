namespace DataService.Settings
{
    public class CommandLineArgs(string[] args)
    {
        public string[] Args { get; set; } = args;
    }
}
