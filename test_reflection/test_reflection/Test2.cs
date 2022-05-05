using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test_reflection
{
    class Test2
    {
        class CA
        {
            static CA()
            {
                Console.WriteLine("static Constructor CA");
            }
        }

        class CB
        {
            static CB()
            {
                Console.WriteLine("static Constructor CB");
            }
        }

        class CC
        {
            public static int A = 5;
            public static void ShowMe()
            {

            }
            static CC()
            {
                Console.WriteLine("static Constructor CC");
            }
        }


        public static void Test()
        {
            var a = new CA();

            var b = System.Activator.CreateInstance<CB>();

            CC.ShowMe();
        }
    }
}
