using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace test_struct
{

    class LogTestScope : IDisposable
    {
        string Tag;
        public LogTestScope(string tag)
        {
            Tag = tag;

            Console.WriteLine($"---------- {Tag} start ----------");
        }
        public void Dispose()
        {
            Console.WriteLine($"---------- {Tag} end   ----------");
        }
    }

    class MemoryWatcher
    {
        long initialMemory;
        public MemoryWatcher()
        {
            Start();
        }
        public void Start()
        {
            Thread.MemoryBarrier();
            initialMemory = System.GC.GetTotalMemory(true);
        }
        public long Stop()
        {
            Thread.MemoryBarrier();
            var finalMemory = System.GC.GetTotalMemory(true);
            var consumption = finalMemory - initialMemory;

            return consumption;
        }
    }
    class MemoryWatcherScope : IDisposable
    {
        MemoryWatcher _inside;
        public MemoryWatcherScope()
        {
            _inside = new MemoryWatcher();
        }
        public void Dispose()
        {
            var consumption = _inside.Stop();
            Console.WriteLine($"UsedMem={consumption}");
        }
    }
}
