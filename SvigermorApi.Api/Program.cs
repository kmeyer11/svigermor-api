using SvigermorApi.Api;
using SvigermorApi.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<GeminiTranslationService>();
builder.Services.AddScoped<ITranslationService, GeminiTranslationService>();

var app = builder.Build();

app.MapPost("/translate", async (TranslationRequest request, ITranslationService service) =>
{
    try
    {
        var result = await service.Translate(request);
        return Results.Ok(result);
    }
    catch (InvalidOperationException ex)
    {
        return Results.Problem(
            detail: ex.Message,
            statusCode: StatusCodes.Status500InternalServerError,
            title: "Translation configuration error");
    }
    catch (HttpRequestException ex)
    {
        int statusCode = ex.StatusCode.HasValue
            ? (int)ex.StatusCode.Value
            : StatusCodes.Status502BadGateway;

        return Results.Problem(
            detail: ex.Message,
            statusCode: statusCode,
            title: "Gemini API error");
    }
});

app.Run();