using System.ComponentModel.DataAnnotations;

namespace TaskManagementSystemMVC.Models
{
    public class TaskItems
    {
        public int Id { get; set; }
        [Required]
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public string? Status { get; set; }  

        public string? UserId { get; set; }
    public ApplicationUser? User { get; set; }
    }

}
