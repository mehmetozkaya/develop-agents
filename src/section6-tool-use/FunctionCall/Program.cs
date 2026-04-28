using System.ComponentModel;
using Azure.Identity;
using Azure.AI.OpenAI;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using OpenAI.Chat;

// 1. Define the Enterprise Tool
public static class LogisticsTools
{
    [Description("Retrieves the current shipping status of an enterprise logistics order. Invoke this tool ONLY when the user explicitly provides an Order ID.")]
    public static string GetOrderStatus(
        [Description("The exact, case-sensitive alphanumeric order identifier. Format must be 'ORD-' followed by 5 digits (e.g., ORD-12345).")] string orderId)
    {
        // Simulating a deterministic database or external API call
        if (orderId == "ORD-12345") return "IN TRANSIT - Estimated Delivery Tomorrow";
        if (orderId == "ORD-99999") return "PENDING - Awaiting Stock Validation";
        return "UNKNOWN - Order ID not found in the logistics system.";
    }
}

class Program
{
    static async Task Main(string[] args)
    {
        // 2. Define the variables we extracted from Microsoft Foundry
        var endpoint = Environment.GetEnvironmentVariable("AZURE_OPENAI_ENDPOINT") ?? throw new InvalidOperationException("AZURE_OPENAI_ENDPOINT is not set.");
        var deploymentName = Environment.GetEnvironmentVariable("AZURE_OPENAI_DEPLOYMENT_NAME") ?? "gpt-5-mini";

        // 3. Initialize the Agent and Equip the Tool
        AIAgent agent = new AzureOpenAIClient(new Uri(endpoint), new AzureCliCredential())
            .GetChatClient(deploymentName)
            .AsAIAgent(
                name: "LogisticsSupport",
                instructions: "You are a customer support agent. Help users track their orders concisely.",
                // We dynamically generate the AITool and pass it into the agent's capabilities
                tools: [AIFunctionFactory.Create(LogisticsTools.GetOrderStatus)]
            );

        Console.WriteLine($"Agent '{agent.Name}' initialized. Ready to assist.\n");

        // --- Execution Pattern 1: Synchronous (Non-Streaming) ---
        Console.WriteLine("--- Synchronous Execution ---");
        string prompt1 = "What is the status of order ORD-12345?";
        Console.WriteLine($"User: {prompt1}");

        AgentResponse response = await agent.RunAsync(prompt1);
        Console.WriteLine($"Agent: {response.Text}\n");

        // --- Execution Pattern 2: Real-Time (Streaming) ---
        Console.WriteLine("--- Streaming Execution ---");
        string prompt2 = "I need an update on ORD-99999, please.";
        Console.WriteLine($"User: {prompt2}");
        Console.Write("Agent: ");

        await foreach (AgentResponseUpdate update in agent.RunStreamingAsync(prompt2))
        {
            Console.Write(update.Text);
        }
        Console.WriteLine("\n");

    }
}