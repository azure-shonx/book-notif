namespace net.shonx.books;

using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

public class List
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("owner")]
    public string Owner { get; set; }
    [JsonPropertyName("books")]
    public List<string> BookISBNs { get; set; }
    [JsonPropertyName("old_books")]
    public List<string> PreviousBookISBNs { get; set; }
    [JsonPropertyName("subscribers")]
    public List<string> Subscribers { get; set; }

    [JsonConstructor]
    public List(string id, string Owner, List<string> BookISBNs, List<string> PreviousBookISBNs, List<string> Subscribers)
    {
        Id = id;
        this.Owner = Owner;
        this.PreviousBookISBNs = PreviousBookISBNs;
        this.BookISBNs = BookISBNs;
        this.Subscribers = Subscribers;
    }

    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not List l)
            return false;
        return l.Id.Equals(Id) &&
            l.Owner.Equals(Owner) &&
            EqualityComparer.ListsEqual(l.BookISBNs, BookISBNs) &&
            EqualityComparer.ListsEqual(l.Subscribers, Subscribers);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode() ^
            Owner.GetHashCode() ^
            BookISBNs.GetHashCode() ^
            Subscribers.GetHashCode();
    }
}