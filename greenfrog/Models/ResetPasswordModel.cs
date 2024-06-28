using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace greenfrog.Models
{
    public class ResetPasswordModel
    {
        [Required(ErrorMessage ="Mat khau moi phai duoc khai bao", AllowEmptyStrings = false)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Mat khau moi va nhap lai mat khau khong trung nhau")]
        public string ConfirmPassword { get; set; }

        [Required]
        public string ResetCode { get; set; }
    }
}