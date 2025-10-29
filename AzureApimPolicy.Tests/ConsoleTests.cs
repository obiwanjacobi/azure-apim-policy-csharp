using System.Diagnostics;
using Xunit.Abstractions;

namespace AzureApimPolicy.Tests;

public class ConsoleTests
{
    private readonly ITestOutputHelper _output;

    public ConsoleTests(ITestOutputHelper output)
        => _output = output;

    [Fact]
    public void RunTool()
    {
        var toolInfo = new ProcessStartInfo()
        {
            Arguments = "AzureApimPolicy.Tests.dll -o ./policyXml",
            FileName = "Jacobi.Azure.ApiManagement.Policy.exe",
            RedirectStandardError = true,
            RedirectStandardOutput = true,
        };
        var tool = Process.Start(toolInfo);
        Assert.NotNull(tool);

        bool error = false;
        tool.OutputDataReceived += (sender, args) => { if (args.Data is not null) _output.WriteLine(args.Data); };
        tool.ErrorDataReceived += (sender, args) => { if (args.Data is not null) { _output.WriteLine(args.Data); error = true; } };
        tool.BeginErrorReadLine();
        tool.BeginOutputReadLine();
        tool.WaitForExit();

        Assert.False(error);
    }
}
