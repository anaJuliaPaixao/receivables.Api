using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace receivables.CrossCuting
{
    public class ExceptionDTO
    {
        [Required]
        [JsonProperty("errorMessage")]
        public string ErrorMessage { get; set; } = null!;
    }
}
