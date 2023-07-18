using System;
using System.IO;
using System.Security.Cryptography;

namespace Repository.Helper
{
    public class EncryptDecryptSecreteKey
    {
        public static byte[] base64Decode(string data)
        {
            return Convert.FromBase64String(data);

        }

        public static string base64Encode(byte[] data)
        {
            return Convert.ToBase64String(data);
        }


        public static byte[] getSecretKey(string secretKey)
        {
            byte[] decodeSecretKey = base64Decode(secretKey);
            return decodeSecretKey;

        }


        public static string keyToString(byte[] secretKey)
        {
            string encodedKey = base64Encode(secretKey);
            return encodedKey;
        }




        public static string encrypt(string strToEncrypt, byte[] secretKey)
        {
            byte[] iv = new byte[16];
            byte[] array;

            using (Aes aes = Aes.Create())
            {
                aes.Key = secretKey;
                aes.IV = iv;
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(strToEncrypt);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);

        }

        //ok
        public static string decrypt(string strToDecrypt, byte[] secretKey)
        {
            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(strToDecrypt);

            using (Aes aes = Aes.Create())
            {
                aes.Key = secretKey;
                aes.IV = iv;
                aes.Mode = CipherMode.ECB;
                aes.Padding = PaddingMode.PKCS7;


                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream memoryStream = new MemoryStream(buffer))
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                        {
                            return streamReader.ReadToEnd();
                        }
                    }
                }
            }


        }
    }
}
