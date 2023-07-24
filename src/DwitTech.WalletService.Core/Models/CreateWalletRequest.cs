using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DwitTech.WalletService.Core.Models
{
    public class CreateWalletRequest
    {
        [Required]
        [JsonPropertyName("userName")]
        public string UserName { get; set; }

        [Required]
        [JsonPropertyName("currencyCode")]
        public string CurrencyCode { get; set; }
    }
}
