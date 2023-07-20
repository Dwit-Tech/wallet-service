using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DwitTech.WalletService.Data.Entities
{
    public class Wallet:BaseEntity
    {
        [Required]
        public string Username { get; set; }
        public double Balance { get; set; } = 0.0;
        [Required]
        public string Currency { get; set; }        
    }
}
