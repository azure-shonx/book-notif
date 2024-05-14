namespace net.shonx.books.functions;

using Azure.Messaging.EventGrid;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

public class CosmosWatcher
{
    [Function("CosmosWatcher")]
    [EventGridOutput(TopicEndpointUri = "TOPIC_ENDPOINT", TopicKeySetting = "TOPIC_KEY")]
    public static List<EventGridEvent> Run([CosmosDBTrigger(
        databaseName: "bookclub",
        containerName: "lists",
        Connection = "CosmosDB",
        LeaseConnection = "CosmosDB",
        LeaseContainerName = "leases",
        CreateLeaseContainerIfNotExists = true)] IReadOnlyList<List> updatedLists, FunctionContext context)
    {
        List<EventGridEvent> List = [];
        ILogger<CosmosWatcher>? _logger = context.GetLogger<CosmosWatcher>() ?? throw new NullReferenceException("_logger");
        try
        {
            foreach (List document in updatedLists)
            {
                if (document.Subscribers.Count == 0)
                    continue;
                var Diffs = Differences(document.BookISBNs, document.PreviousBookISBNs);
                if (Diffs.Count == 0)
                    continue;
                EventGridEvent eventGridEvent = new(
                    subject: $"/cosmosdb/books/databases/bookclub/containers/lists/documents/{document.Id}",
                    eventType: "ListUpdated",
                    dataVersion: "1.0",
                    data: new BinaryData(new Event(document.Id, Diffs, document.Subscribers))
                );
                List.Add(eventGridEvent);
            }
            return List;
        }
        catch (Exception e)
        {
            _logger.LogCritical(e.ToString());
            return [];
        }
    }

    public static Dictionary<string, string> Differences(List<string> newList, List<string> oldList)
    {
        Dictionary<string, string> Results = [];
        foreach (string ISBN in newList)
        {
            if (oldList.Contains(ISBN))
                continue;
            Results.Add(ISBN, State.ADDED);
        }

        foreach (string ISBN in oldList)
        {
            if (newList.Contains(ISBN))
                continue;
            Results.Add(ISBN, State.REMOVED);
        }
        return Results;
    }
}