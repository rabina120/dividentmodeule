/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.Text;
using java.util;
using javax.crypto;
using javax.crypto.spec;
using java.security;
using java.security.spec;

namespace CDSMODULE.Helper.EncryptionPayload
{
    public class EncryptDecrypt
    {

        public static byte[] base64Decode(string data)
        {
            return Base64.getDecoder().decode(data);
        }

        public static string base64Encode(byte[] data)
        {
            return Base64.getEncoder().encodeToString(data);
        }

        public static SecretKey getSecretKey(string secretKey)
        {
            byte[] decodeSecretKey = base64Decode(secretKey);
            return new SecretKeySpec(decodeSecretKey, 0, decodeSecretKey.Length, "AES");
        }

        public static string keyToString(SecretKey secretKey)
        {
            byte[] encoded = secretKey.getEncoded();
            string encodedKey = base64Encode(encoded);
            return encodedKey;
        }

        public static byte[] encrypt(string data, string esewaPublicKey)
        {
            Cipher cipher = Cipher.getInstance("RSA/ECB/PKCS1Padding");
            cipher.init(Cipher.ENCRYPT_MODE, getSecretKey(esewaPublicKey));
            return cipher.doFinal(Convert.FromBase64String(data));
        }

        public string encrypt(string strToEncrypt, SecretKey secretKey)
        {
            Cipher cipher = Cipher.getInstance("AES/ECB/PKCS5Padding");
            cipher.init(Cipher.ENCRYPT_MODE, secretKey);
            return base64Encode(cipher.doFinal(Encoding.UTF8.GetBytes(strToEncrypt)));
        }

     
        public static string decrypt(string strToDecrypt, SecretKey secretKey)
        {
            Cipher cipher = Cipher.getInstance("AES/ECB/PKCS5Padding");
            cipher.init(Cipher.DECRYPT_MODE, secretKey);
            return Convert.ToString(cipher.doFinal(base64Decode(data: strToDecrypt)));
        }
    }
}
*/