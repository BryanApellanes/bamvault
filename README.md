# bamvault

Used to manage sensitive information in a Bam Application installation.

## Read
To read a value from a vault use the following command line arguments:

- /read - Specify a value from `KnownVaults` enum or the path to an existing Sqlite vault file.
- /key - Specify the key to read from the vault.
- /setEnvironmentVariable - If specified, sets an environment variable to the value of the specified key.
- /out - If specified, writes the value to a file with the name of the value specified.

Following is an example using `bamvault` to read a value from the `Profile` vault, set an environment variable and write the value to the file `./secret.txt`.

```cmd
bamvault /read:Profile /key:Secret /setEnvironmentVariable /out:./secret.txt
```

The resulting environment variable is named `Secret`.

## Write
To write a value to a vault use the following command line arguments:

- /write - Specify a value from `KnownVaults` enum or the path to an existing Sqlite vault file.
- /key - Specify the key to write to the vault.
- /value - Specify the value to assoicate with the specified key.

Following is an example using `bamvault` to read a value from the `Application` vault.

```cmd
bamvault /write:Application /key:Secret /value:superSecretPassword
```

## Print
To print all the key value pairs in a vault use the following command line argument:

- /print: Specify a value from the `KnownVaults` enum or the path to an existing Sqlite vault file.

Following is an example using `bamvault` to print the values in the `System` vault.

```cmd
bamvault /print:System
```
## Import
To import values from a file use the following command line arguments:

- /import - Specify either the full path to a `json` or `yaml` file, or the relative path of a file that exists in `BamProfile.DataPath`.  If no value is specified values are imported from either `import.json` or `import.yaml` from the directory `BamProfile.DataPath`.
- /vault - Specify the vault to import values into, the default is `Profile` if not specified.
- /delete - If specified, deletes the file after loading its values.

Following is an example using `bamvault` to import the values from the file `secret-values.yaml` into the `Profile` vault.

```cmd
bamvault /import:secret-values.yaml
```

## Known Vault Names
There are three known vaults commonly used in a Bam Application, they are:

- `Profile` - A vault file located at `$"{BamProfile.DataPath}/Profile.vault.sqlite"`.
- `Application` - A vault file located at `$"{BamHome.DataPath}/Application_{ApplicationName}.vault.sqlite"`, where `{ApplicationName}` is determined by reading the `appSetting` with the key of `ApplicationName`. 
- `System` - A vault file located at `$"{BamHome.DataPath}/System.vault.sqlite"`.