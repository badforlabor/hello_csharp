using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test_boxing
{
    class Program
    {

        struct A
        {
            public int a;
            public float b;
            public int[] c;
        }
        class CA
        {
            public int a;
            public float b;
            public int[] c;
        }

        // 拆箱，装箱，不影响原有结构体变量
        static void Proc1(ref object obj)
        {
            var other = (A)obj;
            other.a = 2;
            other.c[0] = 2;
        }
        static void Proc1(ref A obj)
        {
            ref var other = ref obj;
            other.a = 2;
            other.c[0] = 2;
        }
        // 拆箱，装箱，会影响类
        static void Proc2(ref object obj)
        {
            var other = (CA)obj;
            other.a = 2;
            other.c[0] = 2;
        }

        static void Main(string[] args)
        {
            // 测试拆箱装箱
            A a;
            a.a = 1;
            a.b = 2.2f;
            a.c = new int[2];
            a.c[0] = 1;

            object o = a;
            Proc1(ref o);
            A b = (A)o;
            Debug.Assert(b.a == 1);
            Debug.Assert(b.c[0] == 2);
            Debug.Assert(a.a == 1);
            Debug.Assert(a.c[0] == 2);

            Console.WriteLine($"{b.a}, {b.c[0]}");
            Console.WriteLine($"{a.a}, {a.c[0]}");


            Proc1(ref a);
            Debug.Assert(a.a == 2);
            Debug.Assert(a.c[0] == 2);
            Console.WriteLine($"{a.a}, {a.c[0]}");

            CA ca = new CA();
            ca.a = 1;
            ca.b = 2.2f;
            ca.c = new int[2];
            ca.c[0] = 1;
            o = ca;
            Proc2(ref o);
            Debug.Assert(ca.a == 2);
        }
    }
}
