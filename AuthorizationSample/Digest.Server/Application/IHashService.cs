namespace Digest.Server.Application;

public interface IHashService
{
    string ToMd5Hash(byte[] bytes);
    string ToMd5Hash(string inputString);
}
