using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class AssignDepartment
    {
        //[Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        //public int AssignId { get; set; }

        //public int UserId { get; set; }
        [Required(ErrorMessage = "Required")]
        public string UserName { get; set; }

        //public int RoleId { get; set; }

        [Required(ErrorMessage = "Required")]
        public string DepartmentName { get; set; }

    }
}
