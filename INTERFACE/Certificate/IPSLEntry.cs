using Entity.Certificate;
using Entity.Common;
using System.Collections.Generic;
namespace Interface.Certificate
{
    public interface IPSLEntry
    {
        JsonResponse GetShholderDetailsByShHolderNo(string CompCode, int ShholderNo, string SelectedAction, int pslno, string UserName, string IP);
        JsonResponse Getstatus(string Trantype);


        JsonResponse GethlderinfoBysearch(string CompCode, string UserName);
        JsonResponse SavePslBatchEntry(List<ATTPSLEntry> PSLEntry, string CompCode, int ShholderNo, string Code, string Remark, string Transdate, string SelectedAction, string UserName, string Pleggeamount, string Status, int PSLNo, string IP, float charge);
        JsonResponse InsertCertnoInfo(List<ATTPSLEntry> aTTPSLEntry);
        JsonResponse GetAllPledgeAt();
        JsonResponse GetHolderByQuery(string CompCode, string FirstName, string LastName, string FatherName, string GrandFatherName, string UserName, string IP);


    }
}
