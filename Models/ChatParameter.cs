namespace Promptune.Models;

public class ChatParameter
{
    public int No { get; set; }
    public string? Model { get; set; }
    public string? SystemMessage { get; set; }
    public string? PromptFormat { get; set; }
    public string? PromptParameter1 { get; set; }
    public string? PromptParameter2 { get; set; }
    public string? PromptParameter3 { get; set; }
    public string? UserMessage { get; set; }
    public string? Result { get; set; }
    public string? Status { get; set; }
    public int TokenCount { get; set; }
    public TimeSpan? ExecutionTime { get; set; }
    public float Temperature { get; set; }
    public float TopP { get; set; }
    public int MaxTokens { get; set; }
    public int? RequestTokens { get; set; }
    public int? ResponseTokens { get; set; }
}