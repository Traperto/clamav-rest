namespace VirusScannerService.Endpoints;

public static class Available
{
    private const string SuccessResultText = "Clamd responding: true";
    
    public static void RegisterDefaultEndpoints(this IEndpointRouteBuilder routes)
    {
        var defaults = routes.MapGroup("/");

        defaults.MapGet("", () => Results.Ok(SuccessResultText));
    }
}