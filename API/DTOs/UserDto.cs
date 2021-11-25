namespace API.DTOs
{
    // information that we are going to send ack to the user one authenticated
    public class UserDto
    {
        public string DisplayName { get; set; }
        public string Token { get; set; }
        public string Username { get; set; }
        public string Image { get; set; }
    }
}