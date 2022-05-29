using System.Text.RegularExpressions;

namespace Digest.Server.Application;

internal class DigestValue
{
    public DigestValue(string realm, string uri, string username, string nonce, string nonceCounter, string clientNonce, string response)
    {
        Realm = realm;
        Uri = uri;
        Username = username;
        Nonce = nonce;
        NonceCounter = nonceCounter;
        ClientNonce = clientNonce;
        Response = response;
    }

    public string Realm { get; }
    public string Uri { get; }
    public string Username { get; }
    public string Nonce { get; }
    public string NonceCounter { get; }
    public string ClientNonce { get; }
    public string Response { get; }
}