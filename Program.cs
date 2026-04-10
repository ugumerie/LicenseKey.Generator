using LicenseKey.Generator.LicenseKeyGenerator;

bool useColor = !Console.IsOutputRedirected
	&& !args.Contains("--no-color", StringComparer.OrdinalIgnoreCase);

WriteLineWithOptionalColor("=== License Key Generator Started ===", ConsoleColor.Magenta, useColor);
Console.WriteLine();

ECDsaLicenseKeyCreator.GenerateLicenseKeys(useColor);

Console.WriteLine();
WriteLineWithOptionalColor(
	"License key generation completed. Copy the private key PEM into your secret manager and distribute only the public key.",
	ConsoleColor.Green,
	useColor);

static void WriteLineWithOptionalColor(string message, ConsoleColor color, bool useColor)
{
	if (!useColor)
	{
		Console.WriteLine(message);
		return;
	}

	Console.ForegroundColor = color;
	Console.WriteLine(message);
	Console.ResetColor();
}