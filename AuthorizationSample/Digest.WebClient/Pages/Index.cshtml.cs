using Digest.Server.Application;
using Digest.Server.Infrastructure;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;

namespace Digest.Client.Pages
{
    public class IndexModel : PageModel
    {
        private const string ServerUrl = "https://localhost:7000/api/resources";
        private readonly HttpClient _httpClient;
        private readonly IHashService _hashService;
        private readonly HeaderService _headerService;

        public IndexModel(IHttpClientFactory factory,
            IHashService hashService,
            HeaderService headerService)
        {
            _httpClient = factory.CreateClient();
            _hashService = hashService;
            _headerService = headerService;
        }


        public string UserName { get; set; }
        public string Password { get; set; }

        public string Realm { get; set; }
        public string Qop { get; set; }
        public string Nonce { get; set; }
        public string Opaque { get; set; }

        public string Response { get; set; }

        public async Task OnGetAsync()
        {
            using (var req = new HttpRequestMessage(HttpMethod.Get, ServerUrl))
            {
                var resp = await _httpClient.SendAsync(req);
                
                if (resp.StatusCode != HttpStatusCode.Unauthorized)
                {
                    throw new Exception($"First call to api without auth should return 401 Unauthorized, but it is {resp.StatusCode}");
                }

                var authHeader = resp.Headers.GetValues("WWW-Authenticate").Single();

                var realmInd = authHeader.IndexOf("realm");
                var values = authHeader.Substring(0, "Digest ".Length).Split(",");

                var valuesDict = new Dictionary<string, string>();

                foreach (var val in values)
                {
                    var keyValuePair = val.Split("=", 2);

                    if (keyValuePair[1].StartsWith('\"'))
                        keyValuePair[1] = keyValuePair[1].Trim('\"');

                    valuesDict.Add(keyValuePair[0], keyValuePair[1]);
                }

                //for this simple example we don't support anything except MD5 so we don't parse it
                Realm = valuesDict["realm"];
                Nonce = valuesDict["nonce"];
                Qop = valuesDict["qop"];
                Opaque = valuesDict["opaque"];
            }

            UserName = "andreyka26_";
            Password = "mypass1";
        }

        public async Task OnPostAsync(string userName, string password, string realm, string qop, string nonce, string opaque)
        {
            var a1Hash = _hashService.ToMd5Hash($"{userName}:{realm}:{password}");
            var a2Hash = _hashService.ToMd5Hash($"GET:{ServerUrl}");
            var nc = "00000001";
            var cnonce = "0a4f113b";

            var response = _hashService.ToMd5Hash($"{a1Hash}:{nonce}:{nc}:{cnonce}:{qop}:{a2Hash}");

            var parts = new (string Key, string Value, bool ShouldQuote)[] {
                ("realm", realm, true),
                ("nonce", nonce, true),
                ("uri", ServerUrl, true),
                ("qop", qop, true),
                ("nc", nc, true),
                ("cnonce", cnonce, true),
                ("response", response, true),
                ("opaque", opaque, true),
                ("algorithm", "MD5", false),
            };

            using (var req = new HttpRequestMessage(HttpMethod.Get, ServerUrl))
            {
                req.Headers.Add("Authorization", $"Digest {parts.Select(_headerService.FormatHeaderComponent)}");
                var resp = await _httpClient.SendAsync(req);

                Response = await resp.Content.ReadAsStringAsync();
            }
        }
    }
}