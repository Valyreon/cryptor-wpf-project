using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileEncryptorWpf.ViewModels.CustomValidationAttributes
{
    public class FileExists : ValidationAttribute
    {
        private readonly bool invert = false;

        public FileExists()
        {
        }

        public FileExists(bool invert)
        {
            this.invert = invert;
        }

        public override bool IsValid(object value)
        {
            string strValue = value as string;

            if (value is null)
            {
                return true;
            }

            var res = File.Exists(strValue);

            if (this.invert)
            {
                return !res;
            }

            return res;
        }
    }
}