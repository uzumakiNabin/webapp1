using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Role
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Required")]
        public string RoleName { get; set; }
    }
}
