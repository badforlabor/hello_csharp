using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test_data
{
    class Program
    {
        static void Main(string[] args)
        {
            var Dict = new Dictionary<int, string>() { {1,"1"}, { 2, "2" }, { 3, "3" }, { 4, "4" }, { 5, "5" }};

            var list = new List<int>(2);            
            foreach (var it in Dict)
            {
                if (it.Key == 3 || it.Key == 4)
                {
                    list.Add(it.Key);
                }
            }
            foreach (var it in list)
            {
                Dict.Remove(it);
            }
            Debug.Assert(Dict.Count == 3);
        }
    }
}
