namespace collab_api.Models
{
    public class Tasks
    {
        public int Id { get; set; }
        public string Title { get; set; } = "";
        public string Description { get; set; } = "";
        public string Alert { get; set; } = "";
        public List<User> Users { get; set; }
        public List<Subtasks> Subtasks { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
        