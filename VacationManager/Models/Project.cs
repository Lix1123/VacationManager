using System.ComponentModel.DataAnnotations;

namespace VacationManager.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        public ICollection<Team> Teams { get; set; } = new List<Team>();
    }
}
