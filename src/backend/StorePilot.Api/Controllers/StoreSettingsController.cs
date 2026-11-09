using Microsoft.AspNetCore.Mvc;
using StorePilot.Api.Models;
using StorePilot.Application.Dtos.Settings;
using StorePilot.Application.Intefaces;

namespace StorePilot.Api.Controllers;

[ApiController]
[Route("api/system/settings")]
public sealed class StoreSettingsController(
    IStoreSettingsService settingsService) : ControllerBase
{
    private const long MaxLogoSize = 2 * 1024 * 1024;

    [HttpGet]
    public async Task<ActionResult<StoreSettingsDto>> GetSettings(
        CancellationToken cancellationToken)
    {
        var settings = await settingsService.GetAsync(cancellationToken);
        return Ok(settings);
    }

    [HttpPut]
    public async Task<ActionResult<StoreSettingsDto>> SaveSettings(
        [FromBody] UpdateStoreSettingsRequestModel model,
        CancellationToken cancellationToken)
    {
        await settingsService.SaveAsync(
            model.ShopName,
            model.OwnerName,
            cancellationToken);

        var settings = await settingsService.GetAsync(cancellationToken);

        return Ok(settings);
    }

    [HttpPost("logo")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(MaxLogoSize + 1024)]
    public async Task<IActionResult> UploadLogo(
        [FromForm] IFormFile? file,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(new
            {
                message = "A logo file is required."
            });
        }

        if (file.Length > MaxLogoSize)
        {
            return BadRequest(new
            {
                message = "Logo size must not exceed 2 MB."
            });
        }

        var extension = Path.GetExtension(file.FileName)
            .ToLowerInvariant();

        if (extension is not (".png" or ".jpg" or ".jpeg" or ".webp"))
        {
            return BadRequest(new
            {
                message = "Only PNG, JPG, JPEG and WEBP images are supported."
            });
        }

        await using var memoryStream = new MemoryStream();

        await file.CopyToAsync(memoryStream, cancellationToken);

        var content = memoryStream.ToArray();

        if (!HasValidImageSignature(content, extension))
        {
            return BadRequest(new
            {
                message = "The uploaded file is not a valid supported image."
            });
        }

        await settingsService.SaveLogoAsync(
            content,
            extension,
            cancellationToken);

        var settings = await settingsService.GetAsync(cancellationToken);

        return Ok(settings);
    }

    [HttpGet("logo")]
    public async Task<IActionResult> GetLogo(
        CancellationToken cancellationToken)
    {
        var logo = await settingsService.GetLogoAsync(cancellationToken);

        if (logo is null)
            return NotFound();

        return File(logo.Content, logo.ContentType);
    }

    [HttpDelete("logo")]
    public async Task<IActionResult> DeleteLogo(
        CancellationToken cancellationToken)
    {
        await settingsService.DeleteLogoAsync(cancellationToken);

        return NoContent();
    }

    private static bool HasValidImageSignature(
        byte[] content,
        string extension)
    {
        return extension switch
        {
            ".png" =>
                content.Length >= 8 &&
                content[0] == 0x89 &&
                content[1] == 0x50 &&
                content[2] == 0x4E &&
                content[3] == 0x47 &&
                content[4] == 0x0D &&
                content[5] == 0x0A &&
                content[6] == 0x1A &&
                content[7] == 0x0A,

            ".jpg" or ".jpeg" =>
                content.Length >= 3 &&
                content[0] == 0xFF &&
                content[1] == 0xD8 &&
                content[2] == 0xFF,

            ".webp" =>
                content.Length >= 12 &&
                content[0] == (byte)'R' &&
                content[1] == (byte)'I' &&
                content[2] == (byte)'F' &&
                content[3] == (byte)'F' &&
                content[8] == (byte)'W' &&
                content[9] == (byte)'E' &&
                content[10] == (byte)'B' &&
                content[11] == (byte)'P',

            _ => false
        };
    }
}
