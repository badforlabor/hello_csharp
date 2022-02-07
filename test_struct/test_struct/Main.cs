using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace test_struct
{
    class CA
    {
        public int[] A;
        public float[] B;
    }
    // 引用拷贝
    struct SA
    {
        public int[] A;
        public float[] B;
    }

    // 值拷贝
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    struct SB
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public int[] A;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 100)]
        public float[] B;

        public static SB New()
        {
            return (SB)StructBytes.NewStruct(typeof(SB));
        }
    }

    // 定义拷贝行为
    struct SC : ICloneable
    {
        public int[] A;
        public float[] B;

        public static SC New()
        {
            SC c = new SC();
            c.A = new int[100];
            c.B = new float[100];
            Debug.Assert(Marshal.SizeOf(c) == (sizeof(int) + sizeof(float)));
            return c;
        }

        public object Clone()
        {
            SC c = new SC();

            c.A = new int[A.Length];
            Array.Copy(A, c.A, A.Length);

            c.B = new float[B.Length];
            Array.Copy(B, c.B, B.Length);

            return c;
        }
    }
    // 定义拷贝行为
    class CC : ICloneable
    {
        public int[] A;
        public float[] B;

        public static CC New()
        {
            CC c = new CC();
            c.A = new int[100];
            c.B = new float[100];
            return c;
        }

        public object Clone()
        {
            CC c = new CC();

            c.A = new int[A.Length];
            for (int i = 0; i < A.Length; i++)
            {
                c.A[i] = A[i];
            }
            //Array.Copy(A, c.A, A.Length);

            c.B = new float[B.Length];
            for (int i = 0; i < B.Length; i++)
            {
                c.B[i] = B[i];
            }
            //Array.Copy(B, c.B, B.Length);

            return c;
        }
    }


    // 定义拷贝行为，嵌套
    struct SD : ICloneable
    {
        public int[] A;
        public float[] B;
        public StructArray<SC> C;
        public long[] D;

        public static SD New()
        {
            var ret = new SD();
            ret.A = new int[100];
            ret.B = new float[100];
            ret.C = new StructArray<SC>(2);

            for (int i = 0; i < ret.C.Length; i++)
            {
                ret.C.GetRef(i).A = new int[100];
                ret.C.GetRef(i).B = new float[100];
            }

            ret.D = new long[100];
            int size = Marshal.SizeOf(ret);
            int size2 = Marshal.SizeOf(ret.C);
            int size3 = sizeof(long);
            Debug.Assert(Marshal.SizeOf(ret) == (sizeof(int) + sizeof(float) + sizeof(long)));
            return ret;
        }

        public object Clone()
        {
            var ret = new SD();

            ret.A = new int[A.Length];
            Array.Copy(A, ret.A, A.Length);

            ret.B = new float[B.Length];
            Array.Copy(B, ret.B, B.Length);

            ret.C = C.TClone();

            ret.D = new long[D.Length];
            Array.Copy(D, ret.D, D.Length);

            return ret;
        }
    }
    public struct SE : ICloneable
    {
        public int A;
        public float B;
        public long C;
        public ulong D;
        public byte E;
        public string F;


        public static SE New()
        {
            return new SE() { A = 1, B = 2.2f, C = 3, D = 4, E = 5, F = "F" };
        }

        public object Clone()
        {
            return this;
        }

    }
    public struct SF
    {
        public int A;
        public float B;
        public long C;
        public ulong D;
        public byte E;
        public string F;


        public static SF New()
        {
            return new SF() { A = 1, B = 2.2f, C = 3, D = 4, E = 5, F = "F" };
        }
    }

    class MainProgram
    {
        static void Test1()
        {
            using (new LogTestScope("Test1"))
            {
                SA a;
                a.A = new int[100];
                a.B = new float[100];
                for (int i = 0; i < a.A.Length; i++)
                {
                    a.A[i] = i * 2;
                }
                for (int i = 0; i < a.B.Length; i++)
                {
                    a.B[i] = i * 3.1f;
                }

                SA b = a;
                Console.WriteLine("b.A:");
                foreach (var it in b.A)
                {
                    Console.Write(it);
                    Console.Write(" ");
                }
                Console.WriteLine("");

                Console.WriteLine("b.B:");
                foreach (var it in b.B)
                {
                    Console.Write(it);
                    Console.Write(" ");
                }
                Console.WriteLine("");
            }
        }


        // 数组是引用类型，所以bList与aList的地址是一样的
        static void Test2(int Count, int Count2)
        {
            //int Count = 1000;
            //int Count2 = 100;
            using (new LogTestScope("Test2"))
            {
                SA[] aList = new SA[Count];

                for (int i=0; i<aList.Length; i++)
                {
                    ref var it = ref aList[i];
                    it.A = new int[Count2];
                    it.B = new float[Count2];
                    for (int j = 0; j < it.A.Length; j++)
                    {
                        it.A[j] = (i + 1) * j * 2;
                    }
                    for (int j = 0; j < it.B.Length; j++)
                    {
                        it.B[j] = (i + 1) * j * 3.1f;
                    }
                }

                GC.Collect();
                GC.WaitForPendingFinalizers();
                GC.Collect();

                var watch = new Stopwatch();
                watch.Start();

                //Thread.MemoryBarrier();
                //var initialMemory = System.GC.GetTotalMemory(true);

                //Process currentProcess = System.Diagnostics.Process.GetCurrentProcess();
                //long totalBytesOfMemoryUsed1 = currentProcess.VirtualMemorySize64;

                SA[] bList = aList;

                //Thread.MemoryBarrier();
                //var finalMemory = System.GC.GetTotalMemory(true);
                //var consumption = finalMemory - initialMemory;
                var consumption = 0;
                //long totalBytesOfMemoryUsed2 = currentProcess.VirtualMemorySize64;
                watch.Stop();

                // totalBytesOfMemoryUsed2 - totalBytesOfMemoryUsed2
                Console.WriteLine($"Copy Array.Count={Count},{Count2}, cost={watch.Elapsed.TotalMilliseconds}ms, UsedMem={consumption}");

                {
                    Debug.Assert(bList.Length == aList.Length);
                    Debug.Assert(AddressHelper.GetAddress(bList) == AddressHelper.GetAddress(aList));

                    int i = 0, j = 0;
                    Debug.Assert(bList[i].A[j] == aList[i].A[j]);
                    Debug.Assert(bList[i].B[j] == aList[i].B[j]);
                    Debug.Assert(AddressHelper.GetAddress(bList[i].A) == AddressHelper.GetAddress(aList[i].A));
                    Debug.Assert(AddressHelper.GetAddress(bList[i].B) == AddressHelper.GetAddress(aList[i].B));

                    i = Count / 2;
                    j = Count2 / 2;
                    Debug.Assert(bList[i].A[j] == aList[i].A[j]);
                    Debug.Assert(bList[i].B[j] == aList[i].B[j]);
                    Debug.Assert(AddressHelper.GetAddress(bList[i].A) == AddressHelper.GetAddress(aList[i].A));
                    Debug.Assert(AddressHelper.GetAddress(bList[i].B) == AddressHelper.GetAddress(aList[i].B));

                    i = Count - 1;
                    j = Count2 - 1;
                    Debug.Assert(bList[i].A[j] == aList[i].A[j]);
                    Debug.Assert(bList[i].B[j] == aList[i].B[j]);
                    Debug.Assert(AddressHelper.GetAddress(bList[i].A) == AddressHelper.GetAddress(aList[i].A));
                    Debug.Assert(AddressHelper.GetAddress(bList[i].B) == AddressHelper.GetAddress(aList[i].B));

                    // 引用关系，所以一致
                    i = Count - 1;
                    j = Count2 - 1;
                    bList[i].A[j] = 0;
                    bList[i].B[j] = 1;
                    Debug.Assert(bList[i].A[j] == aList[i].A[j]);
                    Debug.Assert(bList[i].B[j] == aList[i].B[j]);
                    Debug.Assert(AddressHelper.GetAddress(bList[i].A) == AddressHelper.GetAddress(aList[i].A));
                    Debug.Assert(AddressHelper.GetAddress(bList[i].B) == AddressHelper.GetAddress(aList[i].B));
                }
            }
        }

        // Marshal拷贝
        static void Test3(int Count, int Count2)
        {
            //int Count = 1000;
            //int Count2 = 100;
            using (new LogTestScope("Test3"))
            {
                SB[] aList = new SB[Count];

                for (int i = 0; i < aList.Length; i++)
                {
                    ref var it = ref aList[i];
                    it = SB.New();
                    //it.A = new int[Count2];
                    //it.B = new float[Count2];
                    for (int j = 0; j < it.A.Length; j++)
                    {
                        it.A[j] = (i + 1) * j * 2;
                    }
                    for (int j = 0; j < it.B.Length; j++)
                    {
                        it.B[j] = (i + 1) * j * 3.1f;
                    }
                }


                var memWatch = new MemoryWatcher();

                var watch = new Stopwatch();
                watch.Start();

                SB[] bList = new SB[aList.Length];
                for (int i = 0; i < bList.Length; i++)
                {
                    var b = StructBytes.StructToBytes(aList[i]);
                    bList[i] = (SB)StructBytes.BytesToStuct(b, typeof(SB));
                }

                //long totalBytesOfMemoryUsed2 = currentProcess.WorkingSet64;
                //var consumption = totalBytesOfMemoryUsed2 - totalBytesOfMemoryUsed1;
                watch.Stop();

                var consumption = memWatch.Stop();

                // totalBytesOfMemoryUsed2 - totalBytesOfMemoryUsed2
                Console.WriteLine($"Copy Array.Count={Count},{Count2}, cost={watch.Elapsed.TotalMilliseconds}ms, UsedMem={consumption}");

                {
                    Debug.Assert(bList.Length == aList.Length);
                    Debug.Assert(AddressHelper.GetAddress(bList) != AddressHelper.GetAddress(aList));

                    int i = 0, j = 0;
                    Debug.Assert(bList[i].A[j] == aList[i].A[j]);
                    Debug.Assert(bList[i].B[j] == aList[i].B[j]);
                    Debug.Assert(AddressHelper.GetAddress(bList[i].A) != AddressHelper.GetAddress(aList[i].A));
                    Debug.Assert(AddressHelper.GetAddress(bList[i].B) != AddressHelper.GetAddress(aList[i].B));

                    i = Count / 2;
                    j = Count2 / 2;
                    Debug.Assert(bList[i].A[j] == aList[i].A[j]);
                    Debug.Assert(bList[i].B[j] == aList[i].B[j]);
                    Debug.Assert(AddressHelper.GetAddress(bList[i].A) != AddressHelper.GetAddress(aList[i].A));
                    Debug.Assert(AddressHelper.GetAddress(bList[i].B) != AddressHelper.GetAddress(aList[i].B));

                    i = Count - 1;
                    j = Count2 - 1;
                    Debug.Assert(bList[i].A[j] == aList[i].A[j]);
                    Debug.Assert(bList[i].B[j] == aList[i].B[j]);
                    Debug.Assert(AddressHelper.GetAddress(bList[i].A) != AddressHelper.GetAddress(aList[i].A));
                    Debug.Assert(AddressHelper.GetAddress(bList[i].B) != AddressHelper.GetAddress(aList[i].B));

                    i = Count - 1;
                    j = Count2 - 1;
                    bList[i].A[j] = 0;
                    bList[i].B[j] = 1;
                    Debug.Assert(bList[i].A[j] != aList[i].A[j]);
                    Debug.Assert(bList[i].B[j] != aList[i].B[j]);
                    Debug.Assert(AddressHelper.GetAddress(bList[i].A) != AddressHelper.GetAddress(aList[i].A));
                    Debug.Assert(AddressHelper.GetAddress(bList[i].B) != AddressHelper.GetAddress(aList[i].B));
                }
            }
        }

        // 重写clone方法
        static void Test4(int Count, int Count2)
        {
            //int Count = 1000;
            //int Count2 = 100;
            using (new LogTestScope("Test4"))
            {
                SC[] aList = new SC[Count];

                for (int i = 0; i < aList.Length; i++)
                {
                    ref var it = ref aList[i];
                    it = SC.New();
                    //it.A = new int[Count2];
                    //it.B = new float[Count2];
                    for (int j = 0; j < it.A.Length; j++)
                    {
                        it.A[j] = (i + 1) * j * 2;
                    }
                    for (int j = 0; j < it.B.Length; j++)
                    {
                        it.B[j] = (i + 1) * j * 3.1f;
                    }
                }


                var memWatch = new MemoryWatcher();

                var watch = new Stopwatch();
                watch.Start();

#if true
                SC[] bList = new SC[aList.Length];
                for (int i = 0; i < bList.Length; i++)
                {
                    bList[i] = (SC)aList[i].Clone();
                }
#else
                // 引用
                SC[] bList = (SC[])aList.Clone();
                //Array.Copy(aList, bList, aList.Length);
#endif

                //long totalBytesOfMemoryUsed2 = currentProcess.WorkingSet64;
                //var consumption = totalBytesOfMemoryUsed2 - totalBytesOfMemoryUsed1;
                watch.Stop();

                var consumption = memWatch.Stop();

                // totalBytesOfMemoryUsed2 - totalBytesOfMemoryUsed2
                Console.WriteLine($"Copy Array.Count={Count},{Count2}, cost={watch.Elapsed.TotalMilliseconds}ms, UsedMem={consumption}");

                {
                    Debug.Assert(bList.Length == aList.Length);
                    Debug.Assert(AddressHelper.GetAddress(bList) != AddressHelper.GetAddress(aList));

                    int i = 0, j = 0;
                    Debug.Assert(bList[i].A[j] == aList[i].A[j]);
                    Debug.Assert(bList[i].B[j] == aList[i].B[j]);
                    Debug.Assert(AddressHelper.GetAddress(bList[i].A) != AddressHelper.GetAddress(aList[i].A));
                    Debug.Assert(AddressHelper.GetAddress(bList[i].B) != AddressHelper.GetAddress(aList[i].B));

                    i = Count / 2;
                    j = Count2 / 2;
                    Debug.Assert(bList[i].A[j] == aList[i].A[j]);
                    Debug.Assert(bList[i].B[j] == aList[i].B[j]);
                    Debug.Assert(AddressHelper.GetAddress(bList[i].A) != AddressHelper.GetAddress(aList[i].A));
                    Debug.Assert(AddressHelper.GetAddress(bList[i].B) != AddressHelper.GetAddress(aList[i].B));

                    i = Count - 1;
                    j = Count2 - 1;
                    Debug.Assert(bList[i].A[j] == aList[i].A[j]);
                    Debug.Assert(bList[i].B[j] == aList[i].B[j]);
                    Debug.Assert(AddressHelper.GetAddress(bList[i].A) != AddressHelper.GetAddress(aList[i].A));
                    Debug.Assert(AddressHelper.GetAddress(bList[i].B) != AddressHelper.GetAddress(aList[i].B));

                    i = Count - 1;
                    j = Count2 - 1;
                    bList[i].A[j] = 0;
                    bList[i].B[j] = 1;
                    Debug.Assert(bList[i].A[j] != aList[i].A[j]);
                    Debug.Assert(bList[i].B[j] != aList[i].B[j]);
                    Debug.Assert(AddressHelper.GetAddress(bList[i].A) != AddressHelper.GetAddress(aList[i].A));
                    Debug.Assert(AddressHelper.GetAddress(bList[i].B) != AddressHelper.GetAddress(aList[i].B));
                }
            }
        }

        // 重写clone方法
        static void Test44(int Count, int Count2)
        {
            //int Count = 1000;
            //int Count2 = 100;
            using (new LogTestScope("Test44"))
            {
                CC[] aList = new CC[Count];

                for (int i = 0; i < aList.Length; i++)
                {
                    ref var it = ref aList[i];
                    it = CC.New();
                    //it.A = new int[Count2];
                    //it.B = new float[Count2];
                    for (int j = 0; j < it.A.Length; j++)
                    {
                        it.A[j] = (i + 1) * j * 2;
                    }
                    for (int j = 0; j < it.B.Length; j++)
                    {
                        it.B[j] = (i + 1) * j * 3.1f;
                    }
                }


                var memWatch = new MemoryWatcher();

                var watch = new Stopwatch();
                watch.Start();

#if true
                CC[] bList = new CC[aList.Length];
                for (int i = 0; i < bList.Length; i++)
                {
                    bList[i] = (CC)aList[i].Clone();
                }
#else
                // 引用
                SC[] bList = (SC[])aList.Clone();
                //Array.Copy(aList, bList, aList.Length);
#endif

                //long totalBytesOfMemoryUsed2 = currentProcess.WorkingSet64;
                //var consumption = totalBytesOfMemoryUsed2 - totalBytesOfMemoryUsed1;
                watch.Stop();

                var consumption = memWatch.Stop();

                // totalBytesOfMemoryUsed2 - totalBytesOfMemoryUsed2
                Console.WriteLine($"Copy Array.Count={Count},{Count2}, cost={watch.Elapsed.TotalMilliseconds}ms, UsedMem={consumption}");

                {
                    Debug.Assert(bList.Length == aList.Length);
                    Debug.Assert(AddressHelper.GetAddress(bList) != AddressHelper.GetAddress(aList));

                    int i = 0, j = 0;
                    Debug.Assert(bList[i].A[j] == aList[i].A[j]);
                    Debug.Assert(bList[i].B[j] == aList[i].B[j]);
                    Debug.Assert(AddressHelper.GetAddress(bList[i].A) != AddressHelper.GetAddress(aList[i].A));
                    Debug.Assert(AddressHelper.GetAddress(bList[i].B) != AddressHelper.GetAddress(aList[i].B));

                    i = Count / 2;
                    j = Count2 / 2;
                    Debug.Assert(bList[i].A[j] == aList[i].A[j]);
                    Debug.Assert(bList[i].B[j] == aList[i].B[j]);
                    Debug.Assert(AddressHelper.GetAddress(bList[i].A) != AddressHelper.GetAddress(aList[i].A));
                    Debug.Assert(AddressHelper.GetAddress(bList[i].B) != AddressHelper.GetAddress(aList[i].B));

                    i = Count - 1;
                    j = Count2 - 1;
                    Debug.Assert(bList[i].A[j] == aList[i].A[j]);
                    Debug.Assert(bList[i].B[j] == aList[i].B[j]);
                    Debug.Assert(AddressHelper.GetAddress(bList[i].A) != AddressHelper.GetAddress(aList[i].A));
                    Debug.Assert(AddressHelper.GetAddress(bList[i].B) != AddressHelper.GetAddress(aList[i].B));

                    i = Count - 1;
                    j = Count2 - 1;
                    bList[i].A[j] = 0;
                    bList[i].B[j] = 1;
                    Debug.Assert(bList[i].A[j] != aList[i].A[j]);
                    Debug.Assert(bList[i].B[j] != aList[i].B[j]);
                    Debug.Assert(AddressHelper.GetAddress(bList[i].A) != AddressHelper.GetAddress(aList[i].A));
                    Debug.Assert(AddressHelper.GetAddress(bList[i].B) != AddressHelper.GetAddress(aList[i].B));
                }
            }
        }

        // 使用StructArray代替Array
        static void Test5(int Count, int Count2)
        {
            //int Count = 1000;
            //int Count2 = 100;
            using (new LogTestScope("Test5"))
            {
                var aList = new StructArray<SC>(Count);

                for (int i = 0; i < aList.Length; i++)
                {
                    ref var it = ref aList.GetRef(i);
                    it = SC.New();
                    //it.A = new int[Count2];
                    //it.B = new float[Count2];
                    for (int j = 0; j < it.A.Length; j++)
                    {
                        it.A[j] = (i + 1) * j * 2;
                    }
                    for (int j = 0; j < it.B.Length; j++)
                    {
                        it.B[j] = (i + 1) * j * 3.1f;
                    }
                }


                var memWatch = new MemoryWatcher();

                var watch = new Stopwatch();
                watch.Start();

#if false
                SC[] bList = new SC[aList.Length];
                for (int i = 0; i < bList.Length; i++)
                {
                    bList[i] = (SC)aList[i].Clone();
                }
#else
                // 引用
                var bList = (StructArray<SC>)aList.Clone();
                //Array.Copy(aList, bList, aList.Length);
#endif

                //long totalBytesOfMemoryUsed2 = currentProcess.WorkingSet64;
                //var consumption = totalBytesOfMemoryUsed2 - totalBytesOfMemoryUsed1;
                watch.Stop();

                var consumption = memWatch.Stop();

                // totalBytesOfMemoryUsed2 - totalBytesOfMemoryUsed2
                Console.WriteLine($"Copy Array.Count={Count},{Count2}, cost={watch.Elapsed.TotalMilliseconds}ms, UsedMem={consumption}");

                {
                    Debug.Assert(bList.Length == aList.Length);
                    Debug.Assert(AddressHelper.GetAddress(bList.Value) != AddressHelper.GetAddress(aList.Value));

                    int i = 0, j = 0;
                    Debug.Assert(bList[i].A[j] == aList[i].A[j]);
                    Debug.Assert(bList[i].B[j] == aList[i].B[j]);
                    Debug.Assert(AddressHelper.GetAddress(bList[i].A) != AddressHelper.GetAddress(aList[i].A));
                    Debug.Assert(AddressHelper.GetAddress(bList[i].B) != AddressHelper.GetAddress(aList[i].B));

                    i = Count / 2;
                    j = Count2 / 2;
                    Debug.Assert(bList[i].A[j] == aList[i].A[j]);
                    Debug.Assert(bList[i].B[j] == aList[i].B[j]);
                    Debug.Assert(AddressHelper.GetAddress(bList[i].A) != AddressHelper.GetAddress(aList[i].A));
                    Debug.Assert(AddressHelper.GetAddress(bList[i].B) != AddressHelper.GetAddress(aList[i].B));

                    i = Count - 1;
                    j = Count2 - 1;
                    Debug.Assert(bList[i].A[j] == aList[i].A[j]);
                    Debug.Assert(bList[i].B[j] == aList[i].B[j]);
                    Debug.Assert(AddressHelper.GetAddress(bList[i].A) != AddressHelper.GetAddress(aList[i].A));
                    Debug.Assert(AddressHelper.GetAddress(bList[i].B) != AddressHelper.GetAddress(aList[i].B));

                    i = Count - 1;
                    j = Count2 - 1;
                    bList[i].A[j] = 0;
                    bList[i].B[j] = 1;
                    Debug.Assert(bList[i].A[j] != aList[i].A[j]);
                    Debug.Assert(bList[i].B[j] != aList[i].B[j]);
                    Debug.Assert(AddressHelper.GetAddress(bList[i].A) != AddressHelper.GetAddress(aList[i].A));
                    Debug.Assert(AddressHelper.GetAddress(bList[i].B) != AddressHelper.GetAddress(aList[i].B));
                }
            }
        }

        // 使用StructArray代替Array，嵌套！
        static void Test6(int Count, int Count2)
        {
            //int Count = 1000;
            //int Count2 = 100;
            using (new LogTestScope("Test6"))
            {
                var aList = new StructArray<SD>(Count);

                for (int i = 0; i < aList.Length; i++)
                {
                    ref var it = ref aList.GetRef(i);
                    it = SD.New();
                    //it.A = new int[Count2];
                    //it.B = new float[Count2];
                    for (int j = 0; j < it.A.Length; j++)
                    {
                        it.A[j] = (i + 1) * j * 2;
                    }
                    for (int j = 0; j < it.B.Length; j++)
                    {
                        it.B[j] = (i + 1) * j * 3.1f;
                    }
                    for (int j = 0; j < it.C.Length; j++)
                    {
                        for (int k = 0; k < it.C[j].A.Length; k++)
                        {
                            it.C[j].A[k] = (i + 1) * k * 2;
                        }
                        for (int k = 0; k < it.C[j].A.Length; k++)
                        {
                            it.C[j].B[k] = (i + 1) * k * 3.1f;
                        }
                    }
                    for (int j = 0; j < it.D.Length; j++)
                    {
                        it.D[j] = (i + 1) * j * 5;
                    }
                }


                var memWatch = new MemoryWatcher();

                var watch = new Stopwatch();
                watch.Start();

#if false
                SC[] bList = new SC[aList.Length];
                for (int i = 0; i < bList.Length; i++)
                {
                    bList[i] = (SC)aList[i].Clone();
                }
#else
                // 引用
                var bList = aList.TClone();
                //Array.Copy(aList, bList, aList.Length);
#endif

                //long totalBytesOfMemoryUsed2 = currentProcess.WorkingSet64;
                //var consumption = totalBytesOfMemoryUsed2 - totalBytesOfMemoryUsed1;
                watch.Stop();

                var consumption = memWatch.Stop();

                // totalBytesOfMemoryUsed2 - totalBytesOfMemoryUsed2
                Console.WriteLine($"Copy Array.Count={Count},{Count2}, cost={watch.Elapsed.TotalMilliseconds}ms, UsedMem={consumption}");

                {
                    Debug.Assert(bList.Length == aList.Length);
                    Debug.Assert(AddressHelper.GetAddress(bList.Value) != AddressHelper.GetAddress(aList.Value));

                    int i = 0, j = 0;
                    Debug.Assert(bList[i].A[j] == aList[i].A[j]);
                    Debug.Assert(bList[i].B[j] == aList[i].B[j]);
                    Debug.Assert(bList[i].C[0].A[j] == aList[i].C[0].A[j]);
                    Debug.Assert(bList[i].C[0].B[j] == aList[i].C[0].B[j]);
                    Debug.Assert(bList[i].D[j] == aList[i].D[j]);
                    Debug.Assert(AddressHelper.GetAddress(bList[i].A) != AddressHelper.GetAddress(aList[i].A));
                    Debug.Assert(AddressHelper.GetAddress(bList[i].B) != AddressHelper.GetAddress(aList[i].B));
                    Debug.Assert(AddressHelper.GetAddress(bList[i].C.Value) != AddressHelper.GetAddress(aList[i].C.Value));
                    Debug.Assert(AddressHelper.GetAddress(bList[i].D) != AddressHelper.GetAddress(aList[i].D));

                    i = Count / 2;
                    j = Count2 / 2;
                    Debug.Assert(bList[i].A[j] == aList[i].A[j]);
                    Debug.Assert(bList[i].B[j] == aList[i].B[j]);
                    Debug.Assert(bList[i].C[0].A[j] == aList[i].C[0].A[j]);
                    Debug.Assert(bList[i].C[0].B[j] == aList[i].C[0].B[j]);
                    Debug.Assert(bList[i].D[j] == aList[i].D[j]);
                    Debug.Assert(AddressHelper.GetAddress(bList[i].A) != AddressHelper.GetAddress(aList[i].A));
                    Debug.Assert(AddressHelper.GetAddress(bList[i].B) != AddressHelper.GetAddress(aList[i].B));
                    Debug.Assert(AddressHelper.GetAddress(bList[i].C.Value) != AddressHelper.GetAddress(aList[i].C.Value));
                    Debug.Assert(AddressHelper.GetAddress(bList[i].D) != AddressHelper.GetAddress(aList[i].D));

                    i = Count - 1;
                    j = Count2 - 1;
                    Debug.Assert(bList[i].A[j] == aList[i].A[j]);
                    Debug.Assert(bList[i].B[j] == aList[i].B[j]);
                    Debug.Assert(bList[i].C[0].A[j] == aList[i].C[0].A[j]);
                    Debug.Assert(bList[i].C[0].B[j] == aList[i].C[0].B[j]);
                    Debug.Assert(bList[i].D[j] == aList[i].D[j]);
                    Debug.Assert(AddressHelper.GetAddress(bList[i].A) != AddressHelper.GetAddress(aList[i].A));
                    Debug.Assert(AddressHelper.GetAddress(bList[i].B) != AddressHelper.GetAddress(aList[i].B));
                    Debug.Assert(AddressHelper.GetAddress(bList[i].C.Value) != AddressHelper.GetAddress(aList[i].C.Value));
                    Debug.Assert(AddressHelper.GetAddress(bList[i].D) != AddressHelper.GetAddress(aList[i].D));

                    i = Count - 1;
                    j = Count2 - 1;
                    bList[i].A[j] = 0;
                    bList[i].B[j] = 1;
                    Debug.Assert(bList[i].A[j] != aList[i].A[j]);
                    Debug.Assert(bList[i].B[j] != aList[i].B[j]);
                    Debug.Assert(bList[i].C[0].A[j] == aList[i].C[0].A[j]);
                    Debug.Assert(bList[i].C[0].B[j] == aList[i].C[0].B[j]);
                    Debug.Assert(bList[i].D[j] == aList[i].D[j]);
                    Debug.Assert(AddressHelper.GetAddress(bList[i].A) != AddressHelper.GetAddress(aList[i].A));
                    Debug.Assert(AddressHelper.GetAddress(bList[i].B) != AddressHelper.GetAddress(aList[i].B));
                    Debug.Assert(AddressHelper.GetAddress(bList[i].C.Value) != AddressHelper.GetAddress(aList[i].C.Value));
                    Debug.Assert(AddressHelper.GetAddress(bList[i].D) != AddressHelper.GetAddress(aList[i].D));
                }
            }
        }

        static void TestAddr()
        {
            var a = new CA();
            var aPtr = AddressHelper.GetAddress(a);
            Console.WriteLine($"a.addr=0x{aPtr.ToString("X").PadLeft(8,'0')}");

            void DumpClassAddr(object o, IntPtr oAddr)
            {
                var oPtr = AddressHelper.GetAddress(o);
                Console.WriteLine($"a.addr=0x{oPtr.ToString("X").PadLeft(8, '0')}");
                Debug.Assert(oPtr == oAddr);
            }
            void DumpStructAddr(object o, IntPtr oAddr)
            {
                var oPtr = AddressHelper.GetAddress(o);
                Console.WriteLine($"a.addr=0x{oPtr.ToString("X").PadLeft(8, '0')}");
                // 结构体被装箱后，地址变了
                Debug.Assert(oPtr != oAddr);
            }
            void DumpStructAddr1(ref SA o, IntPtr oAddr)
            {
                var oPtr = AddressHelper.GetAddress(ref o);
                Console.WriteLine($"a.addr=0x{oPtr.ToString("X").PadLeft(8, '0')}");
                // 结构体被装箱后，地址变了
                Debug.Assert(oPtr != oAddr);
            }

            DumpClassAddr(a, aPtr);

            var sa = new SA();
            var saPtr = AddressHelper.GetAddress(sa);
            var sao = (object)sa;
            var saoPtr = AddressHelper.GetAddress(sao);
            // 结构体被装箱后，地址变了
            Debug.Assert(saPtr != saoPtr);

            // 拆箱后，获取内容，但是，地址是不一样的，也就是并非是同一块内存。
            var sa1 = ((SA)sao);
            var sa1Ptr = AddressHelper.GetAddress(sa1);
            Debug.Assert(saPtr != sa1Ptr);

            DumpStructAddr(sa, saPtr);
            DumpStructAddr1(ref sa, saPtr);
        }

        static void Test7()
        {
            using (new LogTestScope("Test7"))
            {
                SE se = SE.New();
                var se2 = se;
                se2.A = se.A + 1;
                Debug.Assert(se2.A != se.A);

                var se3 = (SE)se.Clone();
                se3.A = se.A + 1;
                Debug.Assert(se3.A != se.A);

                // 装箱
                {
                    var se4 = (object)se;
                    if (se4 is ICloneable)
                    {
                        var se5 = (SE)(se4 as ICloneable).Clone();
                        Debug.Assert(se5.A == se.A);
                        se5.A = se.A + 1;
                        Debug.Assert(se5.A != se.A);
                    }
                }
                {
                    SF sf = new SF();
                    var sf4 = (object)sf;
                    // 没有实现Icloneable！
                    Debug.Assert(!(sf4 is ICloneable));
                }
            }
        }

        static void Main(string[] args)
        {
            Test1();

            Test2(1000, 100);

            Test3(10000, 100);

            Test4(10000, 100);
            Test44(10000, 100);

            Test5(10000, 100);

            Test6(10000, 100);

            TestAddr();

            Test7();
        }

    }
}
