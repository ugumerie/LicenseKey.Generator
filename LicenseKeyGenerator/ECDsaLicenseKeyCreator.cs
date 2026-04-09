using System.Security.Cryptography;
using System.Text;

namespace LicenseKey.Generator.LicenseKeyGenerator;

public class ECDsaLicenseKeyCreator
{
    public static void GenerateLicenseKeys()
    {
        using var ecdsa = ECDsa.Create(ECCurve.NamedCurves.nistP256);

        // Private key (for signing)
        byte[] privateKey = ecdsa.ExportECPrivateKey();
        string privateKeyBase64 = Convert.ToBase64String(privateKey);

        // Public key (for verification)
        byte[] publicKey = ecdsa.ExportSubjectPublicKeyInfo();
        string publicKeyBase64 = Convert.ToBase64String(publicKey);

        Console.WriteLine("=== ECDSA Key Generation ===");
        Console.WriteLine("Algorithm: ECDSA");
        Console.WriteLine($"Curve: NIST P-256 (secp256r1)");
        Console.WriteLine($"Key Size: 256 bits");
        Console.WriteLine();

        Console.WriteLine("Private key generated and saved to the license_output folder.");

        Console.WriteLine("Public Key (Distribute with application - Used for verification):");
        Console.WriteLine(publicKeyBase64);
        Console.WriteLine();

        // Get key parameters
        ECParameters parameters = ecdsa.ExportParameters(true);

        Console.WriteLine("Key Parameters:");
        Console.WriteLine($"- Curve: {parameters.Curve.Oid.FriendlyName}");
        Console.WriteLine($"- Q.X (public key X coordinate): {Convert.ToBase64String(parameters.Q.X!)}");
        Console.WriteLine($"- Q.Y (public key Y coordinate): {Convert.ToBase64String(parameters.Q.Y!)}");

        // Save keys to files
        SaveKeysToFile(privateKeyBase64, publicKeyBase64);

        // Demonstrate signing and verification
        DemostrateSigningAndVerification(privateKeyBase64, publicKeyBase64);
    }

    private static void SaveKeysToFile(string privateKeyBase64, string publicKeyBase64)
    {
        string outputDirectoryPath = Path.Combine(AppContext.BaseDirectory, "license_output");
        if (!Directory.Exists(outputDirectoryPath))
            Directory.CreateDirectory(outputDirectoryPath);

        string privateKeyPath = Path.Combine(outputDirectoryPath, "license_private.key");
        string publicKeyPath = Path.Combine(outputDirectoryPath, "license_public.key");

        File.WriteAllText(privateKeyPath, privateKeyBase64);
        Console.WriteLine($"Private key saved to: {privateKeyPath}");

        File.WriteAllText(publicKeyPath, publicKeyBase64);
        Console.WriteLine($"Public key saved to: {publicKeyPath}");
        Console.WriteLine();
    }

    private static void DemostrateSigningAndVerification(string privateKeyBase64, string publicKeyBase64)
    {
        Console.WriteLine("=== Demonstrating Signing and Verification ===");
        Console.WriteLine();

        // Sample license data to sign
        string licenseData = "LICENSEE:Demo Company|EXPIRY:2026-12-31|FEATURES:FeatureA,FeatureB|TYPE:Premium";
        byte[] licenseDataBytes = Encoding.UTF8.GetBytes(licenseData);

        // Sign the license data
        byte[] privateKeyBytes = Convert.FromBase64String(privateKeyBase64);

        using var ecdsaForSigning = ECDsa.Create();
        ecdsaForSigning.ImportECPrivateKey(privateKeyBytes, out _);

        // Create a signature for the license data
        byte[] signature = ecdsaForSigning.SignData(licenseDataBytes, HashAlgorithmName.SHA256);
        string signatureBase64 = Convert.ToBase64String(signature);

        Console.WriteLine($"License Data: {licenseData}");
        Console.WriteLine($"Signature (Base64): {signatureBase64}");

        // Verify the signature using the public key
        byte[] publicKeyBytes = Convert.FromBase64String(publicKeyBase64);
        using var ecdsaForVerification = ECDsa.Create();
        ecdsaForVerification.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);

        bool isSignatureValid = ecdsaForVerification.VerifyData(licenseDataBytes, signature, HashAlgorithmName.SHA256);

        Console.WriteLine($"Verification Result: {(isSignatureValid ? "VALID" : "INVALID")}");
    }
}
