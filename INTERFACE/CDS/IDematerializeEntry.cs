using Entity.Common;
using Entity.ShareHolder;
using System.Collections.Generic;

namespace Interface.CDS
{
    public interface IDematerializeEntry
    {
        JsonResponse GetAllParaCompChild(string CompCode);
        JsonResponse GetShHolderInformation(string CompCode, string ShHolderNo, string Occupation, string UserName, string IP);
        JsonResponse GetMaxRegNo(string CompCode);
        JsonResponse GetCertificateDetails(string CompCode, string DemateType, string ShOwnerType, string HolderNo, string SrNoFrom, string SrNoTo, string UserName, string IP);

        JsonResponse GetDataFromCertificateDetail(string CompCode);
        JsonResponse GetMaxDemateRegNo(string CompCode);

        JsonResponse SaveDematerializeCertificate(List<ATTCertDet> CertificateList, string CompCode, string DemateRegNo, string ShHolderNo, string EntryDate, string DemateReqDate, string BOID, string DrnNo, string DPCode, string Remarks, string RegNO, string ISINNo, string BonusCode, string SelectedAction, string UserName, string IP);

        JsonResponse GetSignature(string CompCode, string HolderNo);
        JsonResponse GetHolderByQuery(string CompCode, string FirstName, string Occupation);
        JsonResponse GetDematedCertificateList(string CompCode, string HolderNo);
        JsonResponse GetDematedCertificateDetails(string CompCode, string DemateRegNo);
        JsonResponse GetStartSrNoEndSrNo(string CompCode, string BonusIssueCode);


    }
}
