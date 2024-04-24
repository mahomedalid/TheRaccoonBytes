using Microsoft.SemanticKernel;

namespace PostGeneratorSdk;

public class PostGeneratorService
{
    private readonly KernelFunction _promptFunction;

    private readonly Kernel _kernel;

    public static PostGeneratorService Create(Kernel kernel)
    {
        string promptTemplate = EmbeddedResource.Read("_prompts.generatePost.skprompt.txt");
        return new PostGeneratorService(kernel, kernel.CreateFunctionFromPrompt(promptTemplate));
    }

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