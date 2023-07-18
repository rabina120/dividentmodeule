
using Entity.Common;
using Entity.ShareHolder;
using System.Collections.Generic;

namespace Interface.Certificate
{
    public interface ICERTIFICATEDISTRIBUTIONENTRY
    {

        JsonResponse GET_SHHOLDER_DISTRIBUTE(string CompCode, string ShholderNo, string UserName, string IP);
        JsonResponse GET_SHHOLDER_CERTDISTRIBUTE(string CompCode, int ShholderNo, string Action);

        JsonResponse SaveDistributionCertificate(List<ATTCertDet> certificateList, string compCode, string certno, string selectedAction, string DistDate, string Username, string IP);
    }
}
