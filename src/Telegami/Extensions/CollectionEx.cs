namespace Telegami.Extensions;

internal static class CollectionEx
{
    /// <summary>
    /// Add item to array copy
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public static T[] Add<T>(this T[] array, T item)
    {
        var newArray = new T[array.Length + 1];

        Array.Copy(array, newArray, array.Length);
        newArray[^1] = item;

        return newArray;
    }

    /// <summary>
    /// Add item to new readonly copy
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection"></param>
    /// <param name="item"></param>
    /// <returns></returns>
    public static IReadOnlyCollection<T> Add<T>(this IReadOnlyCollection<T> collection, T item)
    {
        if (collection is T[] array)
        {
            return array.Add(item);
        }

        var newArray = new T[collection.Count + 1];

        var i = 0;
        foreach (var cItem in collection)
        {
            newArray[i] = cItem;
            i++;
        }

        newArray[^1] = item;

        return newArray;
    }
}