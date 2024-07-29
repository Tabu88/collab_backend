namespace collab_api2.Models
{
    public class MessagesResponseModel
    {
        public string Status { get; set; }
        public string Message { get; set; }

        public string Token { get; set; }

        public List<Messages> Messages { get; set; }
    }
}
