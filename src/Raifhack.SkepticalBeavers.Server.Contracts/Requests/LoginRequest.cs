using Newtonsoft.Json;

namespace Raifhack.SkepticalBeavers.Server.Contracts.Requests
{
    public sealed class LoginRequest
    {
        [JsonProperty(Required = Required.Always)]
        public string UserName { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Password { get; set; }
    }
}