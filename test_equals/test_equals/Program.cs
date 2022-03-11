using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using libre_hash;

namespace test_equals
{

    public class CA
    {
        public int A;
        public float B;
        public long C;
        public ulong D;
        public byte E;
        public string F;

        public static CA New()
        {
            return new CA() { A=1, B=2.2f, C=3, D=4, E=5, F = "F" };
        }

        public override bool Equals(object obj)
        {
            var other = obj as CA;

            return A.Equals(other.A)
                && B.Equals(other.B)
                && C.Equals(other.C)
                && D.Equals(other.D)
                && E.Equals(other.E)
                && F.Equals(other.F);
        }

        public override int GetHashCode()
        {
            return MyHashTool.CombineHash(A, B, C, D, E, F);            
        }
    }

    public struct SA
    {
        public int A;
        public float B;
        public long C;
        public ulong D;
        public byte E;
        public string F;


        public static SA New()
        {
            return new SA() { A = 1, B = 2.2f, C = 3, D = 4, E = 5, F = "F" };
        }

        // 纯结构体，不用写equals
        //public override bool Equals(object obj)
        //{
        //    SA other = (SA)obj;

        //    return A.Equals(other.A)
        //        && B.Equals(other.B)
        //        && C.Equals(other.C)
        //        && D.Equals(other.D)
        //        && E.Equals(other.E)
        //        && F.Equals(other.F);
        //}

        // 但是需要写GetHashCode？
        public override int GetHashCode()
        {
            return MyHashTool.CombineHash(A, B, C, D, E, F);
        }
    }
    public struct SB : IEquatable<SB>
    {
        public int A;
        public float B;
        public long C;
        public ulong D;
        public byte E;
        public string F;


        public static SB New()
        {
            return new SB() { A = 1, B = 2.2f, C = 3, D = 4, E = 5, F = "F" };
        }

        public override bool Equals(object obj)
        {
            SB other = (SB)obj;

            return A.Equals(other.A)
                && B.Equals(other.B)
                && C.Equals(other.C)
                && D.Equals(other.D)
                && E.Equals(other.E)
                && F.Equals(other.F);
        }

        public override int GetHashCode()
        {
            return MyHashTool.CombineHash(A, B, C, D, E, F);
        }

        public bool Equals(SB other)
        {
            return A.Equals(other.A)
                && B.Equals(other.B)
                && C.Equals(other.C)
                && D.Equals(other.D)
                && E.Equals(other.E)
                && F.Equals(other.F);
        }
    }

    public class CB
    {
        public CA A;
        public SA B;

        public List<CA> C;
        public List<SA> D;

        public Dictionary<string, CA> E;
        public Dictionary<string, SA> F;

        public static CB New()
        {
            return new CB()
            {
                A = CA.New(),
                B = SA.New(),
                C = new List<CA>() { CA.New(), CA.New() },
                D = new List<SA>() { SA.New(), SA.New() },
                E = new Dictionary<string, CA>() { {"A", CA.New() }, { "B", CA.New() }, },
                F = new Dictionary<string, SA>() { { "sA", SA.New() }, { "sB", SA.New() }, },
            };
        }

        public override bool Equals(object obj)
        {
            CB other = (CB)obj;

            return A.Equals(other.A)
                && B.Equals(other.B)
                && MyHashTool.Equals(C, other.C)
                && MyHashTool.Equals(D, other.D)
                && MyHashTool.Equals(E, other.E)
                && MyHashTool.Equals(F, other.F);
        }

        public override int GetHashCode()
        {
            return MyHashTool.CombineHash(A, B, C, D, E, F);
        }
    }


    class Program
    {
        static void Test1()
        {
            var ca = CA.New();
            var caHash = ca.GetHashCode();
            Console.WriteLine($"{caHash}");

            var cb = CA.New();
            Debug.Assert(cb.GetHashCode() == ca.GetHashCode());
            Debug.Assert(cb.Equals(ca));

            var cc = CA.New();
            cc.F = "FF";
            Debug.Assert(cc.GetHashCode() != ca.GetHashCode());
            Debug.Assert(!cc.Equals(ca));

            var cd = CA.New();
            cd.B = 2;
            Debug.Assert(cd.GetHashCode() != ca.GetHashCode());
            Debug.Assert(!cd.Equals(ca));
        }
        static void Test2()
        {
            var ca = SA.New();
            var caHash = ca.GetHashCode();
            Console.WriteLine($"{caHash}");

            var cb = SA.New();
            Debug.Assert(cb.GetHashCode() == ca.GetHashCode());
            Debug.Assert(cb.Equals(ca));

            var cc = SA.New();
            cc.F = "FF";
            Debug.Assert(cc.GetHashCode() != ca.GetHashCode());
            Debug.Assert(!cc.Equals(ca));

            var cd = SA.New();
            cd.B = 2;
            Debug.Assert(cd.GetHashCode() != ca.GetHashCode());
            Debug.Assert(!cd.Equals(ca));

            var ce = SB.New();
            var cf = SB.New();
            Debug.Assert(ce.Equals(cf));    // 调用的是：public bool Equals(SB other)
            var cg = (object)ce;
            Debug.Assert(ce.Equals(cg));    // 调用的是：public override bool Equals(object obj)
        }
        static void Test3()
        {
            var ca = CB.New();
            var caHash = ca.GetHashCode();
            Console.WriteLine($"{caHash}");

            var cb = CB.New();
            Debug.Assert(cb.GetHashCode() == ca.GetHashCode());
            Debug.Assert(cb.Equals(ca));
        }


        static void Main(string[] args)
        {
            Test1();
            Test2();
            Test3();
        }

    }
}
