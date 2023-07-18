using Entity.Common;


namespace Interface.Reports
{
    public interface ISignatureReport
    {
        JsonResponse GenerateReport(string CompCode, string UserName, string SelectedAction, string IPAddress, string DateFrom = null, string DateTo = null, string HolderFrom = null, string HolderTo = null);
    }
}
