namespace Bam.Net
{
    public partial class Program : CommandLineTool
    {
        public static void Main(string[] args)
        {
            AddArguments();
            ExecuteMainOrInteractive(args);
        }
    }
}