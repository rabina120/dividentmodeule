using System;

namespace Entity.Signature
{
    public class ATTShHolderSignature
    {
        public byte[] signature { get; set; }
        public string ScanedBy { get; set; }
        public int? TotalKitta { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string base64SignatureString { get; set; }
        public string UserName { get; set; }
        public string ImageType { get; set; }
        public int? FileLength { get; set; }
        public string FileName { get; set; }
        public string ShHolderNo { get; set; }
        public string ApprovedBy { get; set; }
        public DateTime? ApprovedDate { get; set; }
        public bool Is_Approved { get; set; }
        public bool PassProcted { get; set; }
    }
}
