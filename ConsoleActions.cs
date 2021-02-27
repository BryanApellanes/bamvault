using System;
using System.Collections.Generic;
using System.IO;
using Bam.Net.Automation;
using Bam.Net.CommandLine;
using Bam.Net.Data.SQLite;
using Bam.Net.Encryption;

namespace Bam.Net
{
    public class ConsoleActions : CommandLineTool
    {
        [ConsoleAction("import", "Import json or yaml key value pairs into a vault")]
        public void ImportDictionary()
        {
            string toImport = GetArgumentOrDefault("import", string.Empty);
            if (string.IsNullOrEmpty(toImport) || !File.Exists(toImport))
            {
                string fileName = string.IsNullOrEmpty(toImport) ? "import.json" : toImport;
                toImport = Path.Combine(BamProfile.DataPath, fileName);
                if (!File.Exists(toImport))
                {
                    toImport = Path.Combine(BamProfile.DataPath, "import.yaml");
                }
            }
            FileInfo toImportFile = new FileInfo(toImport);
            if (!toImportFile.Exists)
            {
                Message.PrintLine("Unable to find file to import: {0}", ConsoleColor.Red, toImport);
                Exit(1);
            }
            string specifiedVault = GetArgumentOrDefault("vault", "Profile");
            Vault vault = GetTarget(specifiedVault);

            Message.PrintLine("Importing values from {0} into {1}.", ConsoleColor.Cyan, toImportFile.FullName, ((SQLiteDatabase)vault.Database).DatabaseFile.FullName);
            
            Dictionary<string, string> test = toImportFile.FromFile<Dictionary<string, string>>();
            test.Keys.Each(key => vault.Set(key, test[key]));
            if (Arguments.Contains("delete"))
            {
                toImportFile.Delete();
            }
        }
        
        [ConsoleAction("write", "Write a value to a bam vault")]
        public void Write()
        {
            string target = GetArgumentOrDefault("write", string.Empty);
            Vault vault = GetTarget(target);

            string key = GetArgument("key", "Please enter the key of the vault value to write");
            string value = GetArgument("value", "Please enter the value to write");
            
            vault.Set(key, value);
        }

        [ConsoleAction("read", "Read a value from a bam vault")]
        public void Read()
        {
            string target = GetArgumentOrDefault("read", string.Empty);
            Vault vault = GetTarget(target);
            Message.PrintLine("Using vault {0}", ConsoleColor.Cyan, vault.Database.ConnectionString);

            string key = GetArgument("key", "Please enter the key of the vault value to read");
            string value = vault.Get(key);
            Message.PrintLine(value);
            if (Arguments.Contains("setEnvironmentVariable") || Arguments.Contains("sev"))
            {
                Environment.SetEnvironmentVariable(key, value);
                Message.PrintLine("Set environment variable: {0}={1}", ConsoleColor.Yellow, key, value);
            }

            if (Arguments.Contains("out"))
            {
                FileInfo outputTo = new FileInfo(Arguments["out"]);
                outputTo.FullName.SafeWriteFile(value, true);
            }
        }

        [ConsoleAction("print", "Print values from a bam vault")]
        public void Print()
        {
            string target = GetArgumentOrDefault("print", "");
            target = string.IsNullOrEmpty(target) ? "Profile" : target;
            Vault vault = GetTarget(target);
            Message.PrintLine("Printing vault {0}", ConsoleColor.Cyan, vault.Database.ConnectionString);
            vault.Keys.Each(key => Message.PrintLine("{0} : {1}", key, vault[key]));
        }
        
        private static Vault GetTarget(string target)
        {
            Vault vault = Vault.System;
            if (Enum.TryParse(target, out KnownVaults targetVault))
            {
                switch (targetVault)
                {
                    case KnownVaults.Invalid:
                    case KnownVaults.System:
                        vault = Vault.System;
                        break;
                    case KnownVaults.Profile:
                        vault = Vault.Profile;
                        break;
                    case KnownVaults.Application:
                        vault = Vault.Application;
                        break;
                }
            }
            else
            {
                FileInfo vaultFile = new FileInfo(target);
                string name = Path.GetFileNameWithoutExtension(target);
                vault = Vault.Create(vaultFile, name);
            }

            return vault;
        }
    }
}