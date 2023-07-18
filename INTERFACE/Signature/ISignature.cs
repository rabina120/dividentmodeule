
using Entity.Common;

namespace Interface.Signature
{
    public interface ISignature
    {
        public JsonResponse GetSignature(string compcode, string holderno);

    }
}
