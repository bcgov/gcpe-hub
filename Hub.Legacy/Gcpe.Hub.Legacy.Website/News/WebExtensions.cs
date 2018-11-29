using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Gcpe.Hub.News
{
    public static class WebExtensions
    {
        private static string EscapeString(string value)
        {
            return value.Replace(@"\", @"\\").Replace("'", @"\'");
        }

        public static string ToJavaScript<T>(this IEnumerable<T> enumerable)
        {
            return string.Join(",", enumerable.Select(e => String.Format("'{0}'", EscapeString(e.ToString()))));
        }

        public static string ToJavaScript<T, U>(this Dictionary<T, U> enumerable)
        {
            return string.Join(",", enumerable.Select(e => String.Format("'{0}':'{1}'", EscapeString(e.Key.ToString()), EscapeString(e.Value.ToString()))));
        }

        public static string ToJavaScript<T, U>(this Dictionary<T, IEnumerable<U>> enumerable)
        {
            return string.Join(",", enumerable.Select(e => String.Format("'{0}':[ {1} ]", EscapeString(e.Key.ToString()), e.Value.ToJavaScript())));
        }
    }
}