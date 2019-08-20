using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileEncryptorWpf.ViewModels.CustomValidationAttributes
{
    public class DirectoryExists : ValidationAttribute
    {
        public DirectoryExists()
        {
        }

        public override bool IsValid(object value)
        {
            string strValue = value as string;

            if (value is null)
                return true;

            return Directory.Exists(strValue);
        }
    }
}
