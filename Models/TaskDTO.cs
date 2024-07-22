using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace collab_api2.Models
{
    public class TaskDTO
    {

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public string Alert { get; set; }

        [Required]
        public string UsersId { get; set; }

        [Required]
        public DateTime Deadline { get; set; }


    }

    public class SubtaskDTO
    {
        [Required]
        public string Subtitle { get; set; }

        [Required]
        public string Checked { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("TaskId")]
        public int TaskId { get; set; }


    }
}
