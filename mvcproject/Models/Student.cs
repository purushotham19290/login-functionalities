using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace mvcproject.Models
{
 //   [Table("Student1")]
    public class Student
    { 
        [Key]
        public int StdId { get; set; }
        [Required(ErrorMessage ="Name Must Not Be Empty")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Address Must Not Be Empty")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Gender Must Not Be Empty")]
        public string Gender { get; set; }

        [Required(ErrorMessage = "Phone Must Not Be Empty")]

        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$",
         ErrorMessage = "Entered phone format is not valid.")]
        public long Phoneno { get; set; }


        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$",
        ErrorMessage = "Entered phone format is not valid.")]
        [Required(ErrorMessage = "Email Must Not Be Empty")]
        public string Email { get; set; }

        [Required(ErrorMessage = "StdFee Must Not Be Empty")]
        public decimal StdFee { get; set; }
    }
}