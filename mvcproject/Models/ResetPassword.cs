using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace mvcproject.Models
{
    //[Table("ResetPassword")]
    public class ResetPassword
    {
      [Key]

        [Required(ErrorMessage = "Password Must Not Be Empty")]
        [DataType(DataType.Password)]
        public string  NewPassword { get; set; }

        [Required(ErrorMessage = "Confirm Password Must Not Be Empty")]
        [DataType(DataType.Password)]
        [System.Web.Mvc.Compare("NewPassword", ErrorMessage = "Password and confiorm password should be same")]
        public string ConfirmPassword { get; set; }

    }
}