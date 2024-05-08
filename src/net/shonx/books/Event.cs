namespace net.shonx.books;

using System.Text.Json.Serialization;

[method: JsonConstructor]
public class Event(Dictionary<string, string> messages, List<string> emails)
{
    public Dictionary<string, string> Message { get; } = messages;
    public List<string> Emails { get; } = emails;
}