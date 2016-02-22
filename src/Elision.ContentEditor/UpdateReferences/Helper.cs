using Sitecore.Data;
using Sitecore.Data.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Elision.ContentEditor.UpdateReferences
{
    static class Helper
    {
        public static string Replace(this string source, string oldValue, string newValue, bool ignoreCase)
        {
            if (String.IsNullOrEmpty(source))
                return source;

            if (!ignoreCase)
                return source.Replace(oldValue, newValue);

            oldValue = oldValue.Replace("[", @"\[").Replace("]", @"\]");
            return new Regex(oldValue, RegexOptions.IgnoreCase)
                .Replace(source, newValue);
        }

        public static StringBuilder Replace(this StringBuilder source, string oldValue, string newValue, bool ignoreCase)
        {
            return new StringBuilder(Replace(source.ToString(), oldValue, newValue, ignoreCase));
        }

        public static bool OrAny<T>(this IEnumerable<T> list)
        {
            var arrayList = list as T[] ?? list.ToArray();
            if (list != null && arrayList.Any())
                return arrayList.Any();

            return false;
        }

        public static Item GetItem(this ID id, Database db)
        {
            return id.OrEmpty() != string.Empty
                       ? db.GetItem(id)
                       : null;
        }

        public static ID OrID(this object s)
        {
            if (s.OrEmpty() != string.Empty)
            {
                if (ShortID.IsShortID(s.ToString()))
                    return (new ShortID(s.ToString())).ToID();

                if (ID.IsID(s.ToString()))
                    return (new ID(s.ToString()));
            }

            return ID.Null;
        }

        public static string OrEmpty(this object s)
        {
            return s != null
                       ? s.ToString()
                       : string.Empty;
        }
    }
}
