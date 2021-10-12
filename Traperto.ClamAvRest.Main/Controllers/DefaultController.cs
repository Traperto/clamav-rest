using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace VirusScannerService.Controllers
{
    [ApiController]
    [Route("")]
    public class DefaultController : ControllerBase
    {
        private const string SuccessResultText = "Clamd responding: true";

        [HttpGet]
        public async Task<OkObjectResult> Scan()
        {
            return Ok(SuccessResultText);
        }
    }
}