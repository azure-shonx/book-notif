namespace net.shonx.books.functions;

using System.Text;
using System.Text.Json;
using Azure.Communication.Email;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

public class StorageBusEmailer
{
    [Function("StorageBusEmailer")]
    public static void Run([ServiceBusTrigger(
        queueName: "queue",
        Connection = "ServiceBus")] string JSONEvent, FunctionContext context)
    {
        ILogger<StorageBusEmailer>? _logger = context.GetLogger<StorageBusEmailer>() ?? throw new NullReferenceException("_logger");
        _logger.LogInformation("Called ServiceBusTrigger");
        try
        {
            ServiceBusMessage SBM = JsonSerializer.Deserialize<ServiceBusMessage>(JSONEvent) ?? throw new NullReferenceException("SBM");
            Event @event = SBM.Event ?? throw new NullReferenceException("@event");

            StringBuilder htmlEmail = new();
            WriteCSS(ref htmlEmail);

            StringBuilder plainTextEmail = new();

            WriteHeaderHTML(ref htmlEmail, @event.ListID);
            WriteHeaderPlain(ref plainTextEmail, @event.ListID);

            foreach (KeyValuePair<string, string> message in @event.Message)
            {
                Book book = Http.GetBook(message.Key) ?? throw new NullReferenceException($"book:{message.Key}");
                WriteBookHTML(ref htmlEmail, book, message.Value);
                WriteBookPlain(ref plainTextEmail, book, message.Value);
            }

            WriteFooterHTML(ref htmlEmail);
            WriteFooterPlain(ref plainTextEmail);

            EmailClient client = new(AppConfig.EMAIL_COMMUNICATION_KEY);
            string finalHTML = htmlEmail.ToString();
            string finalPlain = plainTextEmail.ToString();

            foreach (string email in @event.Emails)
            {
                Task T = SendEmail(client, email, finalHTML, finalPlain);
                T.Wait();
            }

        }
        catch (Exception e)
        {
            _logger.LogError(e.ToString());
        }
    }

    private static async Task SendEmail(EmailClient EmailClient, string email, string htmlEmail, string plainTextEmail)
    {
        EmailSendOperation operation = await EmailClient.SendAsync(
            Azure.WaitUntil.Completed,
            senderAddress: "DoNotReply@books.shonx.net",
            recipientAddress: email,
            subject: "List update",
            htmlContent: htmlEmail,
            plainTextContent: plainTextEmail
        );
        var result = await operation.WaitForCompletionAsync();
    }

    private static void WriteCSS(ref StringBuilder htmlEmail)
    {
        htmlEmail.AppendLine("<style>");
        htmlEmail.AppendLine(".grid-container {");
        htmlEmail.AppendLine("  display: grid;");
        htmlEmail.AppendLine("  grid-template-columns: repeat(4, 1fr);");
        htmlEmail.AppendLine("  gap: 1rem;");
        htmlEmail.AppendLine("}");
        htmlEmail.AppendLine("");
        htmlEmail.AppendLine(".entry {");
        htmlEmail.AppendLine("  border: 1px solid #cccccc;");
        htmlEmail.AppendLine("  padding: 1rem;");
        htmlEmail.AppendLine("}");
        htmlEmail.AppendLine("");
        htmlEmail.AppendLine(".book-header {");
        htmlEmail.AppendLine("  font-size: smaller;");
        htmlEmail.AppendLine("  font-weight: bold;");
        htmlEmail.AppendLine("}");
        htmlEmail.AppendLine("");
        htmlEmail.AppendLine(".author-list {");
        htmlEmail.AppendLine("  list-style: none;");
        htmlEmail.AppendLine("  padding: 0;");
        htmlEmail.AppendLine("}");
        htmlEmail.AppendLine("</style>");
    }

    private static void WriteHeaderHTML(ref StringBuilder htmlEmail, string ListID)
    {
        htmlEmail.AppendLine($"<h1>New changes to {ListID}!</h1>");
        htmlEmail.AppendLine("<div class=\"grid-container\">");
    }

    private static void WriteHeaderPlain(ref StringBuilder plainTextEmail, string ListID)
    {
        plainTextEmail.AppendLine($"New changes to {ListID}!");
        plainTextEmail.AppendLine();
    }

    private static void WriteBookHTML(ref StringBuilder htmlEmail, Book book, string State)
    {
        htmlEmail.AppendLine("<div class=\"entry\">");
        htmlEmail.AppendLine($"<h2>This book was {State.ToLower()}!</h2>");
        htmlEmail.AppendLine("<table>");
        htmlEmail.AppendLine("<tr>");
        htmlEmail.AppendLine("<td>");
        htmlEmail.AppendLine($"<img src=\"{book.CoverImage}\" width=\"150px\">");
        htmlEmail.AppendLine("</td>");
        htmlEmail.AppendLine("<td>");
        htmlEmail.AppendLine($"<h3 class=\"book-header\">{book.Title}</h3>");
        htmlEmail.AppendLine($"<p>ISBN: {book.Id}</p>");
        htmlEmail.AppendLine("<span>");
        htmlEmail.AppendLine("<strong>Authors</strong>");
        htmlEmail.AppendLine("<ul class=\"author-list\">");
        foreach (string author in book.Authors)
        {
            htmlEmail.AppendLine($"<li>{author}</li>");
        }
        htmlEmail.AppendLine("</ul>");
        htmlEmail.AppendLine("</span>");
        htmlEmail.AppendLine($"<p>Publication Year: {book.PublicationYear}</p>");
        htmlEmail.AppendLine($"<p>Genre: {book.Genre}</p>");
        htmlEmail.AppendLine("</td>");
        htmlEmail.AppendLine("</tr>");
        htmlEmail.AppendLine("</table>");
        htmlEmail.AppendLine("</div>");
    }
    private static void WriteBookPlain(ref StringBuilder plainTextEmail, Book book, string State)
    {
        plainTextEmail.AppendLine($"This book was {State.ToLower()}!");
        plainTextEmail.AppendLine($"ISBN: {book.Id}");
        plainTextEmail.AppendLine($"Title: {book.Title}");
        plainTextEmail.AppendLine($"Authors: {Authors(book.Authors)}");
        plainTextEmail.AppendLine($"Publication Year {book.PublicationYear}");
        plainTextEmail.AppendLine();
    }

    private static string Authors(List<string> Authors)
    {
        StringBuilder builder = new();
        foreach (string Author in Authors)
        {
            builder.Append($"{Author}, ");
        }
        return builder.ToString()[..^1];
    }

    private static void WriteFooterHTML(ref StringBuilder htmlEmail)
    {
        htmlEmail.AppendLine("<p>You can unsubscribe to emails by going to the list's subscribe page and unsubscribing.</p>");
    }

    private static void WriteFooterPlain(ref StringBuilder plainTextEmail)
    {
        plainTextEmail.AppendLine("You can unsubscribe to emails by going to the list's subscribe page and unsubscribing.");
    }
}