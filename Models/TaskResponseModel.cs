namespace collab_api2.Models
{
    public class TaskResponseModel
    {
        public string Status { get; set; }
        public string Message { get; set; }

        public string Token { get; set; }

        public List<Task> Tasks { get; set; }
    }
}
