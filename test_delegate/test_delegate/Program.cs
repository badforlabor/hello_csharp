using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test_delegate
{
    class Program
    {
        static void Main(string[] args)
        {
            var test1 = new Test1();
            test1.evt += Test1_evt;

            test1.Call();
        }

        private static void Test1_evt(int obj)
        {

        }
    }
    class Test1
    {
        public event System.Action<int> evt;

        public void Call()
        {
            evt?.Invoke(1);
        }
    }
}
