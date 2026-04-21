using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using OpenAI.Chat;
using System.ClientModel;

// 1. Define the variables we extracted from Microsoft Foundry
var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
var model = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-5-mini";

// 2. Create the Agent using MAF
AIAgent agent = new AzureOpenAIClient(
        new Uri(endpoint),
        new AzureCliCredential())
    .GetChatClient(model)
    .AsAIAgent(instructions: "You are a friendly assistant. Keep your answers brief.");

//// Alternative initialization using API Key
//AIAgent agent = new AzureOpenAIClient(
//        new Uri(endpoint),
//        new ApiKeyCredential(apiKey))
//    .GetChatClient(model)
//    .AsAIAgent(instructions: "You are a friendly assistant. Keep your answers brief.");

// 3. Invoke the Agent
Console.WriteLine(await agent.RunAsync("What is the largest city in France?"));