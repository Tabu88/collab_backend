using System.ComponentModel.DataAnnotations;


namespace collab_api.Models
{
    public class SubtasksDTO
    {
        [Required]
        public string Title { get; set; } = "";

        [Required]
        public string Checked { get; set; } = "";

        [Required]  
        public int UserId { get; set; }

        [Required]
        public int TaskId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
