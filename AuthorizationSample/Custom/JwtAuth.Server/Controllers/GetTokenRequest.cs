namespace JwtAuth.Server.Controllers
{
    public class GetTokenRequest
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }
}
