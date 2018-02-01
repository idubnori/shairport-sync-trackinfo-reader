using System;
using System.Reactive.Linq;
using System.Text;

namespace ShairportSync.Metadata.Utilities
{
    internal static class UtilityExtensions
    {
        public static IObservable<Tuple<TSource, TSource>> PairWithPrevious<TSource>(this IObservable<TSource> source)
        {
            return source.Scan(
                Tuple.Create(default(TSource), default(TSource)),
                (acc, current) => Tuple.Create(acc.Item2, current));
        }

        public static string HexStringToString(this string hexString)
        {
            if (hexString == null || (hexString.Length & 1) == 1) throw new ArgumentException();

            var sb = new StringBuilder();
            for (var i = 0; i < hexString.Length; i += 2)
            {
                var hexChar = hexString.Substring(i, 2);
                sb.Append((char) Convert.ToByte(hexChar, 16));
            }
            return sb.ToString();
        }
    }
}