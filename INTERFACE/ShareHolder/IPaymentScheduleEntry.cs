
using Entity.Common;

namespace Interface.ShareHolder
{
    public interface IPaymentScheduleEntry
    {
        JsonResponse SavePaymentSchedule(string CompCode, string DivCode, string ShareType, string UserName);

    }
}
