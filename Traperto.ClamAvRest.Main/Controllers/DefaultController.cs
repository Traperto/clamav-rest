using Microsoft.AspNetCore.Mvc;

namespace VirusScannerService.Controllers
{
    [ApiController]
    [Route("")]
    public class DefaultController : ControllerBase
    {
        private const string _SUCCESS_RESULT_TEXT = "Clamd responding: true";

        [HttpGet]
        public OkObjectResult Scan()
        {
            return Ok(_SUCCESS_RESULT_TEXT);
        }
    }
}