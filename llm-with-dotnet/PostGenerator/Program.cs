using System.CommandLine.Parsing;
using System.CommandLine;
using System;

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

var createPostCommand = new Command("create-post", "Create new post");

createPostCommand.AddOption(personaOption);
createPostCommand.AddOption(topicOption);
createPostCommand.AddOption(styleOption);

createPostCommand.SetHandler((persona, topic, option) =>
{
    Console.WriteLine($"Creating new post with persona: {persona}, topic: {topic}, style: {option}");

    throw new NotImplementedException();
}, personaOption, topicOption, styleOption);

rootCommand.AddCommand(createPostCommand);

var result = await rootCommand.InvokeAsync(args);

return result;
