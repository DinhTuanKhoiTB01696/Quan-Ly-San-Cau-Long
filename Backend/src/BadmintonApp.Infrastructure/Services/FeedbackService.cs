using BadmintonApp.Application.DTOs.Feedback;
using BadmintonApp.Application.Interfaces;
using BadmintonApp.Domain.Entities;
using BadmintonApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BadmintonApp.Infrastructure.Services;

public class FeedbackService : IFeedbackService
{
    private readonly AppDbContext _context;

    public FeedbackService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<FeedbackDto> CreateAsync(CreateFeedbackDto createDto)
    {
        var feedback = new Feedback
        {
            IsHelpful = createDto.IsHelpful,
            MissingFeature = createDto.MissingFeature,
            WantedCourt = createDto.WantedCourt
        };

        _context.Feedbacks.Add(feedback);
        await _context.SaveChangesAsync();

        return new FeedbackDto
        {
            Id = feedback.Id,
            IsHelpful = feedback.IsHelpful,
            MissingFeature = feedback.MissingFeature,
            WantedCourt = feedback.WantedCourt,
            CreatedAt = feedback.CreatedAt
        };
    }

    public async Task<IEnumerable<FeedbackDto>> GetAllAsync()
    {
        return await _context.Feedbacks
            .OrderByDescending(f => f.CreatedAt)
            .Select(f => new FeedbackDto
            {
                Id = f.Id,
                IsHelpful = f.IsHelpful,
                MissingFeature = f.MissingFeature,
                WantedCourt = f.WantedCourt,
                CreatedAt = f.CreatedAt
            })
            .ToListAsync();
    }
}
