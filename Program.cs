using LicenseKey.Generator.LicenseKeyGenerator;

Console.WriteLine("=== License Key Generator Started ===");
Console.WriteLine();

ECDsaLicenseKeyCreator.GenerateLicenseKeys();

Console.WriteLine();
Console.WriteLine("License key generation completed. Check the license_output folder for generated keys.");