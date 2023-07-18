using Entity.CDS;
using Entity.Common;

using System.Collections.Generic;

namespace Interface.CDS
{
    public interface IReversalPosting
    {
        JsonResponse GetDataForPosting(string CompCode, string FromDate, string ToDate, string UserName, string IP);
        JsonResponse ViewSingleRematerializeDetail(string CompCode, string RevTranNo, string RegNo, string DrnNo, string UserName, string IP);
        JsonResponse PostData(string Compcode, string SelectedAction, string Remarks, string PostingDate, List<ATTReversalCertificate> ReversalCertificates, string UserName, string IP);

    }
}
