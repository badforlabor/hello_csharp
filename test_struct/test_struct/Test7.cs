using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test_struct
{
    class Test7
    {
        struct SA
        {
            public int A;
            public float B;

            public static void DoClone(SA src, ref SA dst)
            {
                dst = src;
            }
        }

        public static void Test()
        {
            var a = new SA();
            a.A = 1;
            a.B = 2.2f;

            SA b = new SA();
            SA.DoClone(a, ref b);

            System.Diagnostics.Debug.Assert(b.A == 1);
            System.Diagnostics.Debug.Assert(b.B == 2.2f);
        }
    }
}
