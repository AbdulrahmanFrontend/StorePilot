using System.ComponentModel.DataAnnotations;

namespace StorePilot.Domain.Entities;

public sealed class Unit
{
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string Name { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; } = true;

    [MaxLength(100)]
    public string? Notes { get; set; }

    public ICollection<Product>? Products { get; set; }
}
