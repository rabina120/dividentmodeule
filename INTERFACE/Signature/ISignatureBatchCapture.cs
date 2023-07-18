using Entity.Common;
using Entity.Signature;
using System.Collections.Generic;

namespace Interface.Signature
{
    public interface ISignatureBatchCapture
    {
        public JsonResponse SaveBatchSignature(string CompCode, List<ATTShHolderSignature> Signatures, string UserName, string ip);
        public JsonResponse ExportBatchSignature(string CompCode, string StartShHolderNo, string EndShHolderNo, string UserName, string ip);
    }
}
