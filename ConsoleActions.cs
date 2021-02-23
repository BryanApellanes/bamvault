﻿using System;
using System.Collections.Generic;
using System.IO;
using Bam.Net.Automation;
using Bam.Net.CommandLine;
using Bam.Net.Encryption;

namespace Bam.Net
{
    public class ConsoleActions : CommandLineTool
    {
        [ConsoleAction("load", "Load json or yaml key values into a vault")]
        public void LoadDictionary()
        {
            string toLoad = GetArgumentOrDefault("load", string.Empty);
            if (string.IsNullOrEmpty(toLoad) || !File.Exists(toLoad))
            {
                string fileName = string.IsNullOrEmpty(toLoad) ? "load.json" : toLoad;
                toLoad = Path.Combine(BamProfile.DataPath, fileName);
                if (!File.Exists(toLoad))
                {
                    toLoad = Path.Combine(BamProfile.DataPath, "load.yaml");
                }
            }
            FileInfo toLoadFile = new FileInfo(toLoad);
            if (!toLoadFile.Exists)
            {
                Message.PrintLine("Unable to find file to load: {0}", ConsoleColor.Magenta, toLoad);
                Exit(1);
            }
            string specifiedVault = GetArgumentOrDefault("vault", "Profile");
            Vault vault = GetTarget(specifiedVault);

            Dictionary<string, string> test = toLoadFile.FromFile<Dictionary<string, string>>();
            test.Keys.Each(key => vault.Set(key, test[key]));
            if (Arguments.Contains("delete"))
            {
                toLoadFile.Delete();
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