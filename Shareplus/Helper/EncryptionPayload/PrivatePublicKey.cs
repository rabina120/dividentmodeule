/*using java.security;
using java.security.spec;
using java.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDSMODULE.Helper.EncryptionPayload
{
    public class PrivatePublicKey
    {
        public static PrivateKey getPrivateKey(string base64PrivateKey)
        {
            PrivateKey privateKey = null;
            PKCS8EncodedKeySpec keySpec = new PKCS8EncodedKeySpec(Base64.getDecoder().decode(Encoding.UTF8.GetBytes(base64PrivateKey)));
            KeyFactory keyFactory = null;
            try
            {
                keyFactory = KeyFactory.getInstance("RSA");
            }
            catch (NoSuchAlgorithmException e)
            {
                e.printStackTrace();
            }
            try
            {
                privateKey = keyFactory.generatePrivate(keySpec);
            }
            catch (InvalidKeySpecException e)
            {
                e.printStackTrace();
            }
            return privateKey;
        }

        public static PublicKey getPublicKey(string base64PublicKey)
        {
            PublicKey publicKey = null;
            try
            {
                X509EncodedKeySpec keySpec = new X509EncodedKeySpec(Base64.getDecoder().decode(Encoding.UTF8.GetBytes(base64PublicKey)));
                KeyFactory keyFactory = KeyFactory.getInstance("RSA");
                publicKey = keyFactory.generatePublic(keySpec);
                return publicKey;
            }
            catch (NoSuchAlgorithmException e)
            {
                e.printStackTrace();
            }
            catch (InvalidKeySpecException e)
            {
                e.printStackTrace();
            }
            return publicKey;
        }
    }
}
*/