using System.ComponentModel.DataAnnotations;

namespace Bumbo.Web.Models.User
{
    public class CreateViewModel : UserViewModel
    {
        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }
    }
}