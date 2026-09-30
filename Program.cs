using LocalAI.Playground.Providers;
using LocalAI.Playground.Services;

using var httpClient = new HttpClient
{
    BaseAddress = new Uri("http://localhost:11434"),
    Timeout = TimeSpan.FromMinutes(5)
};

Console.WriteLine("Local AI");
Console.WriteLine();

//
// Discover installed models
//

var models = await OllamaProvider.GetModelsAsync(httpClient);

if (models.Count == 0)
{
    Console.WriteLine("No Ollama models installed.");
    return;
}

Console.WriteLine("Available models:");
Console.WriteLine();

for (var i = 0; i < models.Count; i++)
{
    var model = models[i];

    var sizeGb = model.Size / 1024d / 1024d / 1024d;

    Console.WriteLine(
        $"{i + 1}. {model.Name} ({sizeGb:F1} GB)");
}

Console.WriteLine();

//
// Model selection
//

int selectedIndex;

while (true)
{
    Console.Write("Select model: ");

    var input = Console.ReadLine();

    if (int.TryParse(input, out selectedIndex) &&
        selectedIndex >= 1 &&
        selectedIndex <= models.Count)
    {
        break;
    }

    Console.WriteLine("Invalid selection.");
}

var selectedModel = models[selectedIndex - 1];

Console.WriteLine();
Console.WriteLine($"Using: {selectedModel.Name}");
Console.WriteLine();

//
// Create AI provider
//

IAiProvider ai = new OllamaProvider(
    httpClient,
    selectedModel.Name);

Console.WriteLine("Commands: clear | bye");
Console.WriteLine();

//
// Chat
//

while (true)
{
    Console.Write("You: ");

    var prompt = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(prompt))
        continue;

    if (prompt.Equals(
        "bye",
        StringComparison.OrdinalIgnoreCase))
    {
        break;
    }

    if (prompt.Equals(
        "clear",
        StringComparison.OrdinalIgnoreCase))
    {
        ai.ResetConversation();

        Console.WriteLine("Conversation cleared.");
        Console.WriteLine();

        continue;
    }

    try
    {
        Console.WriteLine();
        Console.Write("AI: ");

        await foreach (var chunk in ai.ChatAsync(prompt))
        {
            Console.Write(chunk);
        }

        Console.WriteLine();
        Console.WriteLine();
    }
    catch (Exception ex)
    {
        Console.WriteLine();
        Console.WriteLine($"Error: {ex.Message}");
        Console.WriteLine();
    }
}