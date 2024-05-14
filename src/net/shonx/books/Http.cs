namespace net.shonx.books;

using System.Net;
using System.Text.Json;

internal static class Http
{

    private const string APIM_URL = "https://apim-books.azure-api.net/";
    private static readonly HttpClient httpClient = new();

    internal static Book? GetBook(string ISBN)
    {
        Task<Book?> task = GetBook0(ISBN);
        task.Wait();
        return task.Result;
    }

    private static async Task<Book?> GetBook0(string ISBN)
    {
        if (string.IsNullOrEmpty(ISBN))
            return null;
        HttpRequestMessage request = new(HttpMethod.Get, BuildURL(ISBN));
        Book? b = await WriteRequest(request);
        return Verify(b);
    }

    private static async Task<Book?> WriteRequest(HttpRequestMessage request)
    {
        using HttpResponseMessage response = await httpClient.SendAsync(request);
        HttpStatusCode statusCode = response.StatusCode;
        int sc = (int)statusCode;
        if (!((sc >= 200 && sc < 300) || sc == 404))
        {
            await Console.Error.WriteLineAsync($"Helper got response {sc}: {statusCode}");
            return null;
        }
        return await GetReply(response.Content.ReadAsStream());
    }

    private static async Task<Book?> GetReply(Stream stream)
    {
        string json = await new StreamReader(stream).ReadToEndAsync();
        if (string.IsNullOrEmpty(json))
        {
            return null;
        }
        Console.WriteLine(json);
        return JsonSerializer.Deserialize<Book>(json);
    }

    private static Book? Verify(Book? book)
    {
        if (book is null)
            return null;
        return string.IsNullOrEmpty(book.Id) &&
                string.IsNullOrEmpty(book.Title) &&
                (book.Authors is null || book.Authors.Count == 0) &&
                string.IsNullOrEmpty(book.PublicationYear) &&
                string.IsNullOrEmpty(book.Genre) &&
                string.IsNullOrEmpty(book.CoverImage)
            ? null
            : book;
    }


    private static string BuildURL(string ISBN)
    {
        if (string.IsNullOrEmpty(ISBN))
            throw new NullReferenceException("ISBN");
        UriBuilder uriBuilder = new(APIM_URL);
        uriBuilder.Path += "books/get";
        uriBuilder.Query = $"id={ISBN}&key={AppConfig.APIM_KEY}";
        return uriBuilder.Uri.ToString();
    }
}