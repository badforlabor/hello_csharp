/**,
 * Auth :   liubo
 * Date :   2022/03/08 16:44
 * Comment: 收集类的信息  
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

namespace test_ast
{
    class UsingCollector : CSharpSyntaxWalker
    // </Snippet3>
    {
        // <Snippet4>
        public ICollection<UsingDirectiveSyntax> Usings { get; } = new List<UsingDirectiveSyntax>();
        // </Snippet4>

        // <SNippet5>
        public override void VisitUsingDirective(UsingDirectiveSyntax node)
        {
            WriteLine($"\tVisitUsingDirective called with {node.Name}.");
            if (node.Name.ToString() != "System" &&
                !node.Name.ToString().StartsWith("System."))
            {
                WriteLine($"\t\tSuccess. Adding {node.Name}.");
                this.Usings.Add(node);
            }
        }
        // </Snippet5>
    }
    public class DumpTreeWalker : CSharpSyntaxWalker
    {
        public StringBuilder Sbuf = new StringBuilder();
        static int Tabs = 0;
        public override void Visit(SyntaxNode node)
        {
            Tabs++;
            var indents = new String('\t', Tabs);
            Sbuf.AppendLine(indents + node.Kind());
            base.Visit(node);
            Tabs--;
        }
        public override void VisitUsingDirective(UsingDirectiveSyntax node)
        {
            base.VisitUsingDirective(node);

            // 譬如 using System.Collections;返回System.Collections
            Console.WriteLine(node.Name);
        }

        public override void VisitFileScopedNamespaceDeclaration(FileScopedNamespaceDeclarationSyntax node)
        {
            base.VisitFileScopedNamespaceDeclaration(node);

            // 暂时无效
            Console.WriteLine(node.Name);
        }

        public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            base.VisitNamespaceDeclaration(node);

            // 譬如namespace cell，返回cell
            Console.WriteLine(node.Name);
        }

        public override void VisitClassOrStructConstraint(ClassOrStructConstraintSyntax node)
        {
            base.VisitClassOrStructConstraint(node);

            // 返回的是class，struct关键字，没啥用
            Console.WriteLine(node.ClassOrStructKeyword);
        }

        public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            base.VisitInterfaceDeclaration(node);

            // 譬如interface IA, 返回的是IA
            Console.WriteLine(node.Identifier);

            Dump(node);
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            base.VisitClassDeclaration(node);

            // 譬如class A, 返回的是A
            Console.WriteLine(node.Identifier);

            Dump(node);
        }

        void Dump(TypeDeclarationSyntax node)
        {

            // 继承的接口
            if (node.BaseList != null)
            {
                foreach (var b in node.BaseList.Types)
                {
                    Dump(b);
                }
            }

            foreach (var it in node.Members)
            {
                // 成员变量
                if (it is FieldDeclarationSyntax a)
                {
                    // 类型
                    Dump(a.Declaration.Type);

                    // 变量名
                    foreach (var jt in a.Declaration.Variables)
                    {
                        Dump(jt);
                    }

                    // 修饰的属性
                    foreach (var jt in a.AttributeLists)
                    {
                        Dump(jt);
                    }

                    // modifiers，譬如public,protected等

                }
                else if (it is PropertyDeclarationSyntax pro)
                {
                    // 类型
                    Dump(pro.Type);

                    // 变量名
                    Console.WriteLine(pro.Identifier.ToString());

                    // 修饰的属性
                    foreach (var jt in pro.AttributeLists)
                    {
                        Dump(jt);
                    }

                    // modifiers，譬如public,protected等
                }
                else if (it is MethodDeclarationSyntax func)
                {
                    // 函数名字
                    Console.WriteLine(func.Identifier);

                    // 函数参数
                    foreach (var p in func.ParameterList.Parameters)
                    {
                        Console.WriteLine($"ArgType={p.Type}, ArgName={p.Identifier}");
                    }

                    // modifiers，譬如public,protected等
                    foreach (var m in func.Modifiers)
                    {
                        Console.WriteLine(m);
                    }
                }
                else if (it is ConstructorDeclarationSyntax c)
                {
                    // 构造函数
                    Console.WriteLine(c.Identifier);

                    // 函数参数
                    foreach (var p in c.ParameterList.Parameters)
                    {
                        Console.WriteLine($"ArgType={p.Type}, ArgName={p.Identifier}");
                    }

                    // modifiers，譬如public,protected等
                    foreach (var m in c.Modifiers)
                    {
                        Console.WriteLine(m);
                    }

                }
                else
                {
                    throw new Exception("unknown type.");
                }
            }
        }

        public override void VisitStructDeclaration(StructDeclarationSyntax node)
        {
            base.VisitStructDeclaration(node);

            // 譬如struct A，返回的是A
            // 结构体名字
            Console.WriteLine(node.Identifier);

            Dump(node);
            return;

            // 继承的接口
            foreach (var b in node.BaseList.Types)
            {
                Dump(b);
            }

            foreach (var it in node.Members)
            {
                // 成员变量
                if (it is FieldDeclarationSyntax a)
                {
                    // 类型
                    Dump(a.Declaration.Type);

                    // 变量名
                    foreach (var jt in a.Declaration.Variables)
                    {
                        Dump(jt);
                    }

                    // 修饰的属性
                    foreach (var jt in a.AttributeLists)
                    {
                        Dump(jt);
                    }

                    // modifiers，譬如public,protected等

                }
                else if (it is MethodDeclarationSyntax func)
                {
                    // 函数名字
                    Console.WriteLine(func.Identifier);

                    // 函数参数
                    foreach (var p in func.ParameterList.Parameters)
                    {
                        Console.WriteLine($"ArgType={p.Type}, ArgName={p.Identifier}");
                    }
                }
                else
                {
                    throw new Exception("unknown type.");
                }
            }
        }

        static void Dump(BaseTypeSyntax b)
        {
            if (b is SimpleBaseTypeSyntax)
            {
                var bb = (SimpleBaseTypeSyntax)b;
                Dump(bb.Type);
            }
            else
            {
                throw new Exception("unknown type.");
            }
        }
        
        // 
        static void Dump(TypeSyntax node)
        {
            if (node is IdentifierNameSyntax)
            {
                var cc = (IdentifierNameSyntax)node;
                Console.WriteLine(cc.Identifier);
            }
            else if (node is GenericNameSyntax g)
            {
                Console.WriteLine(g.Identifier);
                foreach (var t in g.TypeArgumentList.Arguments)
                {
                    Debug.Assert(t is IdentifierNameSyntax || t is PredefinedTypeSyntax);
                    Dump(t);
                }
            }
            else if (node is PredefinedTypeSyntax p)
            {
                // 譬如int
                Console.WriteLine(p.Keyword.ToString());
            }
            else if (node is ArrayTypeSyntax a)
            {
                // 譬如int[]
                Console.WriteLine(a.ElementType + "[]");
            }
            else
            {
                throw new Exception("unknown type.");
            }
        }

        // 变量
        static void Dump(VariableDeclaratorSyntax node)
        {
            Console.WriteLine(node.Identifier);
        }

        // 属性
        static void Dump(AttributeListSyntax node)
        {
            //Console.WriteLine(node);
            foreach (var a in node.Attributes)
            {
                Console.WriteLine(a.Name);
                if (a.ArgumentList != null)
                {
                    foreach (var aa in a.ArgumentList.Arguments)
                    {
                        Console.WriteLine(aa.ContainsAnnotations);
                    }
                }
            }
        }
    }

    class Test4
    {
        public static void Main1()
        {
            var Content = File.ReadAllText(@"E:\_tempSSD\1\test_ecs\Assets\Scripts\game\Skill\SkillAction.cs");
            SyntaxTree Tree = CSharpSyntaxTree.ParseText(Content);
            CompilationUnitSyntax root = Tree.GetCompilationUnitRoot();

            var collector = new UsingCollector();
            collector.Visit(root);

            foreach (var directive in collector.Usings)
            {
                WriteLine(directive.Name);
            }

            var walker = new DumpTreeWalker();
            walker.Visit(Tree.GetRoot());
            File.WriteAllText("output.txt", walker.Sbuf.ToString());

            // 收集AST信息
            var ast = new ast.CollectAstInfoWalker();
            ast.Visit(Tree.GetRoot());
            Console.WriteLine(ast.ToString());
        }
    }
}
