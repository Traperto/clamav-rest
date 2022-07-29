using System;
using System.Net;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using nClam;

namespace VirusScannerService
{
    public class Startup
    {
        public Startup()
        {
            var builder = new ConfigurationBuilder()
                .AddEnvironmentVariables();
            builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            
            services.Configure<FormOptions>(fo =>
            {
                fo.ValueLengthLimit = int.MaxValue;
                fo.MultipartBodyLengthLimit = int.MaxValue; // In case of multipart
            });

            services.AddScoped(_ =>
            {
                var port = int.Parse(Environment.GetEnvironmentVariable("PORT") ?? "3310");
                var host = Environment.GetEnvironmentVariable("HOST");
                var ip = Environment.GetEnvironmentVariable("IP");
                var maxStreamSize = Environment.GetEnvironmentVariable("MAX_STREAM_SIZE");
                var maxChunkSize = Environment.GetEnvironmentVariable("MAX_CHUNK_SIZE");

                ClamClient clamClient;

                if (ip != null)
                {
                    clamClient = new ClamClient(IPAddress.Parse(ip), port);
                }
                else if (host != null)
                {
                    clamClient = new ClamClient(host, port);
                }
                else
                {
                    throw new Exception("no valid configuration");
                }

                if (maxStreamSize != null)
                {
                    clamClient.MaxStreamSize = int.Parse(maxStreamSize); 
                }
            
                if (maxChunkSize != null)
                {
                    clamClient.MaxChunkSize = int.Parse(maxChunkSize); 
                }

                return clamClient;
            });
            
            services.Configure<KestrelServerOptions>(options => options.Limits.MaxRequestBodySize = int.MaxValue);
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}