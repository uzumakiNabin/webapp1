using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class Department
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Required")]
        public string DepartmentName { get; set; }
    }
}
