using Microsoft.VisualBasic;

namespace collab_api2.Models
{
    public class ResponseModel
    {
        public string Status { get; set; }
        public string Message { get; set; }

        public string Token { get; set; }

        public List<User> Users { get; set; }
    }
}
