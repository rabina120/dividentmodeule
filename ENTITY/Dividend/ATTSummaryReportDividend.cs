using ENTITY.Dividend;

namespace Entity.Dividend
{
    public class ATTSummaryReportDividend
    {
        public ATTTotalDividendWarrants aTTTotalDividendWarrants { get; set; }
        public ATTIssuedDividendWarrants aTTIssuedDividendWarrants { get; set; }
        public ATTIssuedPostedDividendWarrants aTTIssuedPostedDividendWarrants { get; set; }
        public ATTIssuedUnpostedDividendWarrants aTTIssuedUnpostedDividendWarrants { get; set; }
        public ATTUnIssuedDividendWarrants aTTUnIssuedDividendWarrants { get; set; }

        public ATTPaidPostedDividendWarrents aTTPaidPostedDividendWarrants { get; set; }
        public ATTPaidUnpostedDividendWarrents aTTPaidUnpostedDividendWarrants { get; set; }
        public ATTUnpaidDividendWarrents aTTUnpaidDividendWarrants { get; set; }

    }
}
