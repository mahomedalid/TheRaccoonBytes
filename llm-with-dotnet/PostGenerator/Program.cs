using System.CommandLine.Parsing;
using System.CommandLine;
using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.SemanticKernel;
using PostGeneratorSdk;

var serviceCollection = new ServiceCollection();

ConfigureServices(serviceCollection, args);

using var serviceProvider = serviceCollection.BuildServiceProvider();

var debugOption = new Option<bool>("--debug")
{
    Description = "Enable debug logging"
};

var personaOption = new Option<string>("--persona")
{
    Description = "The persona to use",
    IsRequired = true
};

var topicOption = new Option<string>("--topic")
{
    Description = "The topic to use",
    IsRequired = true
};

var styleOption = new Option<string>("--style")
{
    Description = "The style to use",
    IsRequired = true
};

var rootCommand = new RootCommand();

rootCommand.AddGlobalOption(debugOption);

var createPostCommand = new Command("createpart-post", "Create new post");

createPostCommand.AddOption(personaOption);
createPostCommand.AddOption(topicOption);
createPostCommand.AddOption(styleOption);

var loggerFactory = serviceProvider.GetRequiredService<ILoggerFactory>();

createPostCommand.SetHandler(async (persona, topic, option) =>
{
    var logger = loggerFactory.CreateLogger<Program>();

    logger?.LogDebug($"Command requested for {persona} {topic} {option}");

    var postGeneratorService = serviceProvider.GetRequiredService<PostGeneratorService>();

    var result = await postGeneratorService.GeneratePost(persona, topic, option);

    logger?.LogInformation(result);
}, personaOption, topicOption, styleOption);

rootCommand.AddCommand(createPostCommand);

var result = await rootCommand.InvokeAsync(args);

return result;

static void ConfigureServices(ServiceCollection services, string[] args)
{
    services.AddLogging(builder =>
    {
        builder.AddSimpleConsole(options =>
        {
            options.IncludeScopes = true;
            options.SingleLine = true;
            options.TimestampFormat = "hh:mm:ss ";
        });

        if (args.Any("--debug".Contains))
        {
            builder.SetMinimumLevel(LogLevel.Debug);
        }
    }).AddSingleton((sp) => {
        var builder = Kernel.CreateBuilder();

        builder.AddAzureOpenAIChatCompletion(
                  Environment.GetEnvironmentVariable("AZURE_OPENAI_MODEL")!,
                  Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT")!,
                  Environment.GetEnvironmentVariable("AZURE_OPENAI_KEY")!);

        return builder.Build();
    }).AddSingleton((sp) => {
        var kernel = sp.GetRequiredService<Kernel>();

        var kernelFunctions = kernel.CreatePluginFromPromptDirectory("_prompts");

        return new PostGeneratorService(kernel, kernelFunctions["generatePost"]);
    });
}
