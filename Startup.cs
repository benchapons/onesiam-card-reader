using System.Text.Json;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using ThaiNationalIDCard.NET;
using System.Threading;
using System.Threading.Tasks;

namespace StartingApp
{

    public class Startup
    {
		private static int lineId = 0;
        private static object cardReader;
        private static int bufferLine = 1;

        public static async Task<object> CardReaderEvent(CancellationToken ct)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (bufferLine == 5)
                    {
                        Console.Clear();
                        bufferLine = 1;
                    }
                    Console.WriteLine($" {lineId} CardReader ==> Starting....");
                    var thaiNationalIDCardReader = new ThaiNationalIDCardReader();
                    cardReader = new { thaiNationalIDCardReader.GetPersonalPhoto(), thaiNationalIDCardReader.GetPersonal() };//thaiNationalIDCardReader.GetPersonal();
					Console.WriteLine($" {lineId} CardReader ==> Get Data Success....");
				}
				catch (Exception ex)
                {

                    Console.WriteLine($" {lineId} Error ==> {DateTime.Now:dd MMM yyyy hh:mm:ss}");
                    Console.WriteLine($" {lineId} Error Message ==> ");
                    Console.WriteLine($" {ex.Message}");
                    cardReader = new
                    {
                        errorCode = "CD-TE-01",
                        errorMessage = $" {ex.Message}"
                    };
                }
                lineId++;
                bufferLine++;
            });
            return cardReader;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin",
                    builder => builder.WithOrigins("http://localhost:3000", "https://dev-crmportal-web.onesiam.com", "https://qa-crmportal-web.onesiam.com", "https://crmportal-web.onesiam.com")
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseCors("AllowSpecificOrigin");

            app.Run(async context =>
			{
				var cts = new CancellationTokenSource(3000); // ตั้งเวลา 3 วินาที
				try
				{
					var response = await CardReaderEvent(cts.Token);
					var jsonString = JsonSerializer.Serialize(response);
					await context.Response.WriteAsync(jsonString);
				}
				catch (OperationCanceledException)
				{   
                    var response = new
					{
						errorCode = "CD-TE-02",
						errorMessage = "Job Canceled"
					};
					var jsonString = JsonSerializer.Serialize(response);

					await context.Response.WriteAsync(jsonString);
				}
			});

        }

	}

    //public class Startup
    //{
    //    public void ConfigureServices(IServiceCollection services)
    //    {
    //        services.AddCors(options =>
    //        {
    //            options.AddPolicy("AllowSpecificOrigin",
    //                builder => builder.WithOrigins("http://localhost:3000")
    //                .AllowAnyMethod()
    //                .AllowAnyHeader()
    //                .AllowCredentials());
    //        });
    //        services.AddSignalR();

    //    }

    //    public void Configure(IApplicationBuilder app)
    //    {
    //        app.UseRouting();
    //        app.UseCors("AllowSpecificOrigin");
    //        app.UseEndpoints(endpoints =>
    //        {
    //            endpoints.MapHub<ChatHub>("/chatHub");
    //        });
    //    }
    //}

    //public class ChatHub : Microsoft.AspNetCore.SignalR.Hub
    //{





    //    public async System.Threading.Tasks.Task SendMessage(string user, string message)
    //    {
 

    //        Console.WriteLine($" {lineId} : Count ==> {lineId}");

    //        Console.WriteLine($" {lineId} : Thai National ID Card Number ==> {jsonString}");

    //        Console.WriteLine("**************************");
    //        Console.WriteLine("Pass Card System");
    //        Console.WriteLine("**************************");
    //        Console.WriteLine($"Exit Program ===> {DateTime.Now:dd MMM yyyy hh:mm:ss}");

    //        await Clients.All.SendAsync("ReceiveMessage", jsonString);
    //    }
    //}
}
