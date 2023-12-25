namespace Promptune.Models;

public class PromptPattern
{
    public string? Format { get; set; }
    public string[]? SystemMessages { get; set; }
    public string[]? Parameters1 { get; set; }
    public string[]? Parameters2 { get; set; }
    public string[]? Parameters3 { get; set; }
}