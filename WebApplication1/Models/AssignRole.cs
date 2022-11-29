using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class AssignRole
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AssignId { get; set; }

        public int UserId { get; set; }

        public string UserName { get; set; }

        public int RoleId { get; set; }

        [Required(ErrorMessage = "Required")]
        public string RoleName { get; set; }

    }
}
