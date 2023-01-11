namespace Infer.Services;

public class InferenceResult
{
    public InferenceResult(int digit, float[] probabilities)
    {
        Digit = digit;
        Probabilities = probabilities;
    }

    public int Digit { get; }
    public float[] Probabilities { get; }
}