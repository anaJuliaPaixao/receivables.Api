using Receivables.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace receivables.Domain.DTOs.Company
{
    public class CompanyRequest : IValidatableObject
    {
        public string Cnpj { get; set; } = null!;

        [Required(ErrorMessage = "O nome é obrigatório")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "O faturamento mensal é obrigatório")]
        public decimal MonthlyRevenue { get; set; }

        [Required(ErrorMessage = "O segmento é obrigatório")]
        public CompanySegment Segment { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Cnpj))
            {
                yield return new ValidationResult("O CNPJ é obrigatório", new[] { nameof(Cnpj) });
                yield break;
            }

            var digitsOnly = new string(Cnpj.Where(char.IsDigit).ToArray());

            if (digitsOnly != Cnpj)
            {
                yield return new ValidationResult("O CNPJ deve conter apenas dígitos (sem formatação)", new[] { nameof(Cnpj) });
                yield break;
            }

            if (digitsOnly.Length != 14)
            {
                yield return new ValidationResult("O CNPJ deve ter 14 dígitos", new[] { nameof(Cnpj) });
                yield break;
            }

            if (digitsOnly.Distinct().Count() == 1)
            {
                yield return new ValidationResult("O CNPJ não pode ter todos os dígitos iguais", new[] { nameof(Cnpj) });
                yield break;
            }

            if (!IsValidCnpj(digitsOnly))
            {
                yield return new ValidationResult("O CNPJ informado é inválido", new[] { nameof(Cnpj) });
            }
        }

        private bool IsValidCnpj(string cnpj)
        {
            int[] weight1 = {5,4,3,2,9,8,7,6,5,4,3,2};
            int[] weight2 = {6,5,4,3,2,9,8,7,6,5,4,3,2};

            var nums = cnpj.Select(c => c - '0').ToArray();

            int sum1 = 0;
            for (int i = 0; i < 12; i++) sum1 += nums[i] * weight1[i];
            int mod1 = sum1 % 11;
            int check1 = mod1 < 2 ? 0 : 11 - mod1;
            if (nums[12] != check1) return false;

            int sum2 = 0;
            for (int i = 0; i < 13; i++) sum2 += nums[i] * weight2[i];
            int mod2 = sum2 % 11;
            int check2 = mod2 < 2 ? 0 : 11 - mod2;
            return nums[13] == check2;
        }
    }
}
