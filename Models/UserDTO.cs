using System.ComponentModel.DataAnnotations;

namespace collab_api.Models
{
    public class UserDTO
    {
        [Required]
        public string Name { get; set; } = "";
        
        [Required]
        public string Email { get; set; } = "";
        
        [Required]
        public string Password { get; set; } = "";

        [Required]
        public string Gender { get; set; } = "";

        public byte[] ProfileImage { get; set; }
        public string Location { get; set; } = "";
    }
}
