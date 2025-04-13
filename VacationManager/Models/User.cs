using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationManager.Models
{
    public class User : IdentityUser
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        
        [ForeignKey("Team")]
        public int? TeamId { get; set; }

        public Team? Team { get; set; }

        //  оставащи дни отпуск
        [Required]
        [Range(0, 365)]
        [Display(Name = "Оставащи дни отпуск")]
        public int LeaveDaysRemaining { get; set; } = 20; 
    }
}
