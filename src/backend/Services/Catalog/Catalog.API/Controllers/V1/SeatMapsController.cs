using BuildingBlocks.Application.Interfaces;
using BuildingBlocks.Contracts.Models.Responses;
using Catalog.Application.Common.DTOs.SeatMaps;
using Catalog.Application.Features.SeatMaps.Commands.CreateSeatMap;
using Catalog.Application.Features.SeatMaps.Requests;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.API.Controllers.V1
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SeatMapsController : ControllerBase
    {
        private readonly ISender _sender;
        private readonly IFileService _fileService;

        public SeatMapsController(ISender sender, IFileService fileService)
        {
            _sender = sender;
            _fileService = fileService;
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> CreateSeatMap([FromBody] CreateSeatMapRequest request, CancellationToken cancellation = default)
        {
            var result = await _sender.Send(new CreateSeatMapCommand(request), cancellation);

            return Ok(new ApiResponse<SeatMapDto>
            {
                Data = result,
                Success = true,
                Message = "Tạo sơ đồ chỗ ngồi thành công."
            });
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
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetSeatMapById(
            [FromRoute] Guid id, CancellationToken cancellation = default)
        {
            //var result = await _sender.Send(new GetSeatMapByIdRequest(id), cancellation);
            //if (result == null)
            //    return NotFound(new ApiResponse
            //    {
            //        Success = false,
            //        Message = "Không tìm thấy sơ đồ chỗ ngồi."
            //    });
            //return Ok(new ApiResponse<SeatMapDto>
            //{
            //    Data = result,
            //    Success = true,
            //    Message = "Lấy thông tin sơ đồ chỗ ngồi thành công."
            //});

            throw new NotImplementedException();
        }
    }
}
