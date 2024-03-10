namespace Absencespot.Dtos
{
    public class TokenResponse
    {
        public string Token { get; set; }
        public string? RefreshToken { get; set; }
        public string? IdToken{ get; set; }
        public string? Type{ get; set; }
    }
}
