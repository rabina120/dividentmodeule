namespace Entity.Common
{
    public class ATTDataTableRequest
    {
        public int Draw { get; set; }
        public int Start { get; set; }
        public int Length { get; set; }

        public ATTDataTableOrder[] Order { get; set; }
        public ATTDataTableColumn[] Columns { get; set; }
        public ATTDataTableSearch Search { get; set; }
    }
}
