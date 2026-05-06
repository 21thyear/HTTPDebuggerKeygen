# HTTPDebuggerKeygen

A C# keygen for [HTTP Debugger Pro](https://www.httpdebugger.com/), reverse-engineered from `HTTPDebuggerUI.exe`.

Tested on version 9.0.0.12.

## Build

```sh
dotnet build -c Release
```

## Usage

```sh
HTTPDebuggerKeygen.exe
```

Writes the generated key to `HKCU\SOFTWARE\MadeForNet\HTTPDebuggerPro`. Restart HTTP Debugger.

## License

Public domain. Educational use only.
