using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Reflection;

namespace test_reflection
{
    public class CA
    {
        public int A;
    }
    public struct SA
    {
        public int A;
        public float B;
        public string C;
        public List<int> D;
        public Dictionary<int, float> E;
        public int[] F;
    }
    public struct SB
    {
        public SA Base;
        public CA B;

        public float BB;
        public string BC;
    }
    public struct SC
    {
        public SB Base;
        public CA B;

        public float CB;
        public string CC;
    }

    class PoolArray<T>
    { }


    class Program
    {
        static void Main(string[] args)
        {

            SC c = new SC();
            c.B = new CA();
            c.Base.B = new CA();
            c.Base.Base.C = "";
            c.Base.Base.D = new List<int>();
            c.Base.Base.E = new Dictionary<int, float>();
            c.Base.Base.F = new int[1];



            System.Diagnostics.Debug.Assert(c.B != null);
            System.Diagnostics.Debug.Assert(c.Base.B != null);
            System.Diagnostics.Debug.Assert(c.Base.Base.A == 0);
            System.Diagnostics.Debug.Assert(c.Base.Base.B == 0);
            System.Diagnostics.Debug.Assert(string.IsNullOrEmpty(c.Base.Base.C));
            System.Diagnostics.Debug.Assert(c.Base.Base.D != null);
            System.Diagnostics.Debug.Assert(c.Base.Base.E != null);
            System.Diagnostics.Debug.Assert(c.Base.Base.F != null);

            LoopSetValue(ref c);

            System.Diagnostics.Debug.Assert(c.B == null);
            System.Diagnostics.Debug.Assert(c.Base.B == null);
            System.Diagnostics.Debug.Assert(c.Base.Base.A > 0);
            System.Diagnostics.Debug.Assert(c.Base.Base.B > 0);
            System.Diagnostics.Debug.Assert(!string.IsNullOrEmpty(c.Base.Base.C));
            System.Diagnostics.Debug.Assert(c.Base.Base.D == null);
            System.Diagnostics.Debug.Assert(c.Base.Base.E == null);
            System.Diagnostics.Debug.Assert(c.Base.Base.F == null);

        }

        static void LoopSetValue<T>(ref T selfRef)
        {
            var t = selfRef.GetType();
            var members = t.GetFields();
            object obj = selfRef;
            LoopSetValue(ref obj, members);
            selfRef = (T)obj;
        }

        static void LoopSetValue(ref object selfRef, FieldInfo[] members)
        {
            foreach (var m in members)
            {                
                if (IsSubclassOfRawGeneric(typeof(PoolArray<>), m.FieldType))
                {
                    // 特殊类型
                }
                else if (m.FieldType == typeof(string))
                {
                    // string
                    m.SetValue(selfRef, "string value");
                }
                else if (m.FieldType.IsArray)
                {
                    // array
                    m.SetValue(selfRef, null);
                }
                else if (HasInterface(typeof(ICollection<>), m.FieldType))
                {
                    // list, dict
                    m.SetValue(selfRef, null);
                }
                else if (m.FieldType.IsClass)
                {
                    // class
                    m.SetValue(selfRef, null);
                }
                else if (m.FieldType.IsValueType)
                {
                    if (m.FieldType.IsPrimitive)
                    {
                        // int, float等值类型
                        var whiteList = new List<System.Type>() { typeof(int), typeof(float), typeof(bool) };
                        if (!whiteList.Contains(m.FieldType))
                        {
                            MyDebug.Assert(false, $"unknown t={m.FieldType.Name}");
                        }

                        m.SetValue(selfRef, 1);
                    }
                    else
                    {
                        // struct
                        var nested = m.FieldType.GetFields();
                        var target = m.GetValue(selfRef);
                        LoopSetValue(ref target, nested);
                        // 重点！值类型，必须得设置一下
                        m.SetValue(selfRef, target);
                    }
                }
                else
                {
                    // 其他非法
                    MyDebug.Assert(false, "");
                }
            }
        }

        static bool IsSubclassOfRawGeneric(Type generic, Type toCheck)
        {
            while (toCheck != null && toCheck != typeof(object))
            {
                var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
                if (generic == cur)
                {
                    return true;
                }
                toCheck = toCheck.BaseType;
            }
            return false;
        }

        static bool HasInterface(Type generic, Type myType)
        {
            // this conditional is necessary if myType can be an interface,
            // because an interface doesn't implement itself: for example,
            // typeof (IList<int>).GetInterfaces () does not contain IList<int>!
            if (myType.IsInterface && myType.IsGenericType &&
                myType.GetGenericTypeDefinition() == generic)
                return true;

            foreach (var i in myType.GetInterfaces())
                if (i.IsGenericType && i.GetGenericTypeDefinition() == generic)
                    return true;

            return false;
        }

    }

    class MyDebug
    {
        public static void Assert(bool b, string msg,
            [CallerFilePath] string file = "",
            [CallerMemberName] string member = "",
            [CallerLineNumber] int line = 0)
        {
            if (!b)
            {
                Debug.Assert(false);
            }
        }

    }
}
