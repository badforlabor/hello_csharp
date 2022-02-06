using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test_struct
{
    struct StructArray<T> : ICloneable where T : struct, ICloneable
    {
        public T[] Value;
        public object Clone()
        {
            var ret = new StructArray<T>();
            ret.Value = new T[Value.Length];
            for (int i = 0; i < Value.Length; i++)
            {
                ret.Value[i] = (T)Value[i].Clone();
            }
            return ret;
        }
        public StructArray<T> TClone()
        {
            return (StructArray<T>) Clone();
        }
        public StructArray(int s)
        {
            Value = new T[s];
        }
        public int Length => Value.Length;

        public ref T GetRef(int idx)
        {
            return ref Value[idx];
        }

        public T this[int index]
        {
            get
            {
                return Value[index];
            }
            set
            {
                Value[index] = value;
            }
        }
    }

}
