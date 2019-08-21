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
        bool invert = false;

        public DirectoryExists()
        {
        }

        public DirectoryExists(bool invert)
        {
            this.invert = invert;
        }

        public override bool IsValid(object value)
        {
            string strValue = value as string;

            if (value is null)
                return true;

            var res = Directory.Exists(strValue);

            if (invert)
                return !res;
            return res;
        }
    }
}
