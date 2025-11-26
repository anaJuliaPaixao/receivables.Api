using System.ComponentModel.DataAnnotations;

namespace receivables.Domain.DTOs.Invoice
{
    public class UpdateInvoiceRequest : IValidatableObject
    {
        [Required(ErrorMessage = "O número da nota fiscal é obrigatório")]
        public string Number { get; set; } = null!;

        [Required(ErrorMessage = "O valor da nota fiscal é obrigatório")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "A data de vencimento é obrigatória")]
        public DateTime DueDate { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (DueDate.Date <= DateTime.UtcNow.Date)
            {
                yield return new ValidationResult("A data de vencimento deve ser maior que a data atual", new[] { nameof(DueDate) });
            }

            if (Amount <= 0)
            {
                yield return new ValidationResult("O valor da nota fiscal deve ser maior que zero", new[] { nameof(Amount) });
            }
        }
    }
}
