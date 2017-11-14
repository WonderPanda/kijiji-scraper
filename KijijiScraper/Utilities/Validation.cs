using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KijijiScraper.Utilities
{
    public static class Validation
    {
        public static bool ValidateModel(object model, out List<ValidationResult> results)
        {
            results = new List<ValidationResult>();
            var validationContext = new ValidationContext(model);
            bool isValid = Validator.TryValidateObject(model, validationContext, results, true);
            return isValid;
        }
    }
}
