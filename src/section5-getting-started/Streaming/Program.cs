using Azure.AI.OpenAI;
using Azure.Identity;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

// 1. Define the variables we extracted from Microsoft Foundry
var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-5-mini";

// 2. Instantiate the universal chat client
IChatClient chatClient = new AzureOpenAIClient(
        new Uri(endpoint),
        new AzureCliCredential())
    .GetChatClient(deploymentName)
    .AsIChatClient();

// 3. Define the Agent's Anatomy
AIAgent supportAgent = chatClient.AsAIAgent(
    name: "NetworkSupport",
    instructions: "You are a Tier 1 IT Support Agent. Your answers must be concise, professional, and limited strictly to troubleshooting network and VPN connectivity. Keep your answers brief."
);

Console.WriteLine($"Agent '{supportAgent.Name}' is online.\n");

// 4. Execute the Agent
string userIssue = "I am getting a DNS resolution error when connecting to the corporate VPN from a coffee shop. Keep it brief";
Console.WriteLine($"User: {userIssue}\n");

//// non-streaming response
//AgentResponse response = await supportAgent.RunAsync(userIssue);
//// The entire response is available at once
//Console.WriteLine(response.Text);

// streaming response
await foreach (AgentResponseUpdate update in supportAgent.RunStreamingAsync(userIssue))
{
    // Print each token to the console/UI exactly as it is generated
    Console.Write(update.Text);
}
