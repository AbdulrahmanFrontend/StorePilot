using System.ComponentModel.DataAnnotations;

namespace StorePilot.Domain.Entities;

public sealed class ProductGroup
{
    public int Id { get; set; }

    [Required]
    [MaxLength(20)]
    public string Name { get; set; } = null!;

    public ICollection<Product>? Products { get; set; }
}
