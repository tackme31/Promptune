namespace Promptune.Services;

using Promptune.Models;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration.Attributes;
using System.Globalization;

public class FileExportService
{
    public FileExportService()
    {
    }

    public async Task ExportChatResult(IEnumerable<ChatParameter> results, string path)
    {
        var chatResults = results
            .Select(result => new ChatResult
            {
                Model = result?.Model ?? string.Empty,
                SystemMessage = result?.SystemMessage ?? string.Empty,
                UserMessage = result?.UserMessage ?? string.Empty,
                Response = result?.Result ?? string.Empty,
                RequestTokens = result?.RequestTokens ?? 0,
                ResponseTokens = result?.ResponseTokens ?? 0,
                ResponseCharacters = result?.Result?.Length ?? 0,
                ExecutionTime = result?.ExecutionTime?.TotalSeconds ?? 0,
                Temperature = result?.Temperature ?? 0,
                TopP = result?.TopP ?? 0,
                MaxTokens = result?.MaxTokens ?? 0
            })
            .ToList();
        using var writer = new StreamWriter(path, append: false, encoding: System.Text.Encoding.UTF8);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
        await csv.WriteRecordsAsync(chatResults);
    }

    public async Task<List<string>> ImportPrompts(string path)
    {
        var config = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
        {
            HasHeaderRecord = false
        };
        using var reader = new StreamReader(path, encoding: System.Text.Encoding.UTF8);
        using var csv = new CsvReader(reader, config);

        var prompts = new List<string>();
        await foreach (var record in csv.GetRecordsAsync<ChatPrompt>())
        {
            if (string.IsNullOrWhiteSpace(record.Prompt))
            {
                continue;
            }

            prompts.Add(record.Prompt);
        }

        return prompts;
    }
}

public class ChatResult
{
    [Index(0), Name("Model")]
    public string Model { get; set; }
    [Index(1), Name("System message")]
    public string SystemMessage { get; set; }
    [Index(2), Name("User message")]
    public string UserMessage { get; set; }
    [Index(3), Name("Response")]
    public string Response { get; set; }
    [Index(4), Name("Request tokens")]
    public int RequestTokens { get; set; }
    [Index(5), Name("Response tokens")]
    public int ResponseTokens { get; set; }
    [Index(6), Name("Response Characters")]
    public int ResponseCharacters { get; set; }
    [Index(7), Name("Execution time")]
    public double ExecutionTime { get; set; }
    [Index(8), Name("Temperature")]
    public float Temperature { get; set; }
    [Index(9), Name("Top-p")]
    public float TopP { get; set; }
    [Index(10), Name("Max tokens")]
    public int MaxTokens { get; set; }
}

public class ChatPrompt
{
    public string Prompt { get; set; }
}