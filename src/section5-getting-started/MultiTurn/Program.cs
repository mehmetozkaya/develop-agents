using Azure.Identity;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

// 1. Define the variables we extracted from Microsoft Foundry
var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-5-mini";

IChatClient chatClient = new AzureOpenAIClient(
        new Uri(endpoint),
        new AzureCliCredential())
    .GetChatClient(deploymentName)
    .AsIChatClient();

// 2. Create the Agent
AIAgent agent = chatClient.AsAIAgent(
    name: "HistoryBuff",
    instructions: "You are a helpful history teacher. You answer questions and help students make connections between events."
);

// 3. Create the Session (The Memory Container)
// This object will accumulate the conversation history.
AgentSession session = await agent.CreateSessionAsync();

Console.WriteLine("History Teacher is online. Type 'exit' to quit.\n");

// 4. The Conversation Loop
while (true)
{
    Console.Write("User: ");
    string? input = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(input) || input.ToLower() == "exit") break;

    // We pass the 'session' into RunAsync.
    // The framework automatically appends the user's input to this session,
    // sends the full history to the cloud, and appends the agent's response back to the session.
    AgentResponse response = await agent.RunAsync(input, session);

    Console.WriteLine($"Agent: {response.Text}\n");
}