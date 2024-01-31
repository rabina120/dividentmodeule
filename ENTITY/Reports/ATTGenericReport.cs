namespace Entity.Reports
{
    public class ATTGenericReport
    {
        public float ColumnDefinition { get; set; }
        public string ColumnName { get; set; }
        public string DataBaseColumnName { get; set; }
        public string[] ReportTitle { get; set; }

        public enum ReportName
        {

            UserAuditReport,
            ListIssuedDividendWarrantPhysicalPosted,
            ListIssuedDividendWarrantPhysicalUnposted,
            ListIssuedDividendWarrantDematePosted,
            ListIssuedDividendWarrantDemateUnposted,
            ListUnIssuedDividendWarrantPhysicalPosted,
            ListUnIssuedDividendWarrantPhysicalUnposted,
            ListUnIssuedDividendWarrantDematePosted,
            ListUnIssuedDividendWarrantDemateUnposted,
            ListPaidDividendWarrantPhysicalPosted,
            ListPaidDividendWarrantPhysicalUnposted,
            ListPaidDividendWarrantDematePosted,
            ListPaidDividendWarrantDemateUnposted,
            ListUnPaidDividendWarrantPhysicalPosted,
            ListUnPaidDividendWarrantPhysicalUnposted,
            ListUnPaidDividendWarrantDematePosted,
            ListUnPaidDividendWarrantDemateUnposted,
            UndoReportDemate,
            UndoReportPhysical,
            SummaryReportOfIssuedAndPaidPhysical,
            SummaryReportOfIssuedAndPaidDemate,
            ReversalReport,
            DailyReport,
            DemateRemateReport,
            ShareHolderAddressTableListReport,
            ShareHolderShareHolderListInEnglish,
            ShareHolderShareHolderListInNepali,
            ShareHolderShareHolderAttendanceList,
            ShareHolderShareHoldersDetailsList,
            ShareHolderShareHolderslistInEnglishZeroKitta,
            ShareHolderFractionList,
            AllShareHolderFractionList,
            HolderBOIDHistory,
            AllCertificateList,
            DistributedUndistributedCertificateList,
            SecurityMatrixForSCB,
            UserRoleUpdateReport,
            DakhilList,
            TransferList,
            DakhilKharejBook,
            DakhilKittaReport,
            DakhilBuyerList,
            DakhilSellerList,
            BulkCA,


            SharePurchaseSalesReport,

            ValidAccount,
            InValidAccount,
            TransactionFailedToProcess,
            TranasctionProcessed,
            TransactionStatusProcessing,
            TransactionStatusSuccess,
            TransactionStatusFailed,


            PSLReportUnclearUnposted,
            PSLReportUnclearPosted,
            PSLReportUnclearRejected,
            PSLReportClearUnposted,
            PSLReportClearPosted,
            PSLReportClearRejected,



            listOfIssuedAndUnPaidDividendWarrantPhysicalUnposted,
            listOfIssuedAndUnPaidDividendWarrantPhysicalPosted,
            listOfIssuedAndUnPaidDividendWarrantDemateUnposted,
            listOfIssuedAndUnPaidDividendWarrantDematePosted,

            ShareholderLockReportPosted,
            ShareholderLockReportUnposted,
            ShareholderUnlockReportPosted,
            ShareholderUnlockReportUnposted,


            DematSummaryReport,
            DematDetailReport,
            DematDrnReport,
            RematSummaryReport,
            RematDetailReport,
            RematDrnReport,
            SignatureReport



        }
        //single instance of a table header
        public ATTGenericReport(float _ColumnDefinition, string _ColumnName, string _DataBaseColumnName)
        {
            this.ColumnDefinition = _ColumnDefinition;
            this.ColumnName = _ColumnName;
            this.DataBaseColumnName = _DataBaseColumnName;
        }
    }
}
