

using Entity.Common;
using Entity.DakhilTransfer;
using System.Collections.Generic;

namespace Interface.DakhilTransfer
{
    public interface IDakhilShareTransfer
    {
        JsonResponse GetShareTransferList(string CompCode, string RegNoFrom, string RegNoTo, string DateFrom, string DateTo, string UserName, string IPAddress);
        JsonResponse GetIndividualShareTransferList(string CompCode, string RegNo, string BHolderNo, string UserName, string IPAddress);
        JsonResponse SaveShareTransfer(string CompCode, List<ATTShareDakhilTransfer> aTTShareDakhilTransfers, string UserName, string IPAddress, string TransferedDate, string SelectedAction, string FolioNo = null, string BatchNo = null);
    }
}
