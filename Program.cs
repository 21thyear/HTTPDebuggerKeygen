using System;
using Microsoft.Win32;

class Program
{
    const string RegPath = @"SOFTWARE\MadeForNet\HTTPDebuggerPro";

    static int Main(string[] args)
    {
        var nameGen = new NameGenerator();
        string label = "User";

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--exe"   when i + 1 < args.Length: nameGen.InstallPath         = args[++i]; break;
                case "--ver"   when i + 1 < args.Length: nameGen.FileVersionOverride = args[++i]; break;
                case "--label" when i + 1 < args.Length: label                       = args[++i]; break;
                case "-h":
                case "--help":
                    PrintUsage();
                    return 0;
            }
        }

        string snName, keyValue;
        try
        {
            snName = nameGen.Create();
            keyValue = new KeyGenerator().Create();
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("error: " + ex.Message);
            return 1;
        }

        using (var key = Registry.CurrentUser.CreateSubKey(RegPath))
        {
            key.SetValue(snName,  keyValue, RegistryValueKind.String);
            key.SetValue("Label", label,    RegistryValueKind.String);
        }

        Console.WriteLine($"versionInt : {nameGen.LastVersionInt}");
        Console.WriteLine($"vsn        : 0x{nameGen.LastVolumeOrVersion:X8}");
        Console.WriteLine($"name       : {snName}");
        Console.WriteLine($"key        : {keyValue}");
        Console.WriteLine($"label      : {label}");
        Console.WriteLine($"wrote HKCU\\{RegPath}");
        return 0;
    }

    static void PrintUsage()
    {
        Console.WriteLine("HTTPDebuggerKeygen — patches HKCU registry to activate HTTP Debugger Pro");
        Console.WriteLine();
        Console.WriteLine("usage: HTTPDebuggerKeygen [--exe <path>] [--ver <version>] [--label <name>]");
        Console.WriteLine();
        Console.WriteLine("  --exe <path>     Path to HTTPDebuggerUI.exe (default: Program Files lookup)");
        Console.WriteLine("  --ver <version>  Override FileVersion string (e.g. 9.0.0.12)");
        Console.WriteLine("  --label <name>   Display name (default: User)");
    }
}
