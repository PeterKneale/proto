using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;

namespace Infer.Services;

public class InferenceService
{
    public const string Model = "Model\\model.onnx";
    
    private readonly InferenceSession _session;

    public InferenceService(InferenceSession session)
    {
        _session = session;
    }
    
    public InferenceResult Infer(long[] data)
    {
        int[] dimensions = { 1, 64 };
        var input = new DenseTensor<long>(data, dimensions);
        using var result = _session.Run(new List<NamedOnnxValue> 
        { 
            NamedOnnxValue.CreateFromTensor("image", input) 
        });
        
        var tensorResult = result.ElementAt(0).Value as DenseTensor<long>;
        var tensorProbabilities = result.ElementAt(1).Value as DenseTensor<float>;

        var digit = (int) tensorResult.GetValue(0);
        var probabilities = tensorProbabilities.Buffer.ToArray();
        
        return new InferenceResult(digit, probabilities);
    }
}