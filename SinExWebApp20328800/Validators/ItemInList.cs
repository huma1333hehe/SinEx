using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Globalization;

namespace SinExWebApp20328800.Validators
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    sealed public class ItemInListAttribute : ValidationAttribute
    {
        private readonly List<string> _MyList;

        public ItemInListAttribute(string[] YourList)
            :base("{0} is not in the valid list.")
        {
            _MyList = YourList
            .Select(item => item)
            .ToList();
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

                       if (value != null)
                       {
                       var valueAsString = value.ToString().Trim();
                      if (!_MyList.Contains(valueAsString))
                     {
                     var errorMessage = FormatErrorMessage(validationContext.DisplayName);
                     return new ValidationResult(errorMessage);
                  }
                }
                     return ValidationResult.Success;
                  }
  

        public override string FormatErrorMessage(string name)
        {
            return String.Format(CultureInfo.CurrentCulture, ErrorMessageString, name);
        }
    }
}