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

        private readonly ClamClient _clamClient;
        private readonly ILogger<ScanController> _logger;

        public ScanController(ILogger<ScanController> logger)
        {
            // Initialize settings via. dependencyInjection
            _logger = logger;

            // Initialize settings via. environmentVariables
            var port = int.Parse(Environment.GetEnvironmentVariable("PORT")
                                 ?? "3310");

            var host = Environment.GetEnvironmentVariable("HOST");
            var ipAddress = Environment.GetEnvironmentVariable("IP");
            var maxStreamSize = Environment.GetEnvironmentVariable("MAX_STREAM_SIZE");
            var maxChunkSize = Environment.GetEnvironmentVariable("MAX_CHUNK_SIZE");

            // Initialize ClamAV Client
            if (ipAddress != null)
            {
                _clamClient = new ClamClient(IPAddress.Parse(ipAddress), port);
            }
            else if (host != null)
            {
                _clamClient = new ClamClient(host, port);
            }
            else
            {
                throw new ArgumentNullException(nameof(Environment), "Invalid configuration!");
            }

            // Setup some settings, such as MaxStreamSize and MaxChunkSize
            if (maxStreamSize != null)
            {
                _clamClient.MaxStreamSize = int.Parse(maxStreamSize);
            }

            if (maxChunkSize != null)
            {
                _clamClient.MaxChunkSize = int.Parse(maxChunkSize);
            }
        }

        [HttpPost]
        public async Task<OkObjectResult> Scan(IFormFile file)
        {
            var stream = file.OpenReadStream();
            var scanResult = await _clamClient.SendAndScanFileAsync(stream);

            if (scanResult.InfectedFiles is { Count: > 0 })
            {
                _logger.LogWarning($"File {file.FileName} infected");
                return Ok(InfectionsResultText);
            }

            _logger.LogInformation($"File {file.FileName} ok");
            return Ok(SuccessResultText);
        }
    }
}