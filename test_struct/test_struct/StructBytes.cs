using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace test_struct
{
    //注意这个属性不能少
    [StructLayoutAttribute(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
    struct TestStruct
    {
        public int c;
        //字符串，SizeConst为字符串的最大长度
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string str;
        //int数组，SizeConst表示数组的个数，在转换成
        //byte数组前必须先初始化数组，再使用，初始化
        //的数组长度必须和SizeConst一致，例test = new int[6];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public int[] test;
    }

    class StructBytes
    {
        /// 结构体转byte数组
        /// </summary>
        /// <param name=”structObj”>要转换的结构体</param>
        /// <returns>转换后的byte数组</returns>
        public static byte[] StructToBytes(object structObj)
        {
            //得到结构体的大小
            int size = Marshal.SizeOf(structObj);
            //创建byte数组
            byte[] bytes = new byte[size];
            //分配结构体大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将结构体拷到分配好的内存空间
            Marshal.StructureToPtr(structObj, structPtr, false);
            //从内存空间拷到byte数组
            Marshal.Copy(structPtr, bytes, 0, size);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            //返回byte数组
            return bytes;
        }

        /// <summary>
        /// byte数组转结构体
        /// </summary>
        /// <param name=”bytes”>byte数组</param>
        /// <param name=”type”>结构体类型</param>
        /// <returns>转换后的结构体</returns>
        public static object BytesToStuct(byte[] bytes, Type type)
        {
            //得到结构体的大小
            int size = Marshal.SizeOf(type);
            //byte数组长度小于结构体的大小
            if (size > bytes.Length)
            {
                //返回空
                return null;
            }
            //分配结构体大小的内存空间
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            //将byte数组拷到分配好的内存空间
            Marshal.Copy(bytes, 0, structPtr, size);
            //将内存空间转换为目标结构体
            object obj = Marshal.PtrToStructure(structPtr, type);
            //释放内存空间
            Marshal.FreeHGlobal(structPtr);
            //返回结构体
            return obj;
        }

        public static object NewStruct(Type type)
        {
            int size = Marshal.SizeOf(type);
            IntPtr structPtr = Marshal.AllocHGlobal(size);
            for (int i = 0; i < size; i++)
            {
                Marshal.WriteByte(structPtr, i, 0);
            }
            object obj = Marshal.PtrToStructure(structPtr, type);
            Marshal.FreeHGlobal(structPtr);
            return obj;
        }

        public static IntPtr GetAddr(ref object o)
        {
            return AddressHelper.GetAddress(o);
        }
    }
    public static class AddressHelper
    {
        private static object mutualObject;
        private static ObjectReinterpreter reinterpreter;

        static AddressHelper()
        {
            AddressHelper.mutualObject = new object();
            AddressHelper.reinterpreter = new ObjectReinterpreter();
            AddressHelper.reinterpreter.AsObject = new ObjectWrapper();
        }

        public static IntPtr GetAddress(object obj)
        {
            lock (AddressHelper.mutualObject)
            {
                AddressHelper.reinterpreter.AsObject.Object = obj;
                IntPtr address = AddressHelper.reinterpreter.AsIntPtr.Value;
                AddressHelper.reinterpreter.AsObject.Object = null;
                return address;
            }
        }

        public static IntPtr GetAddress<T>(ref T a) where T : struct
        {
            // 注意，被装箱了，不是真的地址！
            lock (AddressHelper.mutualObject)
            {
                AddressHelper.reinterpreter.AsObject.Object = a;
                IntPtr address = AddressHelper.reinterpreter.AsIntPtr.Value;
                AddressHelper.reinterpreter.AsObject.Object = null;
                return address;
            }
        }


        public static T GetInstance<T>(IntPtr address)
        {
            lock (AddressHelper.mutualObject)
            {
                AddressHelper.reinterpreter.AsIntPtr.Value = address;
                return (T)AddressHelper.reinterpreter.AsObject.Object;
            }
        }

        // I bet you thought C# was type-safe.
        [StructLayout(LayoutKind.Explicit)]
        private struct ObjectReinterpreter
        {
            [FieldOffset(0)] public ObjectWrapper AsObject;
            [FieldOffset(0)] public IntPtrWrapper AsIntPtr;
        }

        private class ObjectWrapper
        {
            public object Object;
        }

        private class IntPtrWrapper
        {
            public IntPtr Value;
        }
    }



}
