using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test_enum
{
    class MyEnumTools
    {
        public static int ToIdx<T>(T t)
        {
            return EnumTools<T>.GetIdx(t);
        }


        class EnumTools<T>
        {
            private static Dictionary<T, int> EnumDict;
            static EnumTools()
            {
                var values = Enum.GetValues(typeof(T));
                EnumDict = new Dictionary<T, int>();
                for (int i = 0; i < values.Length; i++)
                {
                    EnumDict.Add((T)values.GetValue(i), i);
                }
            }

            public static int GetIdx(T t)
            {
                int ret = -1;

                if (EnumDict.TryGetValue(t, out ret))
                {
                    return ret;
                }

                //MyDebug.Assert(false, $"invalid {t}");
                return -1;
            }
        }
    }

}
