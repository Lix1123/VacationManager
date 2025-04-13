using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationManager.Models
{
    public class Notification
    {
        public int Id { get; set; }

        [Required]
        public string RecipientId { get; set; } = null!;

        [ForeignKey("RecipientId")]
        public User Recipient { get; set; } = null!;

        [Required]
        public string Message { get; set; } = string.Empty;

        public bool IsRead { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
