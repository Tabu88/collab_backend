using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace collab_api2.Models
{
    public class Task
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Alert { get; set; }
        public string Status { get; set; }
        public string Category { get; set; }
        public DateTime Deadline { get; set; }
        public string UserId { get; set; }
        public DateTime CreatedAt { get; set; }

    }

    public class Subtask
    {
        [Key]
        public int Id { get; set; }
        public string Subtitle { get; set; }
        public string Checked { get; set; }
        public int UserId { get; set; }

        [ForeignKey("TaskId")]
        public int TaskId { get; set; }
        public DateTime CreatedAt { get; set; }




    }
}
