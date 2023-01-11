using Infer.Services;
using Microsoft.ML.OnnxRuntime;
using Xunit.Abstractions;

namespace Infer.Tests;

public class UnitTest1
{
    private readonly ITestOutputHelper _output;

    public UnitTest1(ITestOutputHelper output)
    {
        _output = output;
    }

    [Theory]
    [MemberData(nameof(Data))]
    public void Test1(int expectedDigit, long[] digit)
    {
        var path = Path.Combine(Environment.CurrentDirectory, InferenceService.Model);
        using var session = new InferenceSession(path);
        var service = new InferenceService(session);

        var result = service.Infer(digit);
        Display(result);
        Assert.Equal(expectedDigit, result.Digit);
    }

    public static IEnumerable<object[]> Data =>
        new List<object[]>
        {
            new object[] {0, TestData.Digit0},
            new object[] {1, TestData.Digit1},
            new object[] {2, TestData.Digit2},
            new object[] {3, TestData.Digit3},
        };

    private void Display(InferenceResult inferenceResult)
    {
        _output.WriteLine($"{inferenceResult.Digit}");
        int i = 0;
        foreach (var probability in inferenceResult.Probabilities)
        {
            _output.WriteLine($"{i} {probability}");
            i++;
        }
    }
}