using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Repository.Helper
{
    public class RSAEncryptDecrypt
    {
        public static byte[] encrypt(string data, string esewaPublicKey)
        {
            StringBuilder publicKey = new StringBuilder();
            publicKey.Append("-----BEGIN PUBLIC KEY-----\r\n");
            publicKey.Append(esewaPublicKey);
            publicKey.Append("\r\n-----END PUBLIC KEY-----");


            using (TextReader publicKeyTextReader = new StringReader(publicKey.ToString()))
            {
                RsaKeyParameters publicKeyParam = (RsaKeyParameters)new PemReader(publicKeyTextReader).ReadObject();

                RSAParameters rsaParams = DotNetUtilities.ToRSAParameters(publicKeyParam);

                RSACryptoServiceProvider csp = new RSACryptoServiceProvider();
                csp.ImportParameters(rsaParams);

                var result1 = csp.Encrypt(Encoding.UTF8.GetBytes(data), false);

                var output2 = EncryptDecryptSecreteKey.base64Encode(result1);

                var encryptedBytes = csp.Encrypt(Encoding.UTF8.GetBytes(data), false);
                var oooo = EncryptDecryptSecreteKey.base64Encode(encryptedBytes);

                return result1;
            }

        }


        public static string decrypt(byte[] secretKey, string clientPrivateKey)
        {
            string error = string.Empty;

            StringBuilder privateStr = new StringBuilder();
            privateStr.Append("-----BEGIN RSA PRIVATE KEY-----\r\n");
            privateStr.Append(clientPrivateKey);
            privateStr.Append("\r\n-----END RSA PRIVATE KEY-----");

            using (TextReader strReader = new StringReader(privateStr.ToString()))
            {

                PemReader pemReader = new PemReader(strReader);
                AsymmetricCipherKeyPair keyPair = (AsymmetricCipherKeyPair)pemReader.ReadObject();

                RSAParameters rsaParams = DotNetUtilities.ToRSAParameters((RsaPrivateCrtKeyParameters)keyPair.Private);
                RSACryptoServiceProvider csp = new RSACryptoServiceProvider();
                csp.ImportParameters(rsaParams);

                var decryptedBytes = csp.Decrypt(secretKey, false);
                return Encoding.UTF8.GetString(decryptedBytes, 0, decryptedBytes.Length);

            }

        }

    }
}
