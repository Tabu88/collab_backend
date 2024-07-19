using System.ComponentModel.DataAnnotations;

namespace collab_api.Models
{
    public class Messages
    {
        [Key]
        public int Id { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public string Message { get; set; }
        public DateTime SentAt { get; set; }

    }
}
