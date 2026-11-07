using System.ComponentModel.DataAnnotations;

namespace StorePilot.Domain.Entities;

public sealed class Product
{
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = null!;

    [MaxLength(20)]
    public string? Code { get; set; }

    public decimal SalePrice { get; set; } = 0;
    public decimal PurchasePrice { get; set; } = 0;
    public decimal MinQuantity { get; set; } = 0;
    public decimal CurrentQuantity { get; set; }

    public int UnitId { get; set; }
    public Unit Unit { get; set; } = null!;

    public int GroupId { get; set; }
    public ProductGroup Group { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; } = true;

    [MaxLength(100)]
    public string? Notes { get; set; }
}
