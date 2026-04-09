# License Key Generator App

This project is a .NET console application that generates an ECDSA key pair for license signing and verification.

The app:

- Creates a new ECDSA key pair using the NIST P-256 curve
- Exports the private key in Base64 format for signing licenses
- Exports the public key in Base64 format for verifying licenses
- Writes both keys to disk in an `license_output` folder
- Demonstrates signing sample license data and verifying the signature

## What It Generates

When the app runs, it creates these files:

- `license_output/license_private.key`
- `license_output/license_public.key`

The output folder is created relative to the executable location by using `AppContext.BaseDirectory`.

That means:

- When running with `dotnet run`, the files are written under the build output folder, typically `bin/Debug/net10.0/license_output`
- When running a published build, the files are written under the published app folder in `license_output`

## Requirements

- .NET 10 SDK to build and publish the project
- For framework-dependent published builds, the matching .NET runtime on the target machine

## Publish Target Quick Reference

| Platform | Architecture  | Runtime Identifier | Publish Output Example |
| -------- | ------------- | ------------------ | ---------------------- |
| macOS    | Apple Silicon | `osx-arm64`        | `./publish/`           |
| macOS    | Intel         | `osx-x64`          | `./publish/`           |
| Windows  | x64           | `win-x64`          | `./publish/`           |
| Windows  | ARM64         | `win-arm64`        | `./publish/`           |
| Linux    | x64           | `linux-x64`        | `./publish/`           |
| Linux    | ARM64         | `linux-arm64`      | `./publish/`           |

## Run Locally

From the project root:

```bash
dotnet run
```

## Publish And Run On macOS

### macOS framework-dependent publish

This produces a platform-specific macOS build that requires the .NET runtime to be installed on the target machine.

```bash
dotnet publish -c Release -r osx-arm64 --self-contained false -o ./publish
```

If you are targeting Intel macOS instead, use:

```bash
dotnet publish -c Release -r osx-x64 --self-contained false -o ./publish
```

The published files will be under a path similar to:

```text
./publish/
```

Run the published app from that folder:

```bash
./LicenseKey.Generator
```

### macOS self-contained publish

This includes the runtime with the app.

```bash
dotnet publish -c Release -r osx-arm64 --self-contained true -o ./publish
```

Run it the same way:

```bash
./LicenseKey.Generator
```

## Publish And Run On Windows

### Windows framework-dependent publish

This produces a Windows build that requires the .NET runtime to be installed on the target machine.

```bash
dotnet publish -c Release -r win-x64 --self-contained false -o ./publish
```

The published files will be under a path similar to:

```text
./publish/
```

Run the published executable:

```powershell
.\LicenseKey.Generator.exe
```

If you need Windows on ARM instead, use:

```powershell
dotnet publish -c Release -r win-arm64 --self-contained false -o ./publish
```

### Windows self-contained publish

This includes the runtime with the app.

```powershell
dotnet publish -c Release -r win-x64 --self-contained true -o ./publish
```

Run it with:

```powershell
.\LicenseKey.Generator.exe
```

## Publish And Run On Linux

### Linux framework-dependent publish

This produces a Linux build that requires the .NET runtime to be installed on the target machine.

```bash
dotnet publish -c Release -r linux-x64 --self-contained false -o ./publish
```

If you are targeting Linux ARM64 instead, use:

```bash
dotnet publish -c Release -r linux-arm64 --self-contained false -o ./publish
```

The published files will be under a path similar to:

```text
./publish/
```

Run the published app from that folder:

```bash
./LicenseKey.Generator
```

### Linux self-contained publish

This includes the runtime with the app.

```bash
dotnet publish -c Release -r linux-x64 --self-contained true -o ./publish
```

Run it with:

```bash
./LicenseKey.Generator
```

## Example Behavior

When the app runs, it:

1. Prints the generated public key to the console
2. Saves both keys into the `license_output` folder next to the executable
3. Signs sample license data using the private key
4. Verifies the signature using the public key
5. Prints whether the signature is valid

## Security Note

The private key is sensitive and should not be distributed with a client application.

- Keep `license_private.key` secure
- Distribute only the public key to applications that need to verify licenses
- The app no longer prints the private key to stdout, but the file on disk still requires secure handling
