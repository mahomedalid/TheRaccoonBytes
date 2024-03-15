using Microsoft.SemanticKernel;

namespace PostGenerator;

public class PostGeneratorService
{
    private readonly KernelFunction _promptFunction;

    private readonly Kernel _kernel;

    public PostGeneratorService(Kernel kernel, KernelFunction promptFunction)
    {
        _kernel = kernel;
        _promptFunction = promptFunction;
    }

    public async Task<string?> GeneratePost(string persona, string topic, string style)
    {
        var context = new KernelArguments
        {
            { "Topic", topic },
            { "Persona", persona },
            { "Style", style }
        };

        var result = await _promptFunction.InvokeAsync(_kernel, context);

        return result?.ToString();
    }
}