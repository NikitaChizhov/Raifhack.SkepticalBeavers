using Raifhack.SkepticalBeavers.Server.Model;

namespace Raifhack.SkepticalBeavers.Server.Services
{
    internal interface IUserService
    {
        bool IsValidUserCredentials(string userName, string password, out User? user);
    }
}