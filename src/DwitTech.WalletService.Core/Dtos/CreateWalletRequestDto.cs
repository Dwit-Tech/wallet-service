using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DwitTech.WalletService.Core.Dtos
{
    public class CreateWalletRequestDto
    {
        [Required]
        [JsonPropertyName("userId")]
        public int UserId { get; set; }

        [Required]
        [JsonPropertyName("CurrencyCode")]
        public string CurrencyCode { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
    }
}