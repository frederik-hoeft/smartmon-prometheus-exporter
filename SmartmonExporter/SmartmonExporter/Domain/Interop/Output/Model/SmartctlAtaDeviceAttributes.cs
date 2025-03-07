using System.Text.Json;
using System.Text.Json.Serialization;

namespace SmartmonExporter.Domain.Interop.Output.Model;

internal sealed record SmartctlAtaDeviceAttributes(AtaDeviceAttributes AtaSmartAttributes) : SmartctlDevice;

internal sealed record SmartctlNvmeDeviceAttributes
(
    [property: JsonConverter(typeof(NvmeSmartHealthInformationLogConverter))] Dictionary<string, long> NvmeSmartHealthInformationLog,
    Temperature Temperature,
    int PowerCycleCount,
    PowerOnTime PowerOnTime
) : SmartctlDevice;

internal sealed class NvmeSmartHealthInformationLogConverter : JsonConverter<Dictionary<string, long>>
{
    public override Dictionary<string, long> Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        Dictionary<string, long> dictionary = [];
        while (reader.Read())
        {
            if (reader.TokenType == JsonTokenType.EndObject)
            {
                break;
            }
            if (reader.TokenType != JsonTokenType.PropertyName)
            {
                throw new JsonException();
            }
            string key = reader.GetString() ?? throw new JsonException();
            reader.Read();
            // ignore non-integer values
            if (reader.TokenType != JsonTokenType.Number)
            {
                reader.Skip();
                continue;
            }
            long value = reader.GetInt64();
            dictionary.Add(key, value);
        }
        return dictionary;
    }
    public override void Write(Utf8JsonWriter writer, Dictionary<string, long> value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        foreach ((string key, long entry) in value)
        {
            writer.WriteNumber(key, entry);
        }
        writer.WriteEndObject();
    }
}

internal sealed record NvmeSmartHealthInformationLog
(
    int CriticalWarning,
    int Temperature,
    int AvailableSpare,
    int AvailableSpareThreshold,
    int PercentageUsed,
    int DataUnitsRead,
    int DataUnitsWritten,
    int HostReads,
    int HostWrites,
    int ControllerBusyTime,
    int PowerCycles,
    int PowerOnHours,
    int UnsafeShutdowns,
    int MediaErrors,
    int NumErrLogEntries,
    int WarningTempTime,
    int CriticalCompTime,
    int[] TemperatureSensors
);