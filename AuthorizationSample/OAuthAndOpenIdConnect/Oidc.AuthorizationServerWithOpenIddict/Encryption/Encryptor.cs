using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace Oidc.AuthorizationServerWithOpenIddict.Encryption
{
    public class Encryptor : IEncryptor
    {
        public (string name, byte[] content) GenerateEncryptionCertificate()
        {
            using var algorithm = RSA.Create(keySizeInBits: 2048);

            var subject = new X500DistinguishedName("CN=Fabrikam Encryption Certificate");
            var request = new CertificateRequest(subject, algorithm, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.KeyEncipherment, critical: true));

            var now = DateTimeOffset.UtcNow;
            var dueDate = now.AddYears(2);

            var certificate = request.CreateSelfSigned(now, dueDate);

            var name = $"encryption-certificate_{dueDate.Year}.{dueDate.Month}.{dueDate.Day}.pfx";
            var content = certificate.Export(X509ContentType.Pfx, string.Empty);

            return (name, content);
        }

        public (string name, byte[] content) GenerateSigningCertificate()
        {
            using var algorithm = RSA.Create(keySizeInBits: 2048);

            var subject = new X500DistinguishedName("CN=Fabrikam Signing Certificate");
            var request = new CertificateRequest(subject, algorithm, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            request.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, critical: true));

            var now = DateTimeOffset.UtcNow;
            var dueDate = now.AddYears(2);

            var certificate = request.CreateSelfSigned(now, dueDate);

            var name = $"signing-certificate_{dueDate.Year}.{dueDate.Month}.{dueDate.Day}.pfx";
            var content = certificate.Export(X509ContentType.Pfx, string.Empty);

            return (name, content);
        }
    }
}
