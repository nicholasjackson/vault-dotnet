using System.ComponentModel.DataAnnotations;

namespace VaultDotNet.Models;

public class Fighter
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ImageURL { get; set; }
    public string? Hates { get; set; }
    public string? Likes { get; set; }
    public int? Height { get; set; }
    public int? Weight { get; set; }
    [DataType(DataType.Date)]
    public DateTime ReleaseDate { get; set; }
}