using AutoMapper;
using DwitTech.WalletService.Data.Entities;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DwitTech.WalletService.Core.Dtos
{
    [AutoMap(typeof(Currency))]
    public class CurrencyReadDto
    {
        [Required]
        [JsonPropertyName("currencyCode")]
        public string Code { get; set; }

        [Required]
        [JsonPropertyName("currencyName")]
        public string Name { get; set; }
    }
}