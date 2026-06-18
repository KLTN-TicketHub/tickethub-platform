using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Models.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SixLabors.ImageSharp;

namespace Catalog.API.Controllers.V1.Common
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/common/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FilesController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FilesController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [AllowAnonymous]
        [HttpPost("upload-svg")]
        public async Task<IActionResult> UploadSvg(IFormFile file, CancellationToken cancellation = default)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Tệp SVG không được để trống."
                });

            if (file.ContentType != "image/svg+xml" &&
                !Path.GetExtension(file.FileName).Equals(".svg", StringComparison.OrdinalIgnoreCase))
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Tệp phải có định dạng SVG."
                });

            string url = await _fileService.SaveFileAsync(file, "seatmaps", cancellation);

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Tải lên tệp SVG thành công.",
                Data = _fileService.GetAbsoluteUrl(url)
            });
        }

        [AllowAnonymous]
        [HttpPost("upload-cover-image")]
        public async Task<IActionResult> UploadCoverImage(IFormFile file, CancellationToken cancellation = default)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Ảnh bìa không được để trống."
                });

            string[] allowedContentTypes = { "image/jpeg", "image/jpg", "image/png", "image/webp" };
            string extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };

            if (!allowedContentTypes.Contains(file.ContentType) ||
                !allowedExtensions.Contains(extension))
                return BadRequest(new ApiResponse
                {
                    Success = false,
                    Message = "Ảnh bìa phải có định dạng JPG, PNG hoặc WebP."
                });

            using (var stream = file.OpenReadStream())
            {
                using var image = await Image.LoadAsync(stream, cancellation);
                if (image.Width != 1280 || image.Height != 720)
                    return BadRequest(new ApiResponse
                    {
                        Success = false,
                        Message = $"Ảnh bìa phải có kích thước 1280×720 px. Kích thước hiện tại: {image.Width}×{image.Height} px."
                    });
            }

            string url = await _fileService.SaveFileAsync(file, "events/covers", cancellation);

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Tải lên ảnh bìa sự kiện thành công.",
                Data = _fileService.GetAbsoluteUrl(url)
            });
        }
    }
}
