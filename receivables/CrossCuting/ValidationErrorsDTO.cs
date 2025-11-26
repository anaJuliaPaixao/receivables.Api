using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;

namespace receivables.CrossCuting
{
    public class ValidationErrorsDTO
    {
        [Required]
        [JsonProperty("errors")]
        public List<ErrorDetails> Errors { get; set; } = new List<ErrorDetails>();

        public ValidationErrorsDTO(ModelStateDictionary modelState)
        {
            foreach (var item in modelState)
            {
                var error = item.Value.Errors.FirstOrDefault();
                if (error != null)
                {
                    Errors.Add(new ErrorDetails
                    {
                        PropertyName = ToCamelCase(item.Key),
                        ErrorMessage = error.ErrorMessage
                    });
                }
            }
        }

        public static string ToCamelCase(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return input;
            }

            TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
            string result = textInfo.ToTitleCase(input).Replace(" ", string.Empty);
            return char.ToLowerInvariant(result[0]) + result.Substring(1);
        }
    }

    public class ErrorDetails
    {
        [Required]
        [JsonProperty("propertyName")]
        public string PropertyName { get; set; }

        [Required]
        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; set; }
    }
}
