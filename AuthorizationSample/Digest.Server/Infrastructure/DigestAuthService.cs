using Digest.Server.Application;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Text;

namespace Digest.Server.Infrastructure;

internal class DigestAuthService
{
    private readonly IUsernameHashedSecretProvider _usernameHashedSecretProvider;
    private readonly IHashService _hashService;
    private readonly HeaderService _headerService;
    private readonly DigestAuthenticationOptions _options;

    public DigestAuthService(IUsernameHashedSecretProvider usernameHashedSecretProvider,
        IHashService hashService,
        HeaderService headerService,
        IOptions<DigestAuthenticationOptions> options)
    {
        _options = options.Value;
        _usernameHashedSecretProvider = usernameHashedSecretProvider;
        _hashService = hashService;
        _headerService = headerService;
    }

    public bool UseAuthenticationInfoHeader => _options.UseAuthenticationInfoHeader;

    public string GetUnauthorizedDigestHeaderValue()
    {
        var parts = new List<DigestSubItem> {
                new DigestSubItem(Consts.RealmNaming, _options.Realm, true),
                new DigestSubItem(Consts.NonceNaming, CreateNonce(DateTime.UtcNow), true),
                new DigestSubItem(Consts.QopNaming, Consts.QopMode, true),
                new DigestSubItem(Consts.OpaqueNaming, Consts.Opaque, true),
                new DigestSubItem(Consts.AlgorithmNaming, Consts.Algorithm, false),
            };

        return _headerService.BuildDigestHeaderValue(parts);
    }

    public async Task<string> GetAuthInfoHeaderAsync(DigestValue response)
    {
        var timestampStr = response.Nonce.Substring(0, Consts.NonceTimestampFormat.Length);
        var timestamp = ParseTimestamp(timestampStr);

        var delta = DateTime.UtcNow - timestamp;
        var deltaSeconds = Math.Abs(delta.TotalSeconds);

        var a1Hash = await _usernameHashedSecretProvider.GetA1Md5HashForUsernameAsync(response.Username, _options.Realm);
        var a2Hash = _hashService.ToMd5Hash($":{response.Uri}");

        var resp = _hashService.ToMd5Hash($"{a1Hash}:{response.Nonce}:{response.NonceCounter}:{response.ClientNonce}:{Consts.QopMode}:{a2Hash}");

        var digestValueParts = new List<DigestSubItem>();

        if (Math.Abs(deltaSeconds - _options.MaxNonceAgeSeconds) < _options.DeltaSecondsToNextNonce)
        {
            digestValueParts.Add(new DigestSubItem("nextnonce", CreateNonce(DateTime.UtcNow), true));
        }

        digestValueParts.Add(new("qop", Consts.QopMode, true));
        digestValueParts.Add(new("rspauth", resp, true));
        digestValueParts.Add(new("cnonce", response.ClientNonce, true));
        digestValueParts.Add(new("nc", response.NonceCounter, false));

        return _headerService.BuildDigestHeaderValue(digestValueParts, string.Empty);
    }

    public async Task EnsureDigestValueValid(DigestValue challengeResponse, string requestMethod)
    {
        EnsureNonceValid(challengeResponse);

        var a1Hash = await _usernameHashedSecretProvider.GetA1Md5HashForUsernameAsync(challengeResponse.Username, _options.Realm);

        var a2 = $"{requestMethod}:{challengeResponse.Uri}";
        var a2Hash = _hashService.ToMd5Hash(a2);

        var expectedHash = _hashService
            .ToMd5Hash($"{a1Hash}:{challengeResponse.Nonce}:{challengeResponse.NonceCounter}:{challengeResponse.ClientNonce}:{Consts.QopMode}:{a2Hash}");

        if (expectedHash != challengeResponse.Response)
            throw new Exception("Hashes are not equal");
    }

    private void EnsureNonceValid(DigestValue challengeResponse)
    {
        var timestampStr = challengeResponse.Nonce.Substring(0, Consts.NonceTimestampFormat.Length);
        var timestamp = ParseTimestamp(timestampStr);

        var delta = DateTime.UtcNow - timestamp;

        if (Math.Abs(delta.TotalSeconds) > _options.MaxNonceAgeSeconds)
        {
            throw new Exception("time exceeded MaxNonceAge");
        }

        var currentNonce = CreateNonce(timestamp.DateTime);

        if (challengeResponse.Nonce != currentNonce)
            throw new Exception("Nonce doesn't match.");
    }
    private string CreateNonce(DateTime timestamp)
    {
        var sb = new StringBuilder();
        var timestampStr = timestamp.ToString(Consts.NonceTimestampFormat, CultureInfo.InvariantCulture);

        sb.Append(timestampStr);
        sb.Append(" ");
        sb.Append(_hashService.ToMd5Hash($"{timestampStr}:{_options.ServerNonceSecret}"));

        return sb.ToString();
    }

    private static DateTimeOffset ParseTimestamp(string timestampStr)
    {
        return DateTimeOffset.ParseExact(timestampStr, Consts.NonceTimestampFormat, CultureInfo.InvariantCulture);
    }
}