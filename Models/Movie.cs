using System.ComponentModel.DataAnnotations;

namespace APICLASS.Models{
public class Movie
{[Key]
    public Guid id { get; set; }
    public string? Title { get; set; }
    public string? Genere { get; set; }
    public DateTime RebaseDate { get; set; }
}

}