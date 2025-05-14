namespace ProbabilityTrades.Common.Extensions;

public static class LinqToSqlExtensions
{
    public static IQueryable<TSource> TakeIf<TSource>(this IQueryable<TSource> source, int count)
    {
        return count > 0 ? source.Take(count) : source;
    }
}
