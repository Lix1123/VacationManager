using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationManager.Models
{
    public class Team
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        // Връзка с Project 
        public int? ProjectId { get; set; }
        public Project? Project { get; set; }

        // Връзка с лидер на екипа
        public string? TeamLeadId { get; set; }
        public User? TeamLead { get; set; }

        // Потребители в екипа 
        public ICollection<User> Developers { get; set; } = new List<User>();

        // Име на проекта 
        [NotMapped]
        public string ProjectName => Project != null ? Project.Name : "Без проект";

        // Име на лидера 
        [NotMapped]
        public string TeamLeadName => TeamLead != null ? TeamLead.UserName : "Няма лидер";
    }
}
