using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static System.Console;

namespace test_ast
{
    class Program
    {
        static void Main(string[] args)
        {
            GenCode.Main1();
            Test4.Main1();

            // zhushi1
            int a = 0;
            // zhushi2
            var b = false;
            // zhushi3
            const float c = 1.1f;
            /*
             * zhushi4
             */
            Console.WriteLine($"{a}, {b}, {c}");
            Test1.Main1(args); // zhushi5
            Test2.Main1();// zhushi6
            Test3.Main1();

            TestRoslynquoter.Main1();
        }
    }
}
