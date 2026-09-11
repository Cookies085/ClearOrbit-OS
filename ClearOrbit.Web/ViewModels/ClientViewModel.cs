using System.ComponentModel.DataAnnotations;

namespace ClearOrbit.Web.ViewModels;

public class ClientViewModel
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Client name is required.")]
    [StringLength(200)]
    public string Name { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Industry { get; set; }

    [StringLength(500)]
    [Url(ErrorMessage = "Enter a valid URL (including https://)")]
    public string? Website { get; set; }

    public string? Notes { get; set; }
}

public class ClientListItem
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Industry { get; set; }
    public string? Website { get; set; }
    public int ContactCount { get; set; }
    public DateTime CreatedAt { get; set; }
}