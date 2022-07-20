using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace test_reflect_dll
{
    class Program
    {
        // 配置一次，不允许修改
        public static List<string> hotDlls = new List<string>()
        {
            "Proto.dll",
            "Common.dll",
            "Table.dll",
            "HexGrid.dll",
            "Battle.dll",
            "Assembly-CSharp-firstpass.dll",
            "Assembly-CSharp.dll",
        };

        static void Main(string[] args)
        {

            var dir = @"E:\workspace\infinite\gm-b3-pure\dev_huatuo\HybridCLRData\HotFixDlls\Android";
            var files = Directory.GetFiles(dir, "*.dll", SearchOption.TopDirectoryOnly);

            var depSet = new HashSet<string>();
            foreach (var f in files)
            {
                var fileName = Path.GetFileName(f);
                if (!hotDlls.Contains(fileName))
                {
                    continue;
                }

                var ass = Assembly.LoadFile(f);
                var refAssList = ass.GetReferencedAssemblies();
                foreach (var dep in refAssList)
                {
                    Console.WriteLine($"ass={f}, dep={dep.FullName}");
                    depSet.Add(dep.Name);
                }

                //var types = ass.GetTypes();


                var ad = AssemblyDefinition.ReadAssembly(f);
                DumpAssemblies(new AssemblyDefinition[] { ad });
            }

            foreach (var dep in depSet)
            {
                Console.WriteLine($"dep={dep}");
            }

            foreach (var dep in depSet)
            {
                Console.WriteLine($"<assembly fullname=\"{ dep} \" preserve=\"all\" />");
            }
        }

        // 打印依赖的类
        static void DumpAssemblies(AssemblyDefinition[] assemblies)
        {
            var types = assemblies
                .SelectMany(assembly => assembly.MainModule.Types.Cast<TypeDefinition>());

            var dependencies = types.ToDictionary(
                key => key,
                typedef => new HashSet<string>(typedef.Methods.Cast<MethodDefinition>()
                            .Where(method => null != method.Body) // ignore abstracts and generics
                            .SelectMany(method => method.Body.Instructions.Cast<Instruction>())
                            .Select(instr => instr.Operand)
                            .OfType<TypeReference>().Distinct()
                            //    .Where(type => !type.Namespace.StartsWith("System"))
                            .Select(type => type.FullName)));

            foreach (var entry in dependencies)
            {
                Console.WriteLine("{0}\t{1}", entry.Key.Name, string.Join(", ", entry.Value.ToArray()));
            }
        }
    }
}
