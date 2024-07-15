using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Model.BillModel
{
    public class BillCreateReqModel
    {
        public Guid RoomId { get; set; }

        [MinimumCount(1, ErrorMessage = "Please choose at least one service")]
        [DictionaryValueGreaterThanZero(ErrorMessage = "Service quantities must be greater than zero")]
        public Dictionary<Guid, decimal> ServiceQuantities { get; set; } = new Dictionary<Guid, decimal>();
    }

    public class MinimumCountAttribute : ValidationAttribute
    {
        private readonly int _minCount;

        public MinimumCountAttribute(int minCount)
        {
            _minCount = minCount;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IDictionary dictionary)
            {
                if (dictionary.Count >= _minCount)
                {
                    return ValidationResult.Success!;
                }
            }

            return new ValidationResult(ErrorMessage ?? $"The collection must contain at least {_minCount} items.");
        }
    }

    public class DictionaryValueGreaterThanZeroAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IDictionary<Guid, decimal> dictionary)
            {
                foreach (var kvp in dictionary)
                {
                    if (kvp.Value <= 0)
                    {
                        return new ValidationResult(ErrorMessage ?? "All values must be greater than zero.");
                    }
                }
            }

            return ValidationResult.Success!;
        }
    }
}
