using BadmintonApp.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace BadmintonApp.Application.DTOs.Courts;

public class CourtDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Area Area { get; set; }
    public string Address { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Ceiling { get; set; } = string.Empty;
    public string Light { get; set; } = string.Empty;
    public string Surface { get; set; } = string.Empty;
    public double Rating { get; set; }
    public string Phone { get; set; } = string.Empty;
    public bool IsFeatured { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
}

public class CourtCreateDto
{
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public Area Area { get; set; }
    [Required]
    public string Address { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Ceiling { get; set; } = string.Empty;
    public string Light { get; set; } = string.Empty;
    public string Surface { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
}
