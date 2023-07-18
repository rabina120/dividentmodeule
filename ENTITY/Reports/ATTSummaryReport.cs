namespace Entity.Reports
{
    public class ATTSummaryReport
    {

        public ATTSummaryReport1 aTTSummaryReport1 { get; set; }
        public ATTSummaryReport2 aTTSummaryReport2 { get; set; }
        public ATTSummaryReport3 aTTSummaryReport3 { get; set; }
        public ATTSummaryReport4 aTTSummaryReport4 { get; set; }
        public ATTSummaryReport5 aTTSummaryReport5 { get; set; }
        public ATTSummaryReport6 aTTSummaryReport6 { get; set; }
        public ATTSummaryReport7 aTTSummaryReport7 { get; set; }
        public ATTSummaryReport8 aTTSummaryReport8 { get; set; }
        public ATTSummaryReport9 aTTSummaryReport9 { get; set; }
        public ATTSummaryReport10 aTTSummaryReport10 { get; set; }

    }

    public class ATTSummaryReport1
    {
        public int? certstatus { get; set; }
        public int? tcount { get; set; }
        public int? tkitta { get; set; }
    }

    public class ATTSummaryReport2
    {
        public int? sharetype { get; set; }
        public int? tcount { get; set; }
        public int? tkitta { get; set; }

    }
    public class ATTSummaryReport3
    {
        public int? shownertype { get; set; }
        public int? tcount { get; set; }
        public int? tkitta { get; set; }

    }
    public class ATTSummaryReport4
    {
        public int? tcount { get; set; }
        public int? tkitta { get; set; }

    }
    public class ATTSummaryReport5
    {
        public int? tcount { get; set; }
        public int? tkitta { get; set; }

    }
    public class ATTSummaryReport6
    {
        public int? tcount { get; set; }
        public int? tkitta { get; set; }

    }
    public class ATTSummaryReport7
    {
        public int? tcount { get; set; }
        public bool transferred { get; set; }

    }

    public class ATTSummaryReport8
    {
        public int? tcount { get; set; }
        public int? tkitta { get; set; }
        public bool minor { get; set; }

    }

    public class ATTSummaryReport9
    {
        public float? FracKitta { get; set; }
    }


    public class ATTSummaryReport10
    {
        public int? tcount { get; set; }
        public int? tkitta { get; set; }
        public bool distcert { get; set; }
    }


}
