namespace net.shonx.books;

using System.Collections.Generic;
using System.Text.Json.Serialization;

public class Book
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("Title")]
    public string Title { get; set; }
    [JsonPropertyName("authors")]
    public List<string> Authors { get; set; }
    [JsonPropertyName("publication_year")]
    public string PublicationYear { get; set; }
    [JsonPropertyName("genre")]
    public string Genre { get; set; }
    [JsonPropertyName("cover-image")]
    public string CoverImage { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
    public Book()
    {
        // Json will fix it, don't worry...
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

    public override bool Equals(object? obj)
    {
        if (obj is not Book o)
            return false;
        return o.Id.Equals(Id) &&
            o.Title.Equals(Title) &&
            EqualityComparer.ListsEqual(o.Authors, Authors) &&
            o.PublicationYear.Equals(PublicationYear) &&
            o.Genre.Equals(Genre) &&
            o.CoverImage.Equals(CoverImage);
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode() ^
            Title.GetHashCode() ^
            Authors.GetHashCode() ^
            PublicationYear.GetHashCode() ^
            Genre.GetHashCode() ^
            CoverImage.GetHashCode();
    }
}