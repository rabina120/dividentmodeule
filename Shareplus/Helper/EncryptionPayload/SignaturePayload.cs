/*using java.security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CDSMODULE.Helper.EncryptionPayload
{
    public class SignaturePayload
    {
        public static byte[] generateSignature(byte[] data, string privateKeyPath)
        {
            Signature signature = Signature.getInstance("SHA256withRSA");
            signature.initSign(PrivatePublicKey.getPrivateKey(privateKeyPath));
            signature.update(data);
            return signature.sign();
        }

        public static bool verifySignature(string data, string esewaPublicKey, byte[] signature)
        {
            Signature publicSignature = Signature.getInstance("SHA256withRSA");
            publicSignature.initVerify(PrivatePublicKey.getPublicKey(esewaPublicKey));
            publicSignature.update(Encoding.UTF8.GetBytes(data));
            return publicSignature.verify(signature);
        }


    }
}
*/