using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using nClam;

namespace VirusScannerService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ScanController : ControllerBase
    {

        private const string SuccessResultText = "Everything ok : true";
        private const string InfectionsResultText = "Everything ok : false";

        private readonly ILogger<ScanController> _logger;

        public ScanController(
            ILogger<ScanController> logger
        )
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<OkObjectResult> Scan(IFormFile file)
        {
            var port = int.Parse(Environment.GetEnvironmentVariable("PORT") ?? "3310");
            var host = Environment.GetEnvironmentVariable("HOST");
            var ip = Environment.GetEnvironmentVariable("IP");
            var maxStreamSize = Environment.GetEnvironmentVariable("MAX_STREAM_SIZE");
            var maxChunkSize = Environment.GetEnvironmentVariable("MAX_CHUNK_SIZE");

            ClamClient clam;

            if (ip != null)
            {
                clam = new ClamClient(IPAddress.Parse(ip), port);
            }
            else if (host != null)
            {
                clam = new ClamClient(host, port);
            }
            else
            {
                throw new Exception("no valid configuration");
            }

            if (maxStreamSize != null)
            {
                clam.MaxStreamSize = int.Parse(maxStreamSize); 
            }
            
            if (maxChunkSize != null)
            {
                clam.MaxChunkSize = int.Parse(maxChunkSize); 
            }

            var stream = file.OpenReadStream();
            var scanResult = await clam.SendAndScanFileAsync(stream);

            if (scanResult.InfectedFiles != null && scanResult.InfectedFiles.Count > 0)
            {
                
                _logger.LogWarning($"File {file.FileName} infected");
                
                return Ok(InfectionsResultText);
            }
            
            _logger.LogInformation($"File {file.FileName} ok");

            return Ok(SuccessResultText);
        }
    }
}