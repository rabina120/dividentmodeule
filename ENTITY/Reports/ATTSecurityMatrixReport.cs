namespace ENTITY.Reports
{
    public class ATTSecurityMatrixReport
    {
        public string RoleName { get; set; }
        public string RoleDescription { get; set; }
        public string Permissions { get; set; }
        public string IndexSheet { get; set; } = "";
        public string HPProfileDeclaration { get; set; } = "";
        public ATTSecurityMatrixReport(string roleName, string roleDescription, string permissions)
        {
            RoleName = roleName;
            RoleDescription = roleDescription;
            Permissions = permissions;
        }
    }
}
