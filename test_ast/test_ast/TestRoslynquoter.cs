/**,
 * Auth :   liubo
 * Date :   2022/03/08 09:51
 * Comment: 测试roslynquoter，非常好的工具：https://github.com/KirillOsenkov/RoslynQuoter  
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;
using System.IO;

namespace test_ast
{
    class TestRoslynquoter
    {
        public static void Main1()
        {
            var root = CompilationUnit()
.WithUsings(
    List<UsingDirectiveSyntax>(
        new UsingDirectiveSyntax[]{
            UsingDirective(
                IdentifierName("System")),
            UsingDirective(
                QualifiedName(
                    QualifiedName(
                        IdentifierName("System"),
                        IdentifierName("Collections")),
                    IdentifierName("Generic"))),
            UsingDirective(
                QualifiedName(
                    IdentifierName("System"),
                    IdentifierName("Linq"))),
            UsingDirective(
                QualifiedName(
                    IdentifierName("System"),
                    IdentifierName("Text"))),
            UsingDirective(
                QualifiedName(
                    QualifiedName(
                        IdentifierName("System"),
                        IdentifierName("Threading")),
                    IdentifierName("Tasks"))),
            UsingDirective(
                QualifiedName(
                    IdentifierName("Microsoft"),
                    IdentifierName("CodeAnalysis"))),
            UsingDirective(
                QualifiedName(
                    QualifiedName(
                        IdentifierName("Microsoft"),
                        IdentifierName("CodeAnalysis")),
                    IdentifierName("CSharp"))),
            UsingDirective(
                QualifiedName(
                    QualifiedName(
                        QualifiedName(
                            IdentifierName("Microsoft"),
                            IdentifierName("CodeAnalysis")),
                        IdentifierName("CSharp")),
                    IdentifierName("Syntax"))),
            UsingDirective(
                QualifiedName(
                    IdentifierName("System"),
                    IdentifierName("Console")))
            .WithStaticKeyword(
                Token(SyntaxKind.StaticKeyword))}))
.WithMembers(
    SingletonList<MemberDeclarationSyntax>(
        NamespaceDeclaration(
            IdentifierName("test_ast"))
        .WithMembers(
            SingletonList<MemberDeclarationSyntax>(
                ClassDeclaration("Program")
                .WithMembers(
                    SingletonList<MemberDeclarationSyntax>(
                        MethodDeclaration(
                            PredefinedType(
                                Token(SyntaxKind.VoidKeyword)),
                            Identifier("Main"))
                        .WithModifiers(
                            TokenList(
                                Token(SyntaxKind.StaticKeyword)))
                        .WithParameterList(
                            ParameterList(
                                SingletonSeparatedList<ParameterSyntax>(
                                    Parameter(
                                        Identifier("args"))
                                    .WithType(
                                        ArrayType(
                                            PredefinedType(
                                                Token(SyntaxKind.StringKeyword)))
                                        .WithRankSpecifiers(
                                            SingletonList<ArrayRankSpecifierSyntax>(
                                                ArrayRankSpecifier(
                                                    SingletonSeparatedList<ExpressionSyntax>(
                                                        OmittedArraySizeExpression()))))))))
                        .WithBody(
                            Block(
                                LocalDeclarationStatement(
                                    VariableDeclaration(
                                        PredefinedType(
                                            Token(
                                                TriviaList(
                                                    Comment("// zhushi1")),
                                                SyntaxKind.IntKeyword,
                                                TriviaList())))
                                    .WithVariables(
                                        SingletonSeparatedList<VariableDeclaratorSyntax>(
                                            VariableDeclarator(
                                                Identifier("a"))
                                            .WithInitializer(
                                                EqualsValueClause(
                                                    LiteralExpression(
                                                        SyntaxKind.NumericLiteralExpression,
                                                        Literal(0))))))),
                                LocalDeclarationStatement(
                                    VariableDeclaration(
                                        IdentifierName(
                                            Identifier(
                                                TriviaList(
                                                    Comment("// zhushi2")),
                                                SyntaxKind.VarKeyword,
                                                "var",
                                                "var",
                                                TriviaList())))
                                    .WithVariables(
                                        SingletonSeparatedList<VariableDeclaratorSyntax>(
                                            VariableDeclarator(
                                                Identifier("b"))
                                            .WithInitializer(
                                                EqualsValueClause(
                                                    LiteralExpression(
                                                        SyntaxKind.FalseLiteralExpression)))))),
                                LocalDeclarationStatement(
                                    VariableDeclaration(
                                        PredefinedType(
                                            Token(SyntaxKind.FloatKeyword)))
                                    .WithVariables(
                                        SingletonSeparatedList<VariableDeclaratorSyntax>(
                                            VariableDeclarator(
                                                Identifier("c"))
                                            .WithInitializer(
                                                EqualsValueClause(
                                                    LiteralExpression(
                                                        SyntaxKind.NumericLiteralExpression,
                                                        Literal(1.1f)))))))
                                .WithModifiers(
                                    TokenList(
                                        Token(
                                            TriviaList(
                                                Comment("// zhushi3")),
                                            SyntaxKind.ConstKeyword,
                                            TriviaList()))),
                                ExpressionStatement(
                                    InvocationExpression(
                                        MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            IdentifierName(
                                                Identifier(
                                                    TriviaList(
                                                        Comment(@"/*
             * zhushi4
             */")),
                                                    "Console",
                                                    TriviaList())),
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
                                                                Interpolation(
                                                                    IdentifierName("a")),
                                                                InterpolatedStringText()
                                                                .WithTextToken(
                                                                    Token(
                                                                        TriviaList(),
                                                                        SyntaxKind.InterpolatedStringTextToken,
                                                                        ", ",
                                                                        ", ",
                                                                        TriviaList())),
                                                                Interpolation(
                                                                    IdentifierName("b")),
                                                                InterpolatedStringText()
                                                                .WithTextToken(
                                                                    Token(
                                                                        TriviaList(),
                                                                        SyntaxKind.InterpolatedStringTextToken,
                                                                        ", ",
                                                                        ", ",
                                                                        TriviaList())),
                                                                Interpolation(
                                                                    IdentifierName("c"))}))))))),
                                ExpressionStatement(
                                    InvocationExpression(
                                        MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            IdentifierName("test1"),
                                            IdentifierName("Main1")))
                                    .WithArgumentList(
                                        ArgumentList(
                                            SingletonSeparatedList<ArgumentSyntax>(
                                                Argument(
                                                    IdentifierName("args"))))))
                                .WithSemicolonToken(
                                    Token(
                                        TriviaList(),
                                        SyntaxKind.SemicolonToken,
                                        TriviaList(
                                            Comment("// zhushi5")))),
                                ExpressionStatement(
                                    InvocationExpression(
                                        MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            IdentifierName("test2"),
                                            IdentifierName("Main1"))))
                                .WithSemicolonToken(
                                    Token(
                                        TriviaList(),
                                        SyntaxKind.SemicolonToken,
                                        TriviaList(
                                            Comment("// zhushi6")))),
                                ExpressionStatement(
                                    InvocationExpression(
                                        MemberAccessExpression(
                                            SyntaxKind.SimpleMemberAccessExpression,
                                            IdentifierName("test3"),
                                            IdentifierName("Main1"))))))))))))
                .NormalizeWhitespace().SyntaxTree;

            /// 666
            Console.WriteLine(root.ToString());
            File.WriteAllText("out.cs", root.ToString());
        }
    }
}
