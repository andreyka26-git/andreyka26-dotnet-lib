using Digest.Server.Application;

namespace Digest.Server.Infrastructure;

internal class TrivialUsernameHashedSecretProvider : IUsernameHashedSecretProvider
{
    public Task<string> GetA1Md5HashForUsernameAsync(string username, string realm)
    {
        if (username == "andreyka26_" && realm == "some-realm")
        {
            // The hash value below would have been pre-computed & stored in the database.
            // var hash = _hashService.ToMd5Hash("andreyka26_:some-realm:mypass1");
            const string hash = "3173b15af925576b8b6eb4c65edb9e84";

            return Task.FromResult(hash);
        }

        // User not found
        return Task.FromResult(string.Empty);
    }
}
