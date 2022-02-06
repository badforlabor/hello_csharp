using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test_dispose
{
    class ClassA : IDisposable
    {
        private string step;
        public ClassA(string Step)
        {
            step = Step;
        }

        public void Dispose()
        {
            Console.WriteLine($"ClassA Dispose:{step}");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Test....");

            using (new ClassA("1"))
            using (new ClassA("2"))
            {
                Console.WriteLine("A1");
            }

            Console.WriteLine("End...");
            Console.Write("按任意键退出...");
            Console.ReadKey(true);
        }
    }
}
