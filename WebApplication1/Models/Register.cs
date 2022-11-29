using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Register
    {
        [Required(ErrorMessage = "Required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public string UserName { get; set; }

        [Required(ErrorMessage = "Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Required")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        public string Department { get; set; }

    }
}
