using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace crazy
{
    public static class CrazyCollectorExtensions
    {
        public static void ForEach<T>(this IEnumerable<T> sequence, Action<int, T> action)
        {
            // argument null checking omitted
            int i = 0;
            foreach (T item in sequence)
            {
                action(i, item);
                i++;
            }
        }
    }
}
