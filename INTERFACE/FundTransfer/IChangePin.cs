
using Entity.Common;

namespace INTERFACE.FundTransfer
{
    public interface IChangePin
    {         
        JsonResponse ChangePin(string OldPin, string NewPin, string NewCurrentPin, string UserName, string IPAddress);       
        
    }
}
