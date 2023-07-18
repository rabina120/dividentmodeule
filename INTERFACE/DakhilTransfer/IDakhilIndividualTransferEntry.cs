

using Entity.Common;
using Entity.DakhilTransfer;

namespace Interface.DakhilTransfer
{
    public interface IDakhilIndividualTransferEntry
    {
        JsonResponse GetCertificateInformation(string CompCode, string CertificateNo, string SelectedAction, string UserName, string IP);
        JsonResponse GetBrokerList(string CompCode);
        JsonResponse GetBuyerInformation(string CompCode, string BHolderNo, string UserName, string IPAddress);
        JsonResponse SaveDakhilTransfer(ATTDakhilTransfer DakhilTransferData, string SelectedAction, string UserName, string IP);
    }
}
