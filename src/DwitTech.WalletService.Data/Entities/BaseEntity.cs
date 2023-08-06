using System.ComponentModel.DataAnnotations;

namespace DwitTech.WalletService.Data.Entities
{
    public class BaseEntity
    {
        [Key]
        public int Id { get; set; }
        public DateTime CreatedOnUtc { get; private set; } = DateTime.UtcNow;
        public DateTime? ModifiedOnUtc { get; set; }
        [Required]
        public string CreatedBy { get; set; }
        public string? ModifiedBy { get; set; }
    }
}
