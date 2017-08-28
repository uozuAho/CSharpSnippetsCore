using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace CSharpSnippetsCore.Compile
{
    public class CompilerExample
    {
        public static void Run()
        {
            var result = ExecuteDoubleExpression("2 * 3.1415 / 17");
            Console.WriteLine("result: " + result);
        }

        private static double ExecuteDoubleExpression(string expr)
        {
            string code = 
                @"using System;

                namespace User
                {
                    public static class ExpressionExecutor
                    {
                        public static double ExecuteDouble()
                        {
                            return __DOUBLE__EXPR__;
                        }
                    }
                }".Replace("__DOUBLE__EXPR__", expr);

            var assembly = CompileAssembly(code, 
                MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location));

            var executor = assembly.GetType("User.ExpressionExecutor");
            var executeDouble = executor.GetMethod("ExecuteDouble");

            return (double) executeDouble.Invoke(null, null);
        }

        private static Assembly CompileAssembly(string code, params MetadataReference[] references)
        {
            var syntaxTree = CSharpSyntaxTree.ParseText(code);

            var compilation = CSharpCompilation.Create("Function.dll",
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

            var message = new StringBuilder();

            using (var ms = new MemoryStream())
            {
                var result = compilation.Emit(ms);

                if (!result.Success)
                {
                    var failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (Diagnostic diagnostic in failures)
                    {
                        message.AppendFormat("{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                    }
                    throw new InvalidOperationException("compilation error: " + message.ToString());
                }
                else
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    var context = AssemblyLoadContext.Default;
                    return context.LoadFromStream(ms);
                }
            }
        }
    }
}