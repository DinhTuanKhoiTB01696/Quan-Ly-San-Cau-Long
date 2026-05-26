namespace BadmintonApp.Domain.Entities;

public class Feedback
{
    public int Id { get; set; }
    public bool IsHelpful { get; set; }
    public string MissingFeature { get; set; } = string.Empty;
    public string WantedCourt { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
