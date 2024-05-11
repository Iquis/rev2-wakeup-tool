using System.Text.Json.Serialization;

namespace GGXrdReversalTool.Library.Configuration;

public class ReversalToolConfigObject
{
    [JsonPropertyName("Logging")]
    public Logging Logging { get; set; }

    [JsonPropertyName("GGProcessName")]
    public string GGProcessName { get; set; }

    [JsonPropertyName("P2IdOffset")]
    public string P2IdOffset { get; set; }

    [JsonPropertyName("ScriptOffset")]
    public string ScriptOffset { get; set; }

    [JsonPropertyName("UpdateLink")]
    public string UpdateLink { get; set; }

    [JsonPropertyName("CurrentVersion")]
    public Version CurrentVersion { get; set; }

    [JsonPropertyName("AutoUpdate")]
    public bool AutoUpdate { get; set; }
}

public class Logging
{
    [JsonPropertyName("LogLevel")]
    public LogLevel LogLevel { get; set; }
}

public class LogLevel
{
    [JsonPropertyName("Default")]
    public string Default { get; set; }

    [JsonPropertyName("System")]
    public string System { get; set; }

    [JsonPropertyName("Microsoft")]
    public string Microsoft { get; set; }
}