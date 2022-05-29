using Digest.Server.Application;
using System.Security.Cryptography;
using System.Text;

namespace Digest.Server.Infrastructure;

public class HashService : IHashService
{
    public string ToMd5Hash(byte[] bytes)
    {
        var hashBytes = MD5.Create().ComputeHash(bytes);
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
    }

    public string ToMd5Hash(string inputString)
    {
        return ToMd5Hash(Encoding.UTF8.GetBytes(inputString));
    }
}
