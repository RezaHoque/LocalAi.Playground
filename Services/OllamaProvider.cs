using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using LocalAI.Playground.Models;
using LocalAI.Playground.Providers;

namespace LocalAI.Playground.Services;

public class OllamaProvider : IAiProvider
{
    private readonly HttpClient _httpClient;
    private readonly List<OllamaMessage> _messages = [];

    private readonly string _model;

    private const string SystemPrompt = """
        You are a concise technical assistant.
        Answer directly and briefly.
        Do not repeat the user's question.
        Avoid unnecessary explanations.
        Prefer 1-3 short paragraphs unless more detail is requested.
        For coding questions, provide the code first and explain only when necessary.
        """;

    public OllamaProvider(
        HttpClient httpClient,
        string model = "qwen3:8b")
    {
        _httpClient = httpClient;
        _model = model;

        ResetConversation();
    }

    public async IAsyncEnumerable<string> ChatAsync(
        string prompt,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var userMessage = new OllamaMessage
        {
            Role = "user",
            Content = prompt
        };

        _messages.Add(userMessage);

        var request = new
        {
            model = _model,
            messages = _messages,
            stream = true
        };

        using var httpRequest = new HttpRequestMessage(
            HttpMethod.Post,
            "/api/chat")
        {
            Content = JsonContent.Create(request)
        };

        using var response = await _httpClient.SendAsync(
            httpRequest,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);

        response.EnsureSuccessStatusCode();

        await using var stream =
            await response.Content.ReadAsStreamAsync(cancellationToken);

        using var reader = new StreamReader(stream);

        var assistantResponse = "";

        while (true)
        {
            var line = await reader.ReadLineAsync(cancellationToken);

            if (line is null)
                break;

            if (string.IsNullOrWhiteSpace(line))
                continue;

            var chunk = JsonSerializer.Deserialize<OllamaResponse>(
                line,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (!string.IsNullOrEmpty(chunk?.Message?.Content))
            {
                assistantResponse += chunk.Message.Content;

                yield return chunk.Message.Content;
            }

            if (chunk?.Done == true)
                break;
        }

        _messages.Add(new OllamaMessage
        {
            Role = "assistant",
            Content = assistantResponse
        });
    }
    public static async Task<IReadOnlyList<OllamaModel>> GetModelsAsync(
    HttpClient httpClient,
    CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetFromJsonAsync<OllamaModelsResponse>(
            "/api/tags",
            cancellationToken);

        return response?.Models ?? [];
    }
    public void ResetConversation()
    {
        _messages.Clear();

        _messages.Add(new OllamaMessage
        {
            Role = "system",
            Content = SystemPrompt
        });
    }
}