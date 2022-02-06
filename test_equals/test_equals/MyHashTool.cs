using System;
using System.Collections.Generic;

namespace libre_hash
{
    public class MyHashTool
    {
        // 计算一堆对象的hash
        public static int CombineHash(params object[] objs)
        {
            if (objs == null || objs.Length == 0)
            {
                return 0;
            }

            var vals = new int[objs.Length];
            for(int i=0; i<objs.Length; i++)
            {
                vals[i] = GetHashCode(objs[i]);
            }
            return CalcHash(vals);
        }

        // 计算各种类型的hashcode
        public static int GetHashCode(object obj)
        {
            if (obj == null)
            {
                return 0;
            }

            if (obj is string)
            {
                return (obj as string).GetHashCode();
            }

            // Dict
            if (obj is System.Collections.IDictionary)
            {
                var d = obj as System.Collections.IDictionary;
                return CalcHash(GetHashCode(d.Keys), GetHashCode(d.Values));
            }

            // 列表
            if (obj is System.Collections.IEnumerable)
            {
                var e = obj as System.Collections.IEnumerable;
                var vals = new List<int>();
                foreach (var it in e)
                {
                    var h = GetHashCode(it);
                    vals.Add(h);
                }
                return CalcHash(vals.ToArray());
            }

            return obj.GetHashCode();
        }

        // 各种类型的Equals
        public new static bool Equals(object obj1, object obj2)
        {
            if (obj1 == null && obj2 == null)
            {
                return true;
            }

            if (obj1 == null)
            {
                return false;
            }

            if (obj2 == null)
            {
                return false;
            }

            if (obj1.GetType() != obj2.GetType())
            {
                return false;
            }


            if (obj1 is System.Collections.IDictionary && obj2 is System.Collections.IDictionary)
            {
                var d1 = obj1 as System.Collections.IDictionary;
                var d2 = obj2 as System.Collections.IDictionary;

                return Equals(d1.Keys, d2.Keys)
                    && Equals(d1.Values, d2.Values);
            }

                // 列表
            if (obj1 is System.Collections.IEnumerable && obj2 is System.Collections.IEnumerable)
            {
                System.Collections.IList aList = new List<object>();
                System.Collections.IList bList = new List<object>();
                if (obj1 is System.Collections.IList && obj2 is System.Collections.IList)
                {
                    aList = obj1 as System.Collections.IList;
                    bList = obj2 as System.Collections.IList;
                }
                else
                {
                    foreach (var it in obj1 as System.Collections.IEnumerable)
                    {
                        aList.Add(it);
                    }
                    foreach (var it in obj2 as System.Collections.IEnumerable)
                    {
                        bList.Add(it);
                    }
                }

                if (aList.Count != bList.Count)
                {
                    return false;
                }

                for (int i = 0; i < aList.Count; i++)
                {
                    if (!Equals(aList[i], bList[i]))
                    {
                        return false;
                    }
                }

                return true;
            }

            return obj1.Equals(obj2);
        }

        // 计算hash-vals的结果
        public static int CalcHash(params int[] vals)
        {
            return CalcHash(17, 31, vals);
        }

        public static int CalcHash(int seed, int factor, params int[] vals)
        {
            unchecked
            {
                int hash = seed;
                foreach (int i in vals)
                {
                    hash = (hash * factor) + i;
                }
                return hash;
            }
        }
    }
}
