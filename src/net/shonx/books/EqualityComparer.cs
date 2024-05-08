namespace net.shonx.books;

public class EqualityComparer : IEqualityComparer<string>
{
    private static readonly EqualityComparer equalityComparer = new();

    public bool Equals(string? x, string? y)
    {
        if (string.IsNullOrEmpty(x))
        {
            if (string.IsNullOrEmpty(y))
            {
                return true;
            }
            return false;
        }
        return x.Equals(y);
    }

    public int GetHashCode(string obj)
    {
        return obj.GetHashCode();
    }

    public static bool ListsEqual(List<string> x, List<string> y)
    {
        return x.SequenceEqual(y, equalityComparer);
    }
}