using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace WebApi.Controllers
{
    [Route("identity")]
    [Authorize]
    public class IdentityController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }

        [Route("getFromInternal")]
        [HttpGet]
        public async Task<IActionResult> GetFromInternal()
        {
            var disco = await DiscoveryClient.GetAsync("https://localhost:5000");
            if (disco.IsError)
            {

                return new JsonResult(new { result = "NO DISCO" });
            }

            var accessToken = await HttpContext.GetTokenAsync("access_token");

            // request token
            var tokenResponse = await DelegateAsync(disco, accessToken);
            //var tokenResponse = await RequestResourceOwnerPasswordAsync(disco);

            // call api
            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);

            var response = await client.GetAsync("https://localhost:5003/identity");
            if (!response.IsSuccessStatusCode)
            {
                return new JsonResult(response);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                return new JsonResult(JArray.Parse(content));
            }
        }


        private async Task<TokenResponse> DelegateAsync(DiscoveryResponse disco, string userToken)
        {
            var payload = new
            {
                token = userToken
            };

            // create token client
            var client = new TokenClient(disco.TokenEndpoint, "api1.client", "secret");

            // send custom grant to token endpoint, return response
            return await client.RequestCustomGrantAsync("delegation", "api2_internal", payload);
        }
    }
}