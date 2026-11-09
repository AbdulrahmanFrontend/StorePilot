using System.ComponentModel.DataAnnotations;

namespace StorePilot.Api.Models;

public sealed class UpdateStoreSettingsRequestModel
{
    [Required]
    [StringLength(100)]
    [RegularExpression(@".*\S.*")]
    public string ShopName { get; set; } = null!;

    [Required]
    [StringLength(100)]
    [RegularExpression(@".*\S.*")]
    public string OwnerName { get; set; } = null!;
}
