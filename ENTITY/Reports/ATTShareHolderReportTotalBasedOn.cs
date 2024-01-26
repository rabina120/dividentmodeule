namespace Entity.Reports
{
    public class ATTShareHolderReportTotalBasedOn
    {
        public string TotalBasedOn { get; set; }
        public string Location { get; set; }
        public decimal CalculateTotal { get; set; }
        public int TotalIndex { get; set; }

    }
    public class ATTGroupBy
    {
        public string GroupBy { get; set; }
        public int Location { get; set; }

    }
}
