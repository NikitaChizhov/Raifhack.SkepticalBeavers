namespace Raifhack.SkepticalBeavers.Server.Contracts.Responses
{
    public sealed class LoginResponse
    {
        public string UserName { get; set; }

        public string Role { get; set; }

        public string AccessToken { get; set; }
    }
}