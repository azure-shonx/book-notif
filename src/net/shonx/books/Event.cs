namespace net.shonx.books;

using System.Text.Json.Serialization;

[method: JsonConstructor]
public class Event(string ListID, Dictionary<string, string> Message, List<string> Emails)
{
  [JsonPropertyName("id")]
  public string ListID = ListID;
  [JsonPropertyName("Message")]
  public Dictionary<string, string> Message { get; } = Message ?? throw new NullReferenceException("Message");
  [JsonPropertyName("Emails")]
  public List<string> Emails { get; } = Emails ?? throw new NullReferenceException("Emails");
}