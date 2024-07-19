using System.ComponentModel.DataAnnotations;

namespace collab_api.Models
{
    public class MessagesDTO
    {

        [Required]
        public string From { get; set; }

        [Required]
        public string To { get; set; }

        [Required]
        public string Message { get; set; }
    }
}
