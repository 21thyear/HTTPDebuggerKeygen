using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

class NameGenerator
{
    [DllImport("kernel32.dll", CharSet = CharSet.Ansi, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetVolumeInformationA(
        [MarshalAs(UnmanagedType.LPStr)] string lpRootPathName,
        StringBuilder lpVolumeNameBuffer, uint nVolumeNameSize,
        out uint lpVolumeSerialNumber,
        out uint lpMaximumComponentLength,
        out uint lpFileSystemFlags,
        StringBuilder lpFileSystemNameBuffer, uint nFileSystemNameSize);

    [DllImport("kernel32.dll")]
    private static extern uint GetVersion();

    public string InstallPath { get; set; }
    public string FileVersionOverride { get; set; }
    public uint LastVersionInt { get; private set; }
    public uint LastVolumeOrVersion { get; private set; }

    static readonly string[] DefaultExeCandidates =
    {
        @"C:\Program Files (x86)\HTTPDebuggerPro\HTTPDebuggerUI.exe",
        @"C:\Program Files\HTTPDebuggerPro\HTTPDebuggerUI.exe",
    };

    string ReadFileVersion()
    {
        if (!string.IsNullOrEmpty(FileVersionOverride))
            return FileVersionOverride;

        var exe = InstallPath;
        if (exe == null)
            foreach (var p in DefaultExeCandidates)
                if (File.Exists(p)) { exe = p; break; }

        if (exe == null)
            throw new FileNotFoundException(
                "HTTPDebuggerUI.exe not found. Pass --exe <path> or --ver <version>.");

        var info = FileVersionInfo.GetVersionInfo(exe);
        return info.FileVersion ?? throw new InvalidOperationException(
            "FileVersion missing from " + exe);
    }

    static uint VersionStringToInt(string fileVersion)
    {
        var digits = new StringBuilder(fileVersion.Length);
        foreach (var c in fileVersion)
            if (c >= '0' && c <= '9') digits.Append(c);
        return digits.Length == 0 ? 0u : uint.Parse(digits.ToString());
    }

    static uint ReadVolumeSerial(string driveLetter)
    {
        var label = new StringBuilder(256);
        var fs = new StringBuilder(256);
        bool ok = GetVolumeInformationA(
            driveLetter + ":\\", label, (uint)label.Capacity,
            out uint vsn, out _, out _,
            fs, (uint)fs.Capacity);
        return ok ? vsn : GetVersion();
    }

    public string Create()
    {
        uint versionInt = VersionStringToInt(ReadFileVersion());
        uint vsn = ReadVolumeSerial("C");

        LastVersionInt = versionInt;
        LastVolumeOrVersion = vsn;

        uint id = versionInt ^ ((~vsn >> 1) + 736) ^ 0x590D4u;
        return $"SN{id}";
    }
}
