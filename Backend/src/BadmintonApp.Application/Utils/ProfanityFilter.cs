namespace BadmintonApp.Application.Utils;

public static class ProfanityFilter
{
    // Danh sách các từ cấm cơ bản (có thể mở rộng thêm)
    private static readonly HashSet<string> BadWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "đù", "đụ", "cặc", "lồn", "buồi", "đĩ", "điếm", "chó đẻ", "mẹ mày", "đm", "vcl", "vl", "địt", "cac", "lon", "buoi", "fuck", "shit"
    };

    public static bool ContainsProfanity(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return false;

        var words = text.Split(new[] { ' ', ',', '.', '!', '?', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var word in words)
        {
            if (BadWords.Contains(word))
            {
                return true;
            }
        }
        return false;
    }
}
