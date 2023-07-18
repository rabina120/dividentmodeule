using Microsoft.AspNetCore.Mvc;


namespace Interface.Common
{
    public interface ICheckUserAccess
    {
        bool CheckIfAccessible(string UserId, ControllerContext controllerContext);
    }
}
