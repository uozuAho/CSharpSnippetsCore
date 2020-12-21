using System;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.IO;
using System.Threading.Tasks;

namespace ConsoleAppExample
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var rootCommand = new RootCommand
            {
                new Option<int>(
                    "--int-option",
                    getDefaultValue: () => 42,
                    description: "An option whose argument is parsed as an int"),
                new Option<bool>(
                    "--bool-option",
                    "An option whose argument is parsed as a bool"),
                new Option<FileInfo>(
                    "--file-option",
                    "An option whose argument is parsed as a FileInfo"),
                BuildDoCommand()
            };

            rootCommand.Description = "My sample app";

            // Note that the parameters of the handler method are matched according to the names of the options
            rootCommand.Handler = CommandHandler.Create<int, bool, FileInfo>((intOption, boolOption, fileOption) =>
            {
                Console.WriteLine($"The value for --int-option is: {intOption}");
                Console.WriteLine($"The value for --bool-option is: {boolOption}");
                Console.WriteLine($"The value for --file-option is: {fileOption?.FullName ?? "null"}");
            });

            return await rootCommand.InvokeAsync(args);
        }

        private static Command BuildDoCommand()
        {
            var command = new Command("do")
            {
                new Option<string>("--message")
            };
            command.Handler = CommandHandler.Create<string>(message =>
            {
                Console.WriteLine($"do called with message '{message}'");
            });
            return command;
        }
    }
}
