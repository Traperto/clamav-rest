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
#pragma warning disable 1998
        public async Task<OkObjectResult> Scan()
#pragma warning restore 1998
        {
            return Ok(SuccessResultText);
        }
    }
}