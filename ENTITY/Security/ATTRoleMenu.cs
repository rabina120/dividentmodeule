namespace Entity.Security
{
    public class ATTRoleMenu
    {
        public int? RoleId { get; set; }
        public string RoleName { get; set; }
        public int? MenuId { get; set; }
        public bool IsAccessed { get; set; }
        public string MenuRole { get; set; }

        public int? Level { get; set; }

    }
}
