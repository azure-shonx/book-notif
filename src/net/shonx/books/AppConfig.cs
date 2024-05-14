namespace net.shonx.books;

using System.Diagnostics.CodeAnalysis;

public static class AppConfig
{
    public static readonly string APIM_KEY = NoNull(Environment.GetEnvironmentVariable("API_KEY"), "API_KEY");
    public static readonly string EMAIL_COMMUNICATION_KEY = NoNull(Environment.GetEnvironmentVariable("EMAIL_COMMUNICATION_KEY"), "EMAIL_COMMUNICATION_KEY");

    private static string NoNull([NotNullWhen(false)] string? Value, string Key)
    {
        if (string.IsNullOrEmpty(Value))
            throw new NullReferenceException($"{Key} is not set.");
        return Value;
    }
}