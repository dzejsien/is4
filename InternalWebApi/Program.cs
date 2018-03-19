using System.Net;
using System.Security.Cryptography.X509Certificates;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace InternalWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHostOld(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();

        public static IWebHost BuildWebHost(string[] args)
        {
            // https://dotnetcodr.com/2015/06/08/https-and-x509-certificates-in-net-part-4-working-with-certificates-in-code/
            X509Certificate2 findResult = null;
            X509Store computerCaStore = new X509Store(StoreName.My, StoreLocation.LocalMachine);

            try
            {
                computerCaStore.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection certificatesInStore = computerCaStore.Certificates;
                findResult = certificatesInStore.Find(X509FindType.FindBySubjectName, "localhost", false)[0];
            }
            finally
            {
                computerCaStore.Close();
            }

            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseKestrel(options =>
                {
                    //options.Listen(IPAddress.Loopback, 5000);
                    options.Listen(IPAddress.Loopback, 5003, listenOptions =>
                    {
                        listenOptions.UseHttps(findResult);
                    });
                })
                .Build();
        }
    }
}
