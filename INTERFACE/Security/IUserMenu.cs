


using Entity.Common;

namespace Interface.Security
{
    public interface IUserMenu
    {
        public JsonResponse GetUserMenu(string RoleId);

        public JsonResponse GetMenuList();
        public JsonResponse AddRights(string userId, string[] menuList, string addUpdate, string UserName);
        JsonResponse GetMenuByRole(int roleId, string UserName, string IPAddress);
        public JsonResponse AddRightsByRole(string roleId, string[] menuList, string addUpdate, string UserName, string IPAddress);
    }
}
