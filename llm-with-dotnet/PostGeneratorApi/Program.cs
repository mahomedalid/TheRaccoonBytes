using PostGeneratorSdk;
using Microsoft.SemanticKernel;
using Microsoft.AspNetCore.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton((sp) => {
    var builder = Kernel.CreateBuilder();

    //Get this from the appsettings.json
    var configuration = sp.GetRequiredService<IConfiguration>();

    builder.AddAzureOpenAIChatCompletion(
                configuration["AZURE_OPENAI_MODEL"]!,
                configuration["AZURE_OPENAI_ENDPOINT"]!,
                configuration["AZURE_OPENAI_KEY"]!);

    return builder.Build();
}).AddSingleton((sp) => {
    var kernel = sp.GetRequiredService<Kernel>();

    return PostGeneratorService.Create(kernel);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Hello World!")
    .WithName("GetHelloWorld")
    .WithOpenApi();

app.MapPost("/publication", async (PostGeneratorService postGeneratorService, GeneratePostRequest request) =>
{
    var result = await postGeneratorService.GeneratePost(request.Persona, request.Topic, request.Style);

    return result;
}).WithName("PostPublication").WithOpenApi();

app.Run();
