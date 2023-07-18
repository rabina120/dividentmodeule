namespace Entity.Dividend
{
    public class ATTDividendSummary
    {

        public string ShholderNo { get; set; }
        public string WarrantAmt { get; set; }
        public string TaxDAmt { get; set; }
        public string BonusTax { get; set; }
        public string NetAmountPaid { get; set; }
        public string Balance { get; set; }

        public string BonusAdj { get; set; }

        public string WAmtPaidDt { get; set; }
    }
}
