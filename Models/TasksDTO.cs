using System.ComponentModel.DataAnnotations;


namespace collab_api.Models
{
    public class TasksDTO
    {
        [Required]
        public string Title { get; set; } = "";

        [Required]
        public string Description { get; set; } = "";

        [Required]
        public string Alert { get; set; } = "";

        [Required]
        public List<UserDTO> Users { get; set; }

        [Required]
        public List<SubtasksDTO> Subtasks { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
