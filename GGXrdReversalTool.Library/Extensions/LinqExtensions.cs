namespace GGXrdReversalTool.Library.Extensions;

public static class LinqExtensions
{
    public static int FirstIndexOf<TSource>(this IEnumerable<TSource> source, Func<TSource, bool> predicate)
    {
        if (source == null)
        {
            throw new ArgumentException(null, nameof(source));
        }

        if (predicate == null)
        {
            throw new ArgumentException(null, nameof(predicate));
        }

        var elements = source.Select((item, index) => new { item, index }).Where(x => predicate(x.item)).Select(x=> x.index).ToList();

        if (elements.Any())
        {
            return elements.First();
        }

        return -1;
    }

    public static IEnumerable<IEnumerable<T>> GroupWhile<T>(this IEnumerable<T> source, Func<T,T,bool> condition)
    {
        var seq = source.ToList();

        if (!seq.Any())
        {
            yield break;
        }
        
        T prev = seq.First();

        List<T> list = new List<T>() { prev };

        foreach (var item in seq.Skip(1))
        {
            if (!condition(prev,item))
            {
                yield return list;

                list = new List<T>();
            }
            
            list.Add(item);

            prev = item;
        }

        yield return list;
    }
}