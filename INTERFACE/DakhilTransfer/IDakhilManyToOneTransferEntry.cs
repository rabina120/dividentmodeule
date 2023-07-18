

using Entity.Common;
using Entity.DakhilTransfer;
using System.Collections.Generic;

namespace Interface.DakhilTransfer
{
    public interface IDakhilManyToOneTransferEntry
    {
        JsonResponse GetBrokerList(string CompCode);
        JsonResponse GetBuyerInformation(string CompCode, string BHolderNo, string UserName, string IP);
        JsonResponse GetSellerCertificateInformation(string CompCode, string CertificateNo, string UserName, string IPAddress);
        JsonResponse GetMaxRegNo(string CompCode);
        JsonResponse SaveBatchDakhilTransfer(string CompCode, string BuyerHolderNo, List<ATTDakhilSellerInformation> sellers, int? Letter, int? TradeType,
            string Broker, string DakhilDate, string UserName, string IP);
    }
}
