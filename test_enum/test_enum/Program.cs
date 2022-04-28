using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test_enum
{
    class Program
    {
        enum EA
        {
            None = 0,
            A = 1,
            B = 2,
            Max = 100,
        }
        enum EB
        {
            None = 0,
            A = 10,
            B = 20,
            Max = 100,
        }

        static void Main(string[] args)
        {
            System.Diagnostics.Debug.Assert(MyEnumTools.ToIdx(EA.None) == 0);
            System.Diagnostics.Debug.Assert(MyEnumTools.ToIdx(EA.A) == 1);
            System.Diagnostics.Debug.Assert(MyEnumTools.ToIdx(EA.B) == 2);
            System.Diagnostics.Debug.Assert(MyEnumTools.ToIdx((EA)3) == -1);
            System.Diagnostics.Debug.Assert(MyEnumTools.ToIdx(EA.Max) == 3);


            System.Diagnostics.Debug.Assert(MyEnumTools.ToIdx(EB.None) == 0);
            System.Diagnostics.Debug.Assert(MyEnumTools.ToIdx(EB.A) == 1);
            System.Diagnostics.Debug.Assert(MyEnumTools.ToIdx(EB.B) == 2);
            System.Diagnostics.Debug.Assert(MyEnumTools.ToIdx(EB.Max) == 3);
        }
    }
}
