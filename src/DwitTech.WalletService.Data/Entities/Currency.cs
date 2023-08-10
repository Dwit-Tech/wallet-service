using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace DwitTech.WalletService.Data.Entities
{
    [Index(nameof(Code), IsUnique = true)]
    public class Currency : BaseEntity
    {
        [Required, MaxLength(3)]
        public string Code { get; set; }

        [Required]
        public string Name { get; set; }
    }
}