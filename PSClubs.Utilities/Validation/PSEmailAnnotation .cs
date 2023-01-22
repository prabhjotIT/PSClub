using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.Mail;

namespace PSClubsUtilities.Validation
{
    public class PSEmailAnnotation: ValidationAttribute
    {
        public PSEmailAnnotation()
        {
            ErrorMessage = $"{0} does not match the Candian Postal code pattern A3A 3A3";
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;
            //not sure if prof. asked for this in 3C part at page 4 or something else, this is i coding not giving a lot of attention to needs
            try { 
            MailAddress m = new MailAddress(value.ToString());// it should throw an exception when an object of this email class is intiated if the email is invalid
                return ValidationResult.Success;
            }
            catch
            {
                return new ValidationResult(string.Format(ErrorMessage, validationContext.DisplayName));
            }
           // Regex pattern = new Regex(@"^[A-Za-z]\d[A-Za-z] ?[A-Za-z]\d$", RegexOptions.IgnoreCase);

           
        }
    }
}
