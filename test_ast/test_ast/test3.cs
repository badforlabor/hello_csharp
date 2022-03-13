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

namespace test_ast
{
    class Test3
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
            int j=0;
            Console.WriteLine($""Hello, World! i={i}"");
        }
    }
}";



        public static void Main1()
        {
            // 分析代码，修改代码，并打印代码

            SyntaxTree tree = CSharpSyntaxTree.ParseText(programText);
            CompilationUnitSyntax root = tree.GetCompilationUnitRoot();
            

            // 修改Using
            {
                NameSyntax name = SyntaxFactory.IdentifierName("System");
                WriteLine($"\tCreated the identifier {name}");
                name = QualifiedName(name, IdentifierName("Collections"));
                WriteLine(name.ToString());

                name = QualifiedName(name, IdentifierName("Generic"));
                WriteLine(name.ToString());

                var oldUsing = root.Usings[1];
                var newUsing = oldUsing.WithName(name);

                // 替换代码
                root = root.ReplaceNode(oldUsing, newUsing);
                WriteLine(root.ToString());

                // 追加Namespace
                root = root.AddUsings(oldUsing);
                WriteLine(root.ToString());
            }

            // 修改类
            {
                // 代码中包含多个namespace定义，每个namespace包含多个成员（类，结构体，枚举，delegate等）
                // 每个类包含：变量，函数等
                MemberDeclarationSyntax firstMember = root.Members[0];
                var helloWorldDeclaration = (NamespaceDeclarationSyntax)firstMember;


                // 类的修饰，partial，static等
                var classDeclaration = (ClassDeclarationSyntax)helloWorldDeclaration.Members[0];



                // 变量名

                // 变量变成属性

                // 函数名   
                var methodDeclaration = (MethodDeclarationSyntax)classDeclaration.Members[0];
                //for (var token = methodDeclaration.GetFirstToken(); token != methodDeclaration.GetLastToken(); token = token.GetNextToken())
                //{
                //    Console.WriteLine(token);
                //}
                if (false)
                {
                    // 去掉static关键字
                    var StaticToken = SyntaxFactory.Token(SyntaxKind.StaticKeyword);
                    foreach (var it in methodDeclaration.Modifiers)
                    {
                        Console.WriteLine(it);
                        if (it == StaticToken)
                        {

                        }
                        if (it.RawKind == (int)SyntaxKind.StaticKeyword)
                        {
                            var newToken = SyntaxFactory.Token(SyntaxKind.None);
                            root = root.ReplaceToken(it, newToken);
                        }
                    }
                }
                else if (false)
                {
                    // 添加static关键字
                    var newNode = methodDeclaration.AddModifiers(SyntaxFactory.Token(SyntaxKind.StaticKeyword));
                    root = root.ReplaceNode(methodDeclaration, newNode);
                }

                // 函数的参数
                //methodDeclaration.ParameterList;

                // 函数体内容
                foreach(var it in methodDeclaration.Body.Statements)
                {
                    Console.WriteLine(it);
                    if (it is LocalDeclarationStatementSyntax)
                    {
                        // 如果是本地变量，那么加上一个const关键字
                        var localVariableSyntax = (LocalDeclarationStatementSyntax)it;
                        var localVariable = localVariableSyntax.Declaration;
#if false
                        foreach (var t in localVariable.GetLeadingTrivia())
                        {
                            Console.Write($"{t.ToString()}");
                        }                        

                        Console.Write($"{localVariable.Type.ToString()} ");
                        foreach (var v in localVariable.Variables)
                        {
                            Console.Write($"{v.ToString()}");
                        }


                        foreach (var t in localVariable.GetTrailingTrivia())
                        {
                            Console.Write($"{t.ToString()}");
                        }
#endif


                        var newNode = localVariableSyntax.WithoutLeadingTrivia();
                        newNode = newNode.WithModifiers(TokenList(
                                        Token(
                                            localVariable.GetLeadingTrivia(),
                                            SyntaxKind.ConstKeyword,
                                            TriviaList())));

                        //newNode = newNode.WithLeadingTrivia(localVariable.Declaration.GetLeadingTrivia());

                        root = root.ReplaceNode(localVariableSyntax, newNode).NormalizeWhitespace();
                    }
                }

                // 函数体的第一行，加上：打印函数名字
                var newExp = ExpressionStatement(
                                    InvocationExpression(
                                        MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            IdentifierName("Console"),
                                            IdentifierName("WriteLine")))
                                    .WithArgumentList(
                                        ArgumentList(
                                            SingletonSeparatedList<ArgumentSyntax>(
                                                Argument(
                                                    InterpolatedStringExpression(
                                                        Token(SyntaxKind.InterpolatedStringStartToken))
                                                    .WithContents(
                                                        List<InterpolatedStringContentSyntax>(
                                                            new InterpolatedStringContentSyntax[]{
                                                                InterpolatedStringText()
                                                                .WithTextToken(
                                                                    Token(
                                                                        TriviaList(),
                                                                        SyntaxKind.InterpolatedStringTextToken,
                                                                        "Hello, World! i=",
                                                                        "Hello, World! i=",
                                                                        TriviaList())),
                                                                Interpolation(
                                                                    IdentifierName("i"))})))))));

                Console.WriteLine(newExp.ToString());
                var oldState = methodDeclaration.Body.Statements;
                oldState = oldState.Insert(0, newExp);
                var newBody = methodDeclaration.Body.WithStatements(oldState);
                root = root.ReplaceNode(methodDeclaration.Body, newBody);   // 额，不好用
                Console.WriteLine(root.ToString());


            }


            //root = root.NormalizeWhitespace();

            WriteLine(root.ToString());
            File.WriteAllText("out2.cs", root.ToString());
        }
    }
}
