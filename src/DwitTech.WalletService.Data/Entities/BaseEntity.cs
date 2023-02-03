using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.WalletService.Data.Entities
{
    public class BaseEntity
    {
        public int Id { get; set; }
        public DateTime CreatedOnUtc { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedOnUtc { get; set; }
    }
}
