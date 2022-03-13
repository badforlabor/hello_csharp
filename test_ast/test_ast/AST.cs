/**,
 * Auth :   liubo
 * Date :   2022/03/09 10:03
 * Comment: 解析cs文件，并生成类结构信息  
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
using System.Diagnostics;
using Newtonsoft;
using Newtonsoft.Json;
using crazy;

namespace ast
{
    /*
        结构信息：
            namespace
                class
                    inherit info
                    member
                        property, type, name, comment
                    function
                        variable list
    */

    // 命名空间
    public class AstNamespace
    {
        public string Name { get; set; }
        public List<string> Comment = new List<string>();
        public List<AstClassName> _ClassList = new List<AstClassName>();
        public List<AstClassName> ClassList { get { return _ClassList; } }
    }

    // 类信息
    public class AstClassName
    {
        public enum ClassType
        {
            None,
            Class,
            Struct,
            Interface,            
        }

        public ClassType Type { get; set; } // class, struct, interface
        public string Name { get; set; }
        public List<AstInheritInfo> InheritList = new List<AstInheritInfo>();
        List<AstMemberInfo> _MemberList = new List<AstMemberInfo>();
        public List<AstMemberInfo> MemberList { get { return _MemberList; } }
        public List<AstMemberInfo> PropertyList = new List<AstMemberInfo>();
        public List<AstAttributeInfo> AttributeList = new List<AstAttributeInfo>();
        public List<string> Modifiers = new List<string>();
        public List<string> Comment = new List<string>();
        // where
        public List<AstConstraintInfo> Constraints = new List<AstConstraintInfo>();
        //<T>
        public List<string> TypeParameterList = new List<string>();

        public HashSet<string> GetAllToken()
        {
            var strList = new HashSet<string>();
            strList.Add(Type.Desc());            
            strList.UnionWith(Modifiers);

            InheritList.ForEach((i, v) =>
            {
                strList.Add(v.Type);
            });

            strList.UnionWith(TypeParameterList);
            Constraints.ForEach((i, v) =>
            {
                strList.UnionWith(v.Namelist);
            });

            return strList;
        }

        public bool HasAttribute(params string[] tokenList)
        {
            return AstInfoExtensions.HasAttribute(AttributeList, tokenList);
        }
        public bool IsInherit(params string[] tokenList)
        {
            if (tokenList == null)
            {
                return false;
            }
            
            foreach (var it in tokenList)
            {
                if (InheritList.FindIndex(x => { return x.Type == it; }) < 0)
                {
                    return false;
                }
            }

            return true;
        }

        public bool HasAllToken(params string[] tokenList)
        {
            if (tokenList == null)
            {
                return false;
            }

            var strList = GetAllToken();

            foreach (var it in tokenList)
            {
                if (!strList.Contains(it))
                {
                    return false;
                }
            }

            return true;
        }

        public bool HasAnyToken(params string[] tokenList)
        {
            if (tokenList == null)
            {
                return false;
            }

            var strList = GetAllToken();

            foreach (var it in tokenList)
            {
                if (strList.Contains(it))
                {
                    return true;
                }
            }

            return false;
        }

        public string GetClassDefineDesc()
        {
            // 拼凑出：
            /*
                public partial class AAA : BBB
                public partial class CCC<T> : DDD, EEE where T : FFF, GGG
             */

            var sb = new StringBuilder();

            // public
            sb.Append($"{AstInfoExtensions.GetPubDesc(Modifiers)} ");

            // partial
            if (AstInfoExtensions.HasModifier(Modifiers, "partial"))
            {
                sb.Append("partial ");
            }

            // class
            sb.Append($"{Type.Desc()} ");

            // CCC
            sb.Append($"{Name}");

            // <T, T1, T2>
            if (TypeParameterList.Count > 0)
            {
                sb.Append($"<{TypeParameterList[0]}");
                for(int i=1; i< TypeParameterList.Count; i++)
                {
                    sb.Append($", {TypeParameterList[i]}");
                }
                sb.Append(">");
            }

            sb.Append(" ");

            // : ClassA, ClassB
            if (InheritList.Count > 0)
            {
                sb.Append(": ");
                
                for(int i=0; i<InheritList.Count; i++)
                {
                    if (i == 0)
                    {
                        sb.Append($"{InheritList[i].Type}");
                    }
                    else
                    {
                        sb.Append($", {InheritList[i].Type}");
                    }
                }
            }

            // where T : struct,
            if (Constraints.Count > 0)
            {
                Constraints.ForEach((i, v) => {

                    sb.Append($" where {v.Type} : ");

                    v.Namelist.ForEach((j, vj) =>
                    {
                        if (j > 0)
                        {
                            sb.Append($", ");
                        }
                        sb.Append($"{vj}");
                    });

                });
            }


            return sb.ToString();
        }
    }

    // 继承关系 : IParentCls
    public class AstInheritInfo
    {
        public string Type { get; set; }
    }

    // 约束, where T : struct
    public class AstConstraintInfo
    {
        public string Type { get; set; }
        public List<string> _Namelist = new List<string>();
        public List<string> Namelist { get { return _Namelist; } }
    }

    // 成员变量，譬如：public int a; public int b {get;set;}
    public class AstMemberInfo
    {
        public string Type { get; set; }
        public string Name { get; set; }
        public List<AstAttributeInfo> AttributeList = new List<AstAttributeInfo>();
        // 是否是public的
        public List<string> Modifiers = new List<string>();

        public string GetMemberDesc()
        {
            var sb = new StringBuilder();

            // public
            sb.Append($"{AstInfoExtensions.GetPubDesc(Modifiers)}");

            // int[]
            sb.Append($" {Type}");
            
            sb.Append($" {Name}");

            return sb.ToString();
        }

        public bool HasAttribute(params string[] tokenList)
        {
            return AstInfoExtensions.HasAttribute(AttributeList, tokenList);
        }
    }

    // 修饰的属性，譬如[MyAttribute]
    public class AstAttributeInfo
    {
        public string Name { get; set; }
    }

    public class ASTInfo
    {
        public List<string> UsingList = new List<string>();
        public List<AstNamespace> NamespaceList = new List<AstNamespace>();

        public AstNamespace GetTopNamespace()
        {
            if (NamespaceList.Count == 0)
            {
                NamespaceList.Add(new AstNamespace());
            }
            return NamespaceList[NamespaceList.Count - 1];
        }
        public void PushNamespace(string n)
        {
            Debug.Assert(NamespaceList.Count > 0);

            GetTopNamespace().Name = n;
        }
    }
    
    public static class AstInfoExtensions
    {
        public static String Desc(this AstClassName.ClassType ct)
        {
            switch (ct)
            {
                case AstClassName.ClassType.Class:
                    return "class";
                case AstClassName.ClassType.Interface:
                    return "interface";
                case AstClassName.ClassType.Struct:
                    return "struct";

                default:
                    return "none";
            }
        }

        public static bool HasModifier(List<string> Modifiers, string modi)
        {
            return Modifiers.Contains(modi);
        }


        // public, private, protected
        public static string GetPubDesc(List<string> Modifiers)
        {
            if (HasModifier(Modifiers, "public"))
            {
                return "public";
            }

            if (HasModifier(Modifiers, "protected"))
            {
                return "protected";
            }

            if (HasModifier(Modifiers, "internal"))
            {
                return "internal";
            }

            return "private";
        }


        public static bool HasAttribute(List<AstAttributeInfo> AttributeList, params string[] tokenList)
        {
            if (tokenList == null)
            {
                return false;
            }

            foreach (var it in tokenList)
            {
                if (AttributeList.FindIndex(x => { return x.Name == it; }) < 0)
                {
                    return false;
                }
            }

            return true;
        }
    }


    // 收集信息
    public class CollectAstInfoWalker : CSharpSyntaxWalker
    {
        public ASTInfo AstInfo = new ASTInfo();

        public override string ToString()
        {
            return JsonConvert.SerializeObject(AstInfo, Formatting.Indented);
        }

        public override void VisitUsingDirective(UsingDirectiveSyntax node)
        {
            base.VisitUsingDirective(node);

            AstInfo.UsingList.Add(node.Name.ToString());
        }

        public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            base.VisitNamespaceDeclaration(node);
                       
            // 譬如namespace cell，返回cell
            //Console.WriteLine(node.Name);

            AstInfo.PushNamespace(node.Name.ToString());
        }

        void WalkAst(SyntaxList<AttributeListSyntax> AttrList, List<AstAttributeInfo> DstList)
        {
            if (AttrList != null)
            {
                foreach (var it in AttrList)
                {
                    foreach (var ait in it.Attributes)
                    {
                        var attr = new AstAttributeInfo();
                        DstList.Add(attr);
                        attr.Name = ait.Name.ToString();
                    }
                }
            }
        }

        void WalkAST(AstClassName.ClassType t, TypeDeclarationSyntax node)
        {
            var classInfo = new AstClassName();
            AstInfo.GetTopNamespace().ClassList.Add(classInfo);

            // 譬如struct A，返回的是A
            // 结构体名字
            //Console.WriteLine(node.Identifier);
            classInfo.Type = t;
            classInfo.Name = node.Identifier.ToString();

            // 其他信息
            foreach (var m in node.Modifiers)
            {
                classInfo.Modifiers.Add(m.ToString());
            }

            WalkAst(node.AttributeLists, classInfo.AttributeList);


            // 参数列表
            if (node.TypeParameterList != null)
            {
                foreach (var c in node.TypeParameterList.Parameters)
                {
                    //Console.WriteLine(c.Identifier);
                    classInfo.TypeParameterList.Add(c.ToString());
                }
            }

            // 约束 where T : struct
            foreach (var c in node.ConstraintClauses)
            {
                var cinfo = new AstConstraintInfo();
                classInfo.Constraints.Add(cinfo);

                cinfo.Type = c.Name.ToString();
                foreach (var cc in c.Constraints)
                {
                    cinfo.Namelist.Add(cc.ToString());
                }
            }

            // 继承的接口
            if (node.BaseList != null)
            {
                foreach (var b in node.BaseList.Types)
                {
                    var inheritInfo = new AstInheritInfo();
                    classInfo.InheritList.Add(inheritInfo);

                    if (b is SimpleBaseTypeSyntax sb)
                    {
                        inheritInfo.Type = sb.Type.ToString();
                    }
                    else
                    {
                        throw new Exception("unknown type.");
                    }

                    //Dump(b);
                }
            }

            foreach (var it in node.Members)
            {
                if (it is FieldDeclarationSyntax a)
                {
                    // 成员变量

                    var member = new AstMemberInfo();
                    classInfo.MemberList.Add(member);

                    // 类型
                    //Dump(a.Declaration.Type);
                    member.Type = a.Declaration.Type.ToString();

                    // 变量名
                    foreach (var jt in a.Declaration.Variables)
                    {
                        //Dump(jt);
                    }
                    Debug.Assert(a.Declaration.Variables.Count == 1);
                    member.Name = a.Declaration.Variables[0].Identifier.ToString();

                    // 修饰的属性
                    WalkAst(a.AttributeLists, member.AttributeList);

                    // 其他信息
                    foreach (var m in a.Modifiers)
                    {
                        member.Modifiers.Add(m.ToString());
                    }
                }
                else if (it is PropertyDeclarationSyntax pro)
                {
                    // 成员属性变量

                    var member = new AstMemberInfo();
                    classInfo.PropertyList.Add(member);

                    // 类型
                    //Dump(a.Declaration.Type);
                    member.Type = pro.Type.ToString();

                    // 变量名
                    member.Name = pro.Identifier.ToString();

                    // 修饰的属性
                    WalkAst(pro.AttributeLists, member.AttributeList);

                    // 其他信息
                    foreach (var m in pro.Modifiers)
                    {
                        member.Modifiers.Add(m.ToString());
                    }
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

                    // modifiers，譬如static, public,protected等
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

            WalkAST(AstClassName.ClassType.Struct, node);
        }


        public override void VisitInterfaceDeclaration(InterfaceDeclarationSyntax node)
        {
            base.VisitInterfaceDeclaration(node);
            
            WalkAST(AstClassName.ClassType.Interface, node);
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            base.VisitClassDeclaration(node);

            WalkAST(AstClassName.ClassType.Class, node);
        }

        void Dump(object o)
        {

        }
    }
}
