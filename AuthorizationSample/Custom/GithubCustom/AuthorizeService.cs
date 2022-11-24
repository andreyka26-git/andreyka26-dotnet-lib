namespace GithubCustom
{
    public class AuthorizeService
    {
        // we need to validate this state is the same per client's oauth flow
        private const string State = "qwerty";

        private IConfiguration _config;

        public AuthorizeService(IConfiguration config)
        {
            _config = config;
        }

        public async Task<string> GetAuthTokenAsync(CallbackResponse callback)
        {
            throw new NotImplementedException();
        }

        public string GenerateAuthorizeLink()
        {
            var clientId = _config.GetValue<string>("ClientId");
            var clientSecret = _config.GetValue<string>("ClientSecret");
            var redirectUrl = _config.GetValue<string>("RedirectUrl");

            var link = $"https://github.com/login/oauth/authorize?client_id={clientId}&redirect_uri={redirectUrl}&scope=user&state={State}";

            return link;
        }
    }
}
