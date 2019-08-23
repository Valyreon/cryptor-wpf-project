using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileEncryptorWpf.ViewModels.CustomValidationAttributes
{
    /// <summary>
    /// Custom ValidationAttribute class that is used for determining if string can be used as a secure password.
    /// </summary>
    public class SecurePassword : ValidationAttribute
    {
        public SecurePassword()
        {
            this.ErrorMessage = "Password must be between 8 and 19 characters long and contain at least 1 digit.";
        }

        public override bool IsValid(object value)
        {
            string strValue = value as string;

            if (value is null)
            {
                return true;
            }

            return strValue.Length > 7 && strValue.Length < 20 && strValue.Any(c => char.IsDigit(c));
        }
    }
}