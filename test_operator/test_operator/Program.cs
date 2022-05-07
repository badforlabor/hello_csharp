using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test_operator
{
    public struct OpSample
    {
        public int Val;
        
        public OpSample(int val)
        {
            this.Val = val;
        }

        public static OpSample operator --(OpSample s)
        {
            s.Val = --s.Val;
            return s;
        }

        //Overload unary increment operator
        public static OpSample operator ++(OpSample s)
        {
            s.Val = ++s.Val;
            return s;
        }

        public void PrintValues()
        {
            Console.WriteLine("Values of val: " + Val);
            Console.WriteLine();
        }
    }

    class Program
    {




        static void Main(string[] args)
        {
            OpSample s = new OpSample();

            s++;
            s++;
            s++;
            System.Diagnostics.Debug.Assert(s.Val == 3);

            s--;
            s--;
            s--;
            System.Diagnostics.Debug.Assert(s.Val == 0);
        }
    }
}
