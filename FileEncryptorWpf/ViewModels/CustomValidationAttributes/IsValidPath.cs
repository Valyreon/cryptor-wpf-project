using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileEncryptorWpf.ViewModels.CustomValidationAttributes
{
    public class IsValidPath : ValidationAttribute
    {
        public IsValidPath()
        {
        }

        public override bool IsValid(object value)
        {
            string strValue = value as string;

            if (value != null)
                return true;

            if (strValue != string.Empty)
            {
                FileInfo fi = null;
                try
                {
                    fi = new FileInfo(strValue);
                }
                catch (ArgumentException) { }
                catch (PathTooLongException) { }
                catch (NotSupportedException) { }
                if (fi is null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }
    }
}
