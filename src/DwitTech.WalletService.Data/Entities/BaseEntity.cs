using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.WalletService.Data.Entities
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedOnUtc { get; private set; } = DateTime.UtcNow;
        public DateTime? ModifiedOnUtc { get; set; }

        [Required]
        public string CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
