using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mvcproject.Models
{

    public class Login
    {
 
        [Required(ErrorMessage = "username must not be Empty")]
        public string Email { get; set; }

        [Required(ErrorMessage = "password must not be Empty")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; internal set; }
    }
}
