using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Register
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [MinLength(6, ErrorMessage = "Must be atleast 6 character long.")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Required")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public string? RoleName { get; set; }

        public string? DepartmentName { get; set; }
    }
}
