using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test_struct
{
    class StructRef
    {
        struct Base
        {
            public int v1;
        }
        struct A
        {
            public Base a1;
            public int a2;
            public float a3;
            public List<int> a4;
        }

        class B
        {
            A b1 = new A();

            public A Get()
            {
                return b1;
            }
            public ref A GetRef()
            {
                return ref b1;
            }
            public ref Base GetBaseRef()
            {
                return ref GetRef().a1;
            }
        };

        public static void Test1()
        {
            var b = new B();
            ref var b1 = ref b.GetRef();
            b1.a1.v1 = 1;
            b1.a2 = 2;

            b.GetRef().a3 = 3;
            b.GetRef().a4 = new List<int>();

            b.GetRef().a4.Add(1);
            b.Get().a4.Add(2);

            ref var b2 = ref b.GetBaseRef();
            b2.v1 = 11;
            Console.WriteLine($"a1={b.Get().a1.v1}, a2={b.Get().a2}, a3={b.Get().a3}");

            System.Diagnostics.Debug.Assert(b.Get().a1.v1 == 11);
            System.Diagnostics.Debug.Assert(b.Get().a2 == 2);
            System.Diagnostics.Debug.Assert(b.Get().a3 == 3);
            System.Diagnostics.Debug.Assert(b.Get().a4.Count == 2);
            System.Diagnostics.Debug.Assert(b.Get().a4[0] == 1);
            System.Diagnostics.Debug.Assert(b.Get().a4[1] == 2);
        }
    }
}
