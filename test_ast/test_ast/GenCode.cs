/**,
 * Auth :   liubo
 * Date :   2022/03/09 15:37
 * Comment: 根据AST生成代码
 *  参考：https://github.com/castleproject/NVelocity/blob/master/src/NVelocity.Tests/MultiThreadTestCase.cs
 *  https://www.cnblogs.com/fengyun1234/p/3478921.html
 * 
 * 
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using static System.Console;
using System.Diagnostics;
using NVelocity.App;
using NVelocity;

namespace test_ast
{    class CodeGenTools
    {
        public string Combine(params string[] Arr)
        {
            return string.Concat(Arr);

            var sb = new StringBuilder();
            foreach (var it in Arr)
            {
                sb.Append(it);
            }
            return sb.ToString();
        }
    }

    class GenCode
    {
        public static void Main1()
        {
            var Content = File.ReadAllText(@"E:\_tempSSD\1\test_ecs\Assets\Scripts\game\Skill\SkillAction.cs");
            SyntaxTree Tree = CSharpSyntaxTree.ParseText(Content);
            CompilationUnitSyntax root = Tree.GetCompilationUnitRoot();


            // 收集AST信息
            var ast = new ast.CollectAstInfoWalker();
            ast.Visit(Tree.GetRoot());
            Console.WriteLine(ast.ToString());

            // 生成代码
            /*
                规则：
                    使用相同的using
                    使用相同的namespace包裹

                    继承ISkillAction的，
                        生成一行代码：new SkillActionProc<SkillActionTest1>();
                        如果此类中，有引用类型，那么生成：
                            CloneFunc()
                            CompareFunc()

             */
            var velocityEngine = new VelocityEngine();
            velocityEngine.Init();

            StringWriter sw = new StringWriter();

            VelocityContext c = new VelocityContext();
            c.Put("usingList", ast.AstInfo.UsingList);
            c.Put("namespace", ast.AstInfo.NamespaceList[0]);
            c.Put("tools", new CodeGenTools());

            var template = File.ReadAllText(@"C:\Users\Administrator\source\repos\hello_csharp\test_ast\test_ast\SkillAction.template.vm");
            bool ok = velocityEngine.Evaluate(c, sw, "test1", template);
            Console.WriteLine(sw.ToString());
            File.WriteAllText("SkillAction.gencode.cs", sw.ToString());
        }
    }
}
