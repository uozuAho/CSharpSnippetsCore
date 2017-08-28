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
    // shameless copy from https://stackoverflow.com/a/37739636
    public class CompilerExample
    {
        public static void Run()
        {
            string code = 
@"using System;

namespace User
{
    public static class MyClass
    {
        public static double AddDoubles(double a, double b)
        {
            return a + b;
        }
    }
}";
            var syntaxTree = CSharpSyntaxTree.ParseText(code);

            var references = new MetadataReference[]
            {
                MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location),
            }; 

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
                    var assembly = context.LoadFromStream(ms);

                    var compiledClass = assembly.GetType("User.MyClass");
                    var compiledMethod = compiledClass.GetMethod("AddDoubles");

                    var methodResult = compiledMethod.Invoke(null, new object[] {1, 2});

                    Console.WriteLine("result: " + methodResult);
                }
            }
        }
    }
}