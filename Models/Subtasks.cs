namespace collab_api.Models
{
    public class Subtasks
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Checked { get; set; } = "";
        public int UserId { get; set; }
        public int TaskId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
