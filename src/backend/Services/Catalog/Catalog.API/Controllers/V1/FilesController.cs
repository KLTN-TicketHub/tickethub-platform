using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Models.Responses;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
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
    }
}
