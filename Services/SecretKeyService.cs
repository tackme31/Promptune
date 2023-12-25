namespace Promptune.Services;

public class SecretKeyService
{
    private static readonly string SecretKeyFileName = ".promptune";
    public async Task<(string endpoint, string key)> GetSecretKey()
    {
        var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var filePath = Path.Combine(homeDir, SecretKeyFileName);
        if (!File.Exists(filePath))
        {
            return (string.Empty, string.Empty);
        }

        using var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
        using var reader = new StreamReader(stream, System.Text.Encoding.UTF8);

        var text =  await reader.ReadToEndAsync();
        return text.Contains('|')
            ? (text.Split('|')[0], text.Split('|')[1])
            : (string.Empty, string.Empty);
    }

    public async Task SaveSecretKey(string? endpoint, string? key)
    {
        var homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        var filePath = Path.Combine(homeDir, SecretKeyFileName);

        await File.WriteAllTextAsync(filePath, $"{endpoint?.Trim()}|{key?.Trim()}", System.Text.Encoding.UTF8);
    }
}
