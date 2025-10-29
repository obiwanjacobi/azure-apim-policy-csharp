using System.Reflection;

namespace Jacobi.Azure.ApiManagement.Policy;


public sealed class Program
{
    public static int Main(string[] args)
    {
        try
        {
            WriteBanner();
            if (args.Length == 0)
            {
                WriteHelp();
                return 0;
            }

            var commandLine = ParseCommandLine(args);
            Console.WriteLine($"Generating Policy XML for '{commandLine.AssemblyPath}' to '{commandLine.OutFolder}'...");

            var generator = new PolicyXmlGenerator(commandLine.OutFolder);
            generator.GenerateAll(commandLine.AssemblyPath);

            Console.WriteLine("Done.");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }

    private static void WriteBanner()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "<not available>";
        Console.WriteLine($"Generate Azure API Management Policy XML v{version}");
    }
    private static void WriteHelp()
    {
        Console.WriteLine();
        Console.WriteLine("<assembly-file>");
        Console.WriteLine("<assembly-file> -o <output-folder>");
    }

    private static CommandLine ParseCommandLine(string[] args)
    {
        var assemblyPath = Path.GetFullPath(args[0]);
        if (args.Length == 1)
            return new(assemblyPath, Directory.GetCurrentDirectory());
        if (args.Length >= 3)
        {
            if (args[1] != "-o")
                throw new ArgumentException("Expected parameter '-o' not found.");

            if (args.Length > 3)
                Console.WriteLine($"Ignoring: {String.Join(",", args.Skip(3))}");

            var outputPath = Path.GetFullPath(args[2]);
            return new CommandLine(assemblyPath, outputPath);
        }

        throw new ArgumentException("Could not parse command line arguments.");
    }

    private record CommandLine(string AssemblyPath, string OutFolder)
    { }
}