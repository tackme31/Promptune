namespace Promptune.Models;

public class PromptOptions
{
    public int BatchCount { get; set; }
    public float Temperature { get; set; }
    public float TopP { get; set; }
    public string[]? Models { get; set; }
    public PromptPattern[]? PromptPatterns { get; set; }
    public int MaxTokens { get; set; }
}