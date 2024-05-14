namespace net.shonx.books;

using System.Text.Json.Serialization;

public class ServiceBusMessage(string id, string subject, Event data, string eventType, string dataVersion, string metadataVersion, DateTime eventTime, string topic)
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = id;
    [JsonPropertyName("subject")]
    public string Subject { get; set; } = subject;
    [JsonPropertyName("data")]
    public Event Event { get; set; } = data;
    [JsonPropertyName("eventType")]
    public string EventType { get; set; } = eventType;
    [JsonPropertyName("dataVersion")]
    public string DataVersion { get; set; } = dataVersion;
    [JsonPropertyName("metadataVersion")]
    public string MetadataVersion { get; set; } = metadataVersion;
    [JsonPropertyName("eventTime")]
    public DateTime EventTime { get; set; } = eventTime;
    [JsonPropertyName("topic")]
    public string Topic { get; set; } = topic;
}
