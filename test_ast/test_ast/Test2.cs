using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace test_ast
{
    class Test2
    {
        const string programText =
@"using System;
using System.Collections;
using System.Linq;
using System.Text;

namespace HelloWorld
{
    class Program
    {
        static void Main(string[] args)
        {
            // zhushi
            int i=0;
            Console.WriteLine($""Hello, World! i={i}"");
        }
    }
}";



        public static void Main1()
        {
            // 分析代码，修改代码，并打印代码

            SyntaxTree tree = CSharpSyntaxTree.ParseText(programText);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();

            // root.ToString()，就是要生成的代码，支持注释，空格等
            Console.WriteLine(root.ToString());

            File.WriteAllText("out2.cs", root.ToString());
        }
    }
}
