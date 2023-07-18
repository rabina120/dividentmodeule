﻿namespace Entity.Common
{
    public class ATTDataTableResponse<T>
    {
        public int? Draw { get; set; }
        public int? RecordsTotal { get; set; }
        public int? RecordsFiltered { get; set; }
        public T[] Data { get; set; }
        public string Error { get; set; }
    }
}
