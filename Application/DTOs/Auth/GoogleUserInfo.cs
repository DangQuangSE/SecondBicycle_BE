namespace Application.DTOs.Auth
{
    public class GoogleUserInfo
    {
        public string Email { get; set; } = string.Empty;
        public string? GivenName { get; set; }
        public string? FamilyName { get; set; }
        public string? Name { get; set; }
    }
}
