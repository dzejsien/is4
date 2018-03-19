using IdentityModel.Client;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Client
{
    public class Program
    {
        public static void Main(string[] args) => MainAsync().GetAwaiter().GetResult();

        private static async Task MainAsync()
        {
            // discover endpoints from metadata
            var disco = await DiscoveryClient.GetAsync("https://localhost:5000");
            if (disco.IsError)
            {
                Console.WriteLine(disco.Error);
                return;
            }

            // request token
            var tokenResponse = await RequestClientCredentails(disco);
            //var tokenResponse = await RequestResourceOwnerPasswordAsync(disco);

            // call api
            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);

            var response = await client.GetAsync("https://localhost:5001/identity/getFromInternal");
            //below shouldnt work, access to 5003 only from 5001 (above)
            //var response = await client.GetAsync("https://localhost:5003/identity/getFromInternal");
            //var response = await client.GetAsync("https://localhost:5001/identity");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Code:{response.StatusCode}, {response.ReasonPhrase}, {response.Headers.ToString()}");
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
                //Console.WriteLine(content);
            }

            Console.ReadKey();
        }

        private static async Task<TokenResponse> RequestClientCredentails(DiscoveryResponse disco)
        {
            // request token
            var tokenClient = new TokenClient(disco.TokenEndpoint, "client", "secret");
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("api1");

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return null;
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");

            return tokenResponse;
        }

        private static async Task<TokenResponse> RequestResourceOwnerPasswordAsync(DiscoveryResponse disco)
        {
            var tokenClient = new TokenClient(disco.TokenEndpoint, "ro.client", "secret");
            var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync("alice", "password", "api1");

            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return null;
            }

            Console.WriteLine(tokenResponse.Json);
            Console.WriteLine("\n\n");

            return tokenResponse;
        }

    }
}
