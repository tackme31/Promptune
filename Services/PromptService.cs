namespace Promptune.Services;

using Promptune.Models;
using Azure.AI.OpenAI;
using Azure;
using Microsoft.DeepDev;

public class PromptService
{
    private readonly SecretKeyService _secretKeyService;
    private ITokenizer? _tokenizer;

    public PromptService(SecretKeyService secretKeyService)
    {
        _secretKeyService = secretKeyService;
        _ = LoadTokenizer();
    }

    private async Task LoadTokenizer()
    {
        _tokenizer = await TokenizerBuilder.CreateByModelNameAsync("gpt-4");
    }

    public async Task<List<ChatParameter>> CreateChatParameters(PromptOptions options)
    {
        var result = new List<ChatParameter>();
        var parameters = options.Models
            .SelectMany(_ => options.PromptPatterns, (model, prompt) => (model, prompt))
            .SelectMany(p => CreateParameterCombination(p.model, p.prompt, options.BatchCount));
        foreach (var (parameter, index) in parameters.Select((p, i) => (p, i)))
        {
            parameter.No = index;
            parameter.Temperature = options.Temperature;
            parameter.TopP = options.TopP;
            parameter.MaxTokens = options.MaxTokens;
            parameter.RequestTokens = await GetTokenCount(parameter.SystemMessage + parameter.UserMessage);
            parameter.Status = "waiting";
            result.Add(parameter);
        }

        return result;
    }

    public IEnumerable<ChatParameter> CreateParameterCombination(string model, PromptPattern prompt, int batchCount)
    {
        var parameter1 = prompt.Parameters1 ?? new [] { string.Empty };
        var parameter2 = prompt.Parameters2 ?? new [] { string.Empty };
        var parameter3 = prompt.Parameters3 ?? new [] { string.Empty };
        var systemMessages = prompt.SystemMessages ?? new [] { string.Empty };
        var batch = Enumerable.Range(0, batchCount);
        return
            from p1 in parameter1
            from p2 in parameter2
            from p3 in parameter3
            from sm in systemMessages
            from _ in batch
            select new ChatParameter
            {
                Model = model,
                SystemMessage = sm,
                PromptFormat = prompt.Format,
                PromptParameter1 = p1,
                PromptParameter2 = p2,
                PromptParameter3 = p3,
                UserMessage = string.Format(prompt.Format, p1, p2, p3)
            };
    }

    public async Task<string?> GetChatCompletions(ChatParameter pattern, int timeOut)
    {
        var (endpoint, key) = await _secretKeyService.GetSecretKey();
        var client = new OpenAIClient(
            new Uri(endpoint),
            new AzureKeyCredential(key)
        );

        var options = new ChatCompletionsOptions
        {
            Messages = {},
            Temperature = pattern.Temperature,
            NucleusSamplingFactor = pattern.TopP,
            MaxTokens = pattern.MaxTokens,
            FrequencyPenalty = 0,
            PresencePenalty = 0,
        };

        if (!string.IsNullOrWhiteSpace(pattern.SystemMessage))
        {
            options.Messages.Add(new ChatMessage(ChatRole.System, pattern.SystemMessage));
        }
        options.Messages.Add(new ChatMessage(ChatRole.User, pattern.UserMessage));

        using var tokenSouce = new CancellationTokenSource();
        tokenSouce.CancelAfter(timeOut);

        try
        {
            var completions = await client.GetChatCompletionsAsync(pattern.Model, options, tokenSouce.Token);
            return completions.Value.Choices[0].Message.Content;
        }
        catch (OperationCanceledException)
        {
            return null;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<int> GetTokenCount(string? message)
    {
        if (_tokenizer is null)
        {
            await LoadTokenizer();
        }

        if (string.IsNullOrWhiteSpace(message))
        {
            return 0;
        }

        var encoded = _tokenizer.Encode(message, Array.Empty<string>());
        return encoded.Count;
    }
}