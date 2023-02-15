namespace OpenIdConnect.OpenIddict.AuthorizationServer.Encryption
{
    public interface IEncryptor
    {
        (string name, byte[] content) GenerateEncryptionCertificate();
        (string name, byte[] content) GenerateSigningCertificate();
    }
}
