using System.ComponentModel.DataAnnotations;

namespace MOVIEAPI.Data{
public class StudentLoginDto
    {

        [Required]
        public string? Username{ get; set; }
        [Required]
        public string? Password{ get; set; }
    }
}