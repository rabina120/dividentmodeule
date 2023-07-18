namespace Entity.Common
{
    public class ATTDataTableColumn
    {
        public string Data { get; set; }
        public string Name { get; set; }
        public bool Searchable { get; set; }
        public bool Orderable { get; set; }

        public ATTDataTableSearch Search { get; set; }
    }
}
