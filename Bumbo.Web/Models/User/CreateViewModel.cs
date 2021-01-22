using System.ComponentModel.DataAnnotations;

namespace Bumbo.Web.Models
{
    public class CreateViewModel : UserViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}