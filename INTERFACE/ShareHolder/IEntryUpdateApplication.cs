
using Entity.Common;

namespace Interface.ShareHolder
{
    public interface IEntryUpdateApplication
    {
        JsonResponse GetInformationForApplication(string CompCode, string ShHolderNo, string SelectedAction, string UserName, string IP, string ApplicationNo = null);
        JsonResponse GetInformationFromApplicationNo(string CompCode, string SelectedAction, string UserName, string IP, string ApplicationNo);
        JsonResponse SaveApplicationForUpdate(string CompCode, string ShHolderNo, string ApplicationDate, string Action, string SelectedAction, string UserName, string IPAddress, string ApplicationNo = null);

    }
}
