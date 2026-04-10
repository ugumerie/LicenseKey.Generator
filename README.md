# License Key Generator App

This project is a .NET console application that generates an ECDSA key pair for license signing and verification.

The app:

- Creates a new ECDSA key pair using the NIST P-256 curve
- Prints the private key as PEM to stdout so you can copy it directly into a secret manager
- Prints the public key as PEM for use in applications that verify licenses
- Demonstrates signing sample license data and verifying the signature with `ImportFromPem`

## Security Model

The private key must never be written to disk.

Recommended flow:

1. Run this tool locally one time.
2. Copy the private key PEM from the terminal.
3. Paste that PEM value into your secret manager or environment configuration.
4. Keep only the public key in the application that verifies licenses.

This avoids the common failure mode where a private key is left in a repository, build artifact, container image, or compromised host filesystem.

## What It Outputs

When the app runs, it prints:

- A private key PEM block for signing
- A public key PEM block for verification
- Sample signing and verification output

No key files are created.

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

To force plain, uncolored output:

```bash
dotnet run -- --no-color
```

After it prints the PEM blocks:

1. Copy the private key PEM into your secret store.
2. Copy the public key PEM into the application that verifies licenses.
3. Do not redirect the output into files containing private key material.

Color output is also disabled automatically when stdout is redirected or piped.

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

## Runtime Usage

At runtime, load the private key from configuration or a secret manager and import it directly from PEM.

```csharp
string pem = Environment.GetEnvironmentVariable("LICENCE_PRIVATE_KEY")!
 .Replace("\\n", "\n");

using var ecdsa = ECDsa.Create();
ecdsa.ImportFromPem(pem);
```

If you store the PEM in an environment variable, escaped newlines are common, which is why the example normalizes `\n` into real newlines before calling `ImportFromPem`.

## Example Behavior

When the app runs, it:

1. Prints the generated private key PEM to the console
2. Prints the generated public key PEM to the console
3. Shows the recommended runtime import pattern using `LICENCE_PRIVATE_KEY`
4. Signs sample license data using the private key PEM
5. Verifies the signature using the public key PEM
6. Prints whether the signature is valid

## Security Note

The private key is your signing authority.

- Never commit it to Git
- Never package it into publish output or container images
- Never place it on client machines
- Distribute only the public key to license-verifying applications
