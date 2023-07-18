using Entity.Certificate;
using System;
using System.Collections.Generic;

namespace Entity.Reports
{
    public class ATTConsolidateReport
    {
        public string CompCode { get; set; }
        public string CompEnName { get; set; }
        public string SelectedAction { get; set; }
        public string DataType { get; set; }
        public string ConsolidateDate { get; set; }
        public string ConsolidateTo { get; set; }
        public int? HolderNoFrom { get; set; }
        public int? HolderNoTo { get; set; }
        public string ShareType { get; set; }
        public string ShOwnerTypeName { get; set; }
        public int? CertStatus { get; set; }
        public int? certno { get; set; }
        public string split_no { get; set; }
        public string split_remarks { get; set; }
        public DateTime split_dt { get; set; }
        public string kitta { get; set; }
        public string seqno { get; set; }

        public string compcode { get; set; }
        public int? ShholderNo { get; set; }
        public string Title { get; set; }
        public string FName { get; set; }
        public string LName { get; set; }
        public string Address { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string DistCode { get; set; }
        public string PboxNo { get; set; }
        public string NpTitle { get; set; }
        public string NpName { get; set; }
        public string NpAdd { get; set; }
        public string FaName { get; set; }
        public string GrFaName { get; set; }
        public string HusbandName { get; set; }
        public string TelNo { get; set; }
        public string MobileNo { get; set; }

        public string NomineeName { get; set; }
        public string Relation { get; set; }
        public bool Minor { get; set; }
        public int? TotalKitta { get; set; }
        public int? ShKitta { get; set; }
        public string srnofrom { get; set; }
        public string srnoto { get; set; }
        public string UserName { get; set; }
        public string Entrydate { get; set; }
        public string ActionType { get; set; }
        public string ShOwnerType { get; set; }
        public string ShownerSubtype { get; set; }

        public string Approved { get; set; }
        public string AppStatus { get; set; }
        public string ApprovedBy { get; set; }
        public string ApprovedDate { get; set; }
        public string Remarks { get; set; }
        public string AppRemarks { get; set; }
        public string HolderLock { get; set; }
        public int? refAppNo { get; set; }

        public string BOID { get; set; }

        public List<ATTCertificateConsolidate> aTTCertificateConsolidate { get; set; }
        public bool IsExisting { get; set; }
    }
}
