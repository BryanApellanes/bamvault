using Bam.Net.CommandLine;

namespace Bam.Net
{
    public partial class Program
    {
        public static void AddArguments()
        {
            AddValidArgument("value", false, false, "The value to add to the vault.");
            AddValidArgument("key", false, false, "The key to set the value for.");
            AddValidArgument("setEnvironmentVariable", true, true, "Set an environment variable to the value of the specified key");
            AddValidArgument("out", false, false, "The file to write the read value to");
            AddValidArgument("vault", false, false, "The vault sql lite file to operate on");
            AddValidArgument("delete", true, false, "When loading a yaml or json file, determines if the file is deleted after load");
        }
    }
}