using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DwitTech.WalletService.Data.Entities
{
    public class Wallet:BaseEntity
    {
        [Required]
        public int UserId { get; set; }

        public double Balance { get; set; } = 0.0;

        [ForeignKey("CurrencyId")]
        public Currency Currency { get; set; }
    }
}