namespace VirusScannerService.Endpoints;

public static class Scan
{
    private const string SuccessResultText = "Everything ok : true";
    private const string InfectionsResultText = "Everything ok : false";

    public static void RegisterScanEndpoints(this IEndpointRouteBuilder routes)
    {
        var scans = routes.MapGroup("/scan");

        scans.MapPost("", async (
            ILogger<Startup> logger,
            IFormFile file,
            CancellationToken cancellationToken,
            ClamClient clamClient
        ) =>
        {
            var stream = file.OpenReadStream();
            var scanResult =
                await clamClient.SendAndScanFileAsync(stream, cancellationToken);
            
            if (scanResult.InfectedFiles is { Count: > 0 })
            {
                logger.LogWarning("File {fileName} infected", file.FileName);
                return Results.Ok(InfectionsResultText);
            }

            if (scanResult.Result.Equals(ClamScanResults.Clean))
            {
                logger.LogInformation("File {fileName} ok", file.FileName);
                return Results.Ok(SuccessResultText);
            }

            logger.LogWarning("Error when processing file {fileName}", file.FileName);
            return Results.BadRequest(scanResult.RawResult);
        }).DisableAntiforgery();
    }
}