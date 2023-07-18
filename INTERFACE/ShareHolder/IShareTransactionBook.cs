using Entity.Common;

namespace Interface.ShareHolder
{
    public interface IShareTransactionBook
    {
        JsonResponse GetShareHolderTransactionBook(string CompCode, string SHNumber, string UserName, string IPAddress);
        JsonResponse GetShareTypes(string CompCode, string SHNumber, string ShareType, string UserName, string IPAddress);
        JsonResponse GetPurchaseSalesReport(string CompCode, string SHNumber, string ShareType, string UserName, string IPAddress, string FileType);
    }
}
