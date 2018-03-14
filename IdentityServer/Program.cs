﻿using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace IdentityServer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHostOld(args).Run();
        }

        public static IWebHost BuildWebHostOld(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            // https://dotnetcodr.com/2015/06/08/https-and-x509-certificates-in-net-part-4-working-with-certificates-in-code/
            X509Certificate2 findResult = null;
            X509Store computerCaStore = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            findResult = IdentityModel.X509.CurrentUser.My.SubjectDistinguishedName.Find("web.local").FirstOrDefault();

            try
            {
                computerCaStore.Open(OpenFlags.ReadOnly);
                X509Certificate2Collection certificatesInStore = computerCaStore.Certificates;
                //foreach (X509Certificate2 cert in certificatesInStore)
                //{
                //    Debug.WriteLine(cert.GetExpirationDateString());
                //    Debug.WriteLine(cert.Issuer);
                //    Debug.WriteLine(cert.GetEffectiveDateString());
                //    Debug.WriteLine(cert.GetNameInfo(X509NameType.SimpleName, true));
                //    Debug.WriteLine(cert.HasPrivateKey);
                //    Debug.WriteLine(cert.SubjectName.Name);
                //    Debug.WriteLine("-----------------------------------");
                //}

                findResult = certificatesInStore.Find(X509FindType.FindBySubjectName, "web.local", false)[0];
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
                    options.Listen(IPAddress.Loopback, 5000, listenOptions =>
                    {
                        listenOptions.UseHttps(findResult);
                    });
                })
                .Build();
        }
    }
}
