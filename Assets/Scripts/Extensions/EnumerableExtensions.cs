using System;
using System.Collections.Generic;
using System.Linq;

namespace Extensions
{
  public static class EnumerableExtensions
  {
    private static readonly Random _random = new();

    public static TItem Random<TItem>(this TItem[] array, Random random = null) =>
      array.Length == 0 ? default : array[(random ?? _random).Next(array.Length)];

    public static TItem Random<TItem>(this IEnumerable<TItem> enumerable, Random random = null) =>
      Random(enumerable.ToArray(), random);
  }
}