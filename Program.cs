using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Hosting;
using StartingApp;
using Microsoft.Extensions.DependencyInjection;
namespace YourNamespace
{
    class Program
    {
        static void Main(string[] args)
        {
		

			var certPath = Path.Combine(@"D:\onesiam-cert\", "onesiam.com.pfx");
			Console.WriteLine($" {certPath}");

            var certificate = new X509Certificate2(certPath, "P@ssw0rd");

            var host = new WebHostBuilder()
                .UseKestrel(options =>
                {
                    options.Listen(System.Net.IPAddress.Any, 5000);

                    options.Listen(System.Net.IPAddress.Any, 443, listenOptions =>
                    {
                        listenOptions.UseHttps(certificate);
                    });
                })
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();

            host.Run();

        }
    }


    
}
