using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test_collects
{
    class Program
    {
        static void Main(string[] args)
        {
            {
                crazy_collects.CrazyDictionary<string, int> dict = new crazy_collects.CrazyDictionary<string, int>();
                dict.Add("1", 1);
                dict.TryGetValue("1", out var v);
                Console.WriteLine(v);

                var e = dict.GetEnumerator();
                e.MoveNext();
                Console.WriteLine($"key={e.Current.Key}, v={e.Current.Value}");
            }

            {
                crazy_collects.CrazyHashSet<string> set = new crazy_collects.CrazyHashSet<string>();
                set.Add("abc");
                Console.WriteLine(set.Contains("abc"));

                var e = set.GetEnumerator();
                e.MoveNext();
                Console.WriteLine(e.Current);
            }

            {
                crazy_collects.CrazyList<string> list = new crazy_collects.CrazyList<string>();
                list.Add("abc");
                Console.WriteLine(list.Contains("list-abc"));

                var e = list.GetEnumerator();
                e.MoveNext();
                Console.WriteLine(e.Current);
            }

        }
    }
}
