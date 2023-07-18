namespace Entity.Security
{
    public class ATTMenu
    {
        public int MenuId { get; set; }

        public string MenuText { get; set; }

        public char MenuRole { get; set; }
        public string ToolTip { get; set; }

        public string MUrl { get; set; }

        public int OrderNo { get; set; }

        public int ParentId { get; set; }

        public int Level { get; set; }

        public string HasChild { get; set; }

        public string FontAwesome { get; set; }

        public string CollapseTarget { get; set; }
        public string UserName { get; set; }

    }
}
