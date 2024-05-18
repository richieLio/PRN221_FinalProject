using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Helper
{
    public static class ValidationHelper
    {

        public static List<ValidationResult> ValidateModel(object model)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(model, serviceProvider: null, items: null);
            Validator.TryValidateObject(model, context, results, validateAllProperties: true);

            return results;
        }

        public static bool IsPropertyValid<T>(T model, string propertyName)
        {
            var context = new ValidationContext(model, serviceProvider: null, items: null) { MemberName = propertyName };
            return Validator.TryValidateProperty(model.GetType().GetProperty(propertyName)?.GetValue(model), context, new List<ValidationResult>());
        }

        public static IEnumerable<string> GetErrorMessagesForProperty<T>(T model, string propertyName)
        {
            var context = new ValidationContext(model, serviceProvider: null, items: null) { MemberName = propertyName };
            var results = new List<ValidationResult>();
            Validator.TryValidateProperty(model.GetType().GetProperty(propertyName)?.GetValue(model), context, results);
            return results.Select(r => r.ErrorMessage);
        }
    }
}



