using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aik2
{
    public class Util
    {
        public class Pair<T>
        {
            public T Id;
            public string Name;
        }

        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public static string PadLeft(string s, char c, int n)
        {
            var res = s;
            while (res.Length < n) res = c + res;
            return res;
        }
    }

    public static class Extensions
    {
        public static void InsertCraftSorted(this List<Util.Pair<int>> source,
                Util.Pair<int> element)
        {
            int index = source.FindLastIndex(e => string.Compare(e.Name, element.Name) < 0);
            if (index == 0 || index == -1)
            {
                source.Insert(0, element);
                return;
            }
            source.Insert(index + 1, element);
        }
    }

}
