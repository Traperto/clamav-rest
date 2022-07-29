using System.Threading;
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
        private const string _SUCCESS_RESULT_TEXT = "Everything ok : true";
        private const string _INFECTIONS_RESULT_TEXT = "Everything ok : false";

        private readonly ILogger<ScanController> _logger;
        private readonly ClamClient _clamClient;

        public ScanController(
            ILogger<ScanController> logger,
            ClamClient clamClient
        )
        {
            _logger = logger;
            _clamClient = clamClient;
        }

        [HttpPost]
        public async Task<OkObjectResult> Scan(IFormFile file, CancellationToken cancellationToken)
        {
            var stream = file.OpenReadStream();
            var scanResult = await _clamClient.SendAndScanFileAsync(stream, cancellationToken);

            if (scanResult.InfectedFiles is { Count: > 0 })
            {
                _logger.LogWarning("File {fileName} infected", file.FileName);
                return Ok(_INFECTIONS_RESULT_TEXT);
            }

            _logger.LogInformation("File {fileName} ok", file.FileName);
            return Ok(_SUCCESS_RESULT_TEXT);
        }
    }
}