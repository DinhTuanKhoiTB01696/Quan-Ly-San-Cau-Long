using BadmintonApp.Domain.Enums;

namespace BadmintonApp.Domain.Entities;

public class Court
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Area Area { get; set; }
    public string Address { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Ceiling { get; set; } = string.Empty; // Cao / Trung / Thấp
    public string Light { get; set; } = string.Empty;   // Tốt / Trung bình / Chói
    public string Surface { get; set; } = string.Empty;
    public double Rating { get; set; } = 5.0;
    public string Phone { get; set; } = string.Empty;
    public bool IsFeatured { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Match> Matches { get; set; } = new List<Match>();
}
