namespace collab_api.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = ""; 
        public DateTime CreatedAt { get; set; }
        public byte[] ProfileImage { get; set; }
        public string Location { get; set; } = "";
    }
}
