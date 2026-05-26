using BadmintonApp.Application.DTOs.Feedback;

namespace BadmintonApp.Application.Interfaces;

public interface IFeedbackService
{
    Task<FeedbackDto> CreateAsync(CreateFeedbackDto createDto);
    Task<IEnumerable<FeedbackDto>> GetAllAsync();
}
