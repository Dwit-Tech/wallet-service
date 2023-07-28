using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DwitTech.WalletService.Core.Models
{
    public class CreateWalletRequest
    {
        [Required]
        [JsonPropertyName("userId")]
        public int UserId { get; set; }

        [Required]
        [JsonPropertyName("currencyCode")]
        public string CurrencyCode { get; set; }
    }
}
