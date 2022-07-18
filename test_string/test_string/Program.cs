using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace test_string
{
    class Program
    {
        static void Main(string[] args)
        {
            var keep = "".ToArray();
            Console.WriteLine(keep);

            var s = Proc("Abc中文def_xx@123");
            Console.WriteLine(s);
        }

        public static string Proc(string s)
        {
            //if (!StringUtils.IsSnakeCase(s))
            {
                // 譬如 CamelCase -> camelcase
                var arr = s.ToCharArray();
                var sb = new StringBuilder();
                foreach (var c in arr)
                {
                    if (c >= 'A' && c <= 'Z')
                    {
                        var c2 = c - 'A' + 'a';
                        sb.Append((char)c2);
                    }
                    else if (c >= 'a' && c <= 'z')
                    {
                        sb.Append(c);
                    }
                    else if (c >= '0' && c <= '9')
                    {
                        sb.Append(c);
                    }
                    else if (c == '@')
                    {
                        sb.Append(c);
                    }
                    else if (c == '_')
                    {
                        sb.Append(c);
                    }
                    else
                    {
                        sb.Append('_');
                    }
                }

                return sb.ToString();
            }

            return "";
        }
    }
}
