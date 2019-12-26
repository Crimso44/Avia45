using Aik2.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Aik2
{
    public class Util
    {
        public class Pair<T>
        {
            public T Id;
            public string Name;
            public override string ToString()
            {
                return Name;
            }
        }

        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public static string PadLeft(string s, char c, int n)
        {
            var res = string.IsNullOrEmpty(s) ? "" : s;
            while (res.Length < n) res = c + res;
            return res;
        }

        public static string GetPropValue(object src, string propName)
        {
            var o = src.GetType().GetProperty(propName).GetValue(src, null);
            if (o is null) return "";
            if (o is string) return ((string)o).Trim();
            else if (o is int) return ((int)o).ToString();
            else if (o is int?) return ((int?)o).ToString();
            else
            {
                MessageBox.Show($"Unknown type {propName}: {o}");
                return "";
            }
        }
    }

    public static class Extensions
    {
        public static void InsertCraftSorted(this List<CraftDto> source, CraftDto element)
        {
            int index = source.FindLastIndex(e => string.Compare(e.Sort, element.Sort) < 0);
            if (index == -1)
            {
                source.Insert(0, element);
                return;
            }
            source.Insert(index + 1, element);
        }

        public static void InsertPairSorted(this List<Util.Pair<int>> source,
                Util.Pair<int> element)
        {
            int index = source.FindLastIndex(e => string.Compare(e.Name, element.Name) < 0);
            if (index == -1)
            {
                source.Insert(0, element);
                return;
            }
            source.Insert(index + 1, element);
        }

        public static int InsertStringSorted(this List<string> source,
                string element)
        {
            int index = source.FindLastIndex(e => string.Compare(e, element) < 0);
            if (index == -1)
            {
                source.Insert(0, element);
                return 0;
            }
            source.Insert(index + 1, element);
            return index + 1;
        }

        public static void Move<T>(this List<T> source, int oldIndex, int newIndex) 
        {
            T item = source[oldIndex];
            source.RemoveAt(oldIndex);
            source.Insert(newIndex, item);
        }

        public static int BinaryIndexOf(this List<string> source, string element)
        {
            var ind = (source.Count + 1) / 2;
            var step = ind;
            if (ind >= source.Count) return -1;
            while (true) {
                if (source[ind] == element) return ind;
                if (step == 1) return -1;
                step = (step + 1) / 2;
                if (string.Compare(source[ind], element) < 0)
                {
                    ind += step;
                    if (ind >= source.Count) ind = source.Count - 1;
                } else
                {
                    ind -= step;
                    if (ind < 0) ind = 0;
                };
            };
        }


    }

}
