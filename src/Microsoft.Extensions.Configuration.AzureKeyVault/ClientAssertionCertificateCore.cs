using System;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.IdentityModel.Tokens;

namespace Microsoft.Extensions.Configuration.AzureKeyVault
{
    /// <summary>
    /// Containing certificate used to create client assertion.
    /// </summary>
    public sealed class ClientAssertionCertificateCore : IClientAssertionCertificate
    {
        /// <summary>
        /// Constructor to create credential with client Id and certificate.
        /// </summary>
        /// <param name="clientId">Identifier of the client requesting the token.</param>
        /// <param name="certificate">The certificate used as credential.</param>
        public ClientAssertionCertificateCore(string clientId, X509Certificate2 certificate)
        {
            if (string.IsNullOrWhiteSpace(clientId))
            {
                throw new ArgumentNullException(nameof(clientId));
            }

            if (certificate == null)
            {
                throw new ArgumentNullException(nameof(certificate));
            }

            var key = new X509SecurityKey(certificate);
            if (key.PublicKey.KeySize < MinKeySizeInBits)
            {
                throw new ArgumentOutOfRangeException(nameof(certificate),
                    $"The certificate used must have a key size of at least {MinKeySizeInBits} bits");
            }

            ClientId = clientId;
            Certificate = certificate;
            Key = key;
        }


        /// <summary>
        /// Gets the identifier of the client requesting the token.
        /// </summary>
        public string ClientId { get; }

        /// <summary>
        /// Gets minimum X509 certificate key size in bits
        /// </summary>
        public static int MinKeySizeInBits => 2048;

        /// <summary>
        /// Gets the certificate used as credential.
        /// </summary>
        public X509Certificate2 Certificate { get; }

        /// <summary>
        /// Gets the security key from the credential.
        /// </summary>
        public X509SecurityKey Key { get; }

        /// <summary>
        /// Signs a message using the private key in the certificate
        /// </summary>
        /// <param name="message">Message that needs to be signed</param>
        /// <returns>Signed message as a byte array</returns>
        public byte[] Sign(string message)
        {
            using (var rsa = Key.CryptoProviderFactory.CreateForSigning(Key, SecurityAlgorithms.RsaSha256Signature))
                return rsa.Sign(Encoding.UTF8.GetBytes(message));
        }

        /// <summary>
        /// Returns thumbprint of the certificate
        /// </summary>
        public string Thumbprint => Base64UrlEncoder.Encode(Certificate.GetCertHash());
    }
}
