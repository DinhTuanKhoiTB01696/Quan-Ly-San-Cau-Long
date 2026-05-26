namespace BadmintonApp.Application.DTOs.Feedback;

public class FeedbackDto
{
    public int Id { get; set; }
    public bool IsHelpful { get; set; }
    public string MissingFeature { get; set; } = string.Empty;
    public string WantedCourt { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class CreateFeedbackDto
{
    public bool IsHelpful { get; set; }
    public string MissingFeature { get; set; } = string.Empty;
    public string WantedCourt { get; set; } = string.Empty;
}
