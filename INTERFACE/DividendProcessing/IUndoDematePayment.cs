using Entity.Common;

namespace Interface.DividendProcessing
{
    public interface IUndoDematePayment
    {
        JsonResponse GetDividendInformation(string CompCode, string DivCode, string shholderno, string based, string undoType, string UserName, string IP);
        JsonResponse SaveCashDividend(string DivCode, string CompCode, string undoType, string warrantno, string shholderno, string UserName, string IP);
    }
}
