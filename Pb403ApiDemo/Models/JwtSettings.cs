namespace Pb403ApiDemo.Models
{
    public class JwtRequestModel
    {
        public required string Username { get; set; }
        public required string Email { get; set; }
        public List<string> Roles { get; set; } = [];
    }

    public class JwtResponseModel
    {
        public required string Token { get; set; }
    }

    public class JwtSettings
    {
        public required string Key { get; set; }
        public string? Issuer { get; set; }
        public string? Audience { get; set; }
        public int DurationInMinutes { get; set; }
    }
}
