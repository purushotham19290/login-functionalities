using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace mvcproject.Models
{
   // [Table("NewUser")]
    public class NewUser
    {
        [Key]
        [Required(ErrorMessage = "UserId Must Not be Empty")]
        public int UserId { get; set; }

        [Required(ErrorMessage ="Username Must Not be Empty")]   
        public string Username { get; set; }

        [Required(ErrorMessage ="Password Must Not Be Empty")]
        [DataType(DataType.Password)]    
        public string Password { get; set; }

        [Required(ErrorMessage ="Email Must Not Be Empty")]
        [DataType(DataType.EmailAddress, ErrorMessage = "E-mail is not valid")]
        public string Email { get; set; }

        [Required(ErrorMessage ="PhoneNo Must Not Be Empty")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$",
         ErrorMessage = "Entered phone format is not valid.")]    
        public string PhoneNo { get; set; }
        [Required(ErrorMessage ="DateOfBirth Must Not Be Empty")]

        [DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DOB  { get; set; }

        [Required(ErrorMessage ="Gender Must Not Be Empty")]
       public string Gender { get; set; }

        [Required(ErrorMessage = "Confirm Password Must Not Be Empty")]
        [DataType(DataType.Password)]
        [System.Web.Mvc.Compare( "Password",ErrorMessage ="Password and confiorm password should be same")]
        public string ConfirmPassword { get; set; }


        public Guid? ActivationCode{ get; set; }
        public bool IsEmailVerified { get; set; }
        public string ResetPasswordCode { get; set; }

       
    }
}