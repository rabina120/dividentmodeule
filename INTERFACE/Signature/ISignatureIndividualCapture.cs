
using Entity.Common;

namespace Interface.Signature
{
    public interface ISignatureIndividualCapture
    {
        JsonResponse GetShHolderInformation(string CompCode, string ShHolderNo, string SelectedAction, string UserName, string IPAddress);
        JsonResponse SaveSignatureInformation(string CompCode, string ShHolderNo, string ScannedBy, byte[] Signature, int? fileLength, string SelectedAction, string UserName, string ip);
    }
}
