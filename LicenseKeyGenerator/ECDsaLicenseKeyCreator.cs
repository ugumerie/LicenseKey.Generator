using System.Security.Cryptography;
using System.Text;

namespace LicenseKey.Generator.LicenseKeyGenerator;

public class ECDsaLicenseKeyCreator
{
    private static bool s_useColor = true;

    public static void GenerateLicenseKeys(bool useColor = true)
    {
        s_useColor = useColor;

        using var ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);
        string privateKeyPem = ecdsa.ExportECPrivateKeyPem();
        string publicKeyPem = ecdsa.ExportSubjectPublicKeyInfoPem();

        WriteSection("=== ECDSA Key Generation ===");
        WriteLabelValue("Algorithm", "ECDSA");
        WriteLabelValue("Curve", "NIST P-256 (secp256r1)");
        WriteLabelValue("Key Size", "256 bits");
        Console.WriteLine();

        WriteWarning("Private Key PEM (copy to your secret manager; do not write this to disk):");
        WriteBlock(privateKeyPem, ConsoleColor.Red);
        Console.WriteLine();

        WriteInfo("Public Key PEM (distribute with your application for verification):");
        WriteBlock(publicKeyPem, ConsoleColor.Cyan);
        Console.WriteLine();

        // Get key parameters
        ECParameters parameters = ecdsa.ExportParameters(true);

        WriteSection("Key Parameters");
        WriteBullet($"Curve: {parameters.Curve.Oid.FriendlyName}");
        WriteBullet($"Q.X (public key X coordinate): {Convert.ToBase64String(parameters.Q.X!)}");
        WriteBullet($"Q.Y (public key Y coordinate): {Convert.ToBase64String(parameters.Q.Y!)}");
        Console.WriteLine();
        WriteSection("Runtime Example");
        WriteBlock("string pem = Environment.GetEnvironmentVariable(\"LICENCE_PRIVATE_KEY\")!.Replace(\\\"\\n\\\", \"\\n\");\nusing var signingKey = ECDsa.Create();\nsigningKey.ImportFromPem(pem);", ConsoleColor.DarkGray);

        // Demonstrate signing and verification
        DemonstrateSigningAndVerification(privateKeyPem, publicKeyPem);
    }

    private static void DemonstrateSigningAndVerification(string privateKeyPem, string publicKeyPem)
    {
        WriteSection("=== Demonstrating Signing and Verification ===");
        Console.WriteLine();

        // Sample license data to sign
        string licenseData = "LICENSEE:Demo Company|EXPIRY:2026-12-31|FEATURES:FeatureA,FeatureB|TYPE:Premium";
        byte[] licenseDataBytes = Encoding.UTF8.GetBytes(licenseData);

        // Sign the license data
        using var ecdsaForSigning = ECDsa.Create();
        ecdsaForSigning.ImportFromPem(privateKeyPem);

        // Create a signature for the license data
        byte[] signature = ecdsaForSigning.SignData(licenseDataBytes, HashAlgorithmName.SHA256);
        string signatureBase64 = Convert.ToBase64String(signature);

        WriteLabelValue("License Data", licenseData);
        WriteLabelValue("Signature (Base64)", signatureBase64);

        // Verify the signature using the public key
        using var ecdsaForVerification = ECDsa.Create();
        ecdsaForVerification.ImportFromPem(publicKeyPem);

        bool isSignatureValid = ecdsaForVerification.VerifyData(licenseDataBytes, signature, HashAlgorithmName.SHA256);

        WriteStatus($"Verification Result: {(isSignatureValid ? "VALID" : "INVALID")}", isSignatureValid);
    }

    private static void WriteSection(string message)
    {
        WriteColoredLine(message, ConsoleColor.Magenta);
    }

    private static void WriteInfo(string message)
    {
        WriteColoredLine(message, ConsoleColor.Blue);
    }

    private static void WriteWarning(string message)
    {
        WriteColoredLine(message, ConsoleColor.Yellow);
    }

    private static void WriteBullet(string message)
    {
        WriteColoredLine($"- {message}", ConsoleColor.Green);
    }

    private static void WriteLabelValue(string label, string value)
    {
        if (!s_useColor)
        {
            Console.WriteLine($"{label}: {value}");
            return;
        }

        var originalForeground = Console.ForegroundColor;

        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.Write($"{label}: ");

        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine(value);

        Console.ForegroundColor = originalForeground;
    }

    private static void WriteBlock(string content, ConsoleColor color)
    {
        WriteColoredLine(content, color);
    }

    private static void WriteStatus(string message, bool isSuccess)
    {
        WriteColoredLine(message, isSuccess ? ConsoleColor.Green : ConsoleColor.Red);
    }

    private static void WriteColoredLine(string message, ConsoleColor color)
    {
        if (!s_useColor)
        {
            Console.WriteLine(message);
            return;
        }

        var originalForeground = Console.ForegroundColor;
        Console.ForegroundColor = color;
        Console.WriteLine(message);
        Console.ForegroundColor = originalForeground;
    }
}
