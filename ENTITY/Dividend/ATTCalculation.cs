
namespace ENTITY.Dividend
{
    public class ATTCalculation
    {
        public string CompanyId { get; set; }

        public int HolderNo  { get; set; }
        public string Boid  { get; set; }
        public string HolderName { get; set; }
        public string Address { get; set; }
        public string TotalKitta { get; set; }
        public decimal FractionKitta { get; set; }
        public decimal Total { get; set; }
        public string Action { get; set; }


        //after calculation fields
        public decimal ActualDividentAmount { get; set; }
        public decimal DividendTax { get; set; }
        public decimal NetPay { get; set; }
        public decimal ActualBonus { get; set; }
        public decimal BonusWithPrevFraction { get; set; }
        public decimal IssueBonus { get; set; }
        public decimal RemainingFraction { get; set; }
        public decimal bonustax { get; set; }

        public string Column_Name { get; set; }



    }
}
