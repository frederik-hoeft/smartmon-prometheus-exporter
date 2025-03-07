using SmartmonExporter.Configuration;

namespace SmartmonExporter.Domain.Writers;

internal sealed class FileOutputWriter(IConfiguration configuration) : IOutputWriter
{
    public int Priority => 0;

    public async ValueTask WriteAsync(string metrics, CancellationToken cancellationToken)
    {
        if (configuration.Settings.WriteToConsole)
        {
            return;
        }
        string outputPath = configuration.Settings.OutputPath;
        // create directory if not exists
        string? directory = Path.GetDirectoryName(outputPath);
        if (directory is not null)
        {
            Directory.CreateDirectory(directory);
        }
        // write to file atomically
        Span<byte> randomBytes = stackalloc byte[16];
#pragma warning disable IDE0079 // Remove unnecessary suppression
#pragma warning disable CA5394 // Do not use insecure randomness
        // this is not a security-sensitive operation, don't pull in another dependency just for this
        Random.Shared.NextBytes(randomBytes);
#pragma warning restore CA5394 // Do not use insecure randomness
#pragma warning restore IDE0079 // Remove unnecessary suppression
        string tempFilePath = Path.Combine(directory ?? string.Empty, $"{Path.GetFileName(outputPath)}.{Convert.ToHexStringLower(randomBytes)}.temp");
        await File.WriteAllTextAsync(tempFilePath, metrics, cancellationToken);
        File.Replace(tempFilePath, outputPath, destinationBackupFileName: null);
    }
}
