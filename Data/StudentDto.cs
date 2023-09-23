using System.ComponentModel.DataAnnotations;

namespace MOVIEAPI.Data{
public class StudentDto
    {
        [Required]
        public string? Name { get; set; }
        [Required]
        public string? Username{ get; set; }
        [Required]
        public string? Password{ get; set; }
    }
}