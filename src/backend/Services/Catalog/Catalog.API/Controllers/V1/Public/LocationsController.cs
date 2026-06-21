using BuildingBlocks.Contracts.Models.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Catalog.API.Controllers.V1.Public
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/locations")]
    [ApiController]
    [AllowAnonymous]
    public class LocationsController : ControllerBase
    {
        private static readonly object _lock = new();
        private static Dictionary<string, ProvinceJsonItem>? _provinces;
        private static Dictionary<string, DistrictJsonItem>? _districts;
        private static Dictionary<string, WardJsonItem>? _wards;
        private readonly IWebHostEnvironment _env;

        public LocationsController(IWebHostEnvironment env)
        {
            _env = env;
        }

        private void InitializeData()
        {
            if (_provinces != null && _districts != null && _wards != null) return;

            lock (_lock)
            {
                if (_provinces != null && _districts != null && _wards != null) return;

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                if (_provinces == null)
                {
                    try
                    {
                        var filePath = Path.Combine(AppContext.BaseDirectory, "Data", "province_old.json");
                        if (System.IO.File.Exists(filePath))
                        {
                            var jsonString = System.IO.File.ReadAllText(filePath);
                            _provinces = JsonSerializer.Deserialize<Dictionary<string, ProvinceJsonItem>>(jsonString, options);
                        }
                    }
                    catch { }
                    _provinces ??= new Dictionary<string, ProvinceJsonItem>();
                }

                if (_districts == null)
                {
                    try
                    {
                        var filePath = Path.Combine(AppContext.BaseDirectory, "Data", "district_old.json");
                        if (System.IO.File.Exists(filePath))
                        {
                            var jsonString = System.IO.File.ReadAllText(filePath);
                            _districts = JsonSerializer.Deserialize<Dictionary<string, DistrictJsonItem>>(jsonString, options);
                        }
                    }
                    catch { }
                    _districts ??= new Dictionary<string, DistrictJsonItem>();
                }

                if (_wards == null)
                {
                    try
                    {
                        var filePath = Path.Combine(AppContext.BaseDirectory, "Data", "ward_old.json");
                        if (System.IO.File.Exists(filePath))
                        {
                            var jsonString = System.IO.File.ReadAllText(filePath);
                            _wards = JsonSerializer.Deserialize<Dictionary<string, WardJsonItem>>(jsonString, options);
                        }
                    }
                    catch { }
                    _wards ??= new Dictionary<string, WardJsonItem>();
                }
            }
        }

        [HttpGet("provinces")]
        public IActionResult GetProvinces()
        {
            InitializeData();
            var provincesList = _provinces?.Values
                .Select(p => new ProvinceDto
                {
                    Code = p.Code,
                    Name = p.NameWithType
                })
                .OrderBy(p => p.Name)
                .ToList() ?? new List<ProvinceDto>();

            return Ok(new ApiResponse<List<ProvinceDto>>
            {
                Success = true,
                Message = "Lấy danh sách tỉnh thành công",
                Data = provincesList
            });
        }

        [HttpGet("provinces/{provinceCode}/districts")]
        public IActionResult GetDistricts(string provinceCode)
        {
            InitializeData();
            var filteredDistricts = _districts?.Values
                .Where(d => d.ParentCode == provinceCode)
                .Select(d => new DistrictDto
                {
                    Code = d.Code,
                    Name = d.NameWithType,
                    ProvinceCode = d.ParentCode
                })
                .OrderBy(d => d.Name)
                .ToList() ?? new List<DistrictDto>();

            return Ok(new ApiResponse<List<DistrictDto>>
            {
                Success = true,
                Message = "Lấy danh sách quận huyện thành công",
                Data = filteredDistricts
            });
        }

        [HttpGet("districts/{districtCode}/wards")]
        public IActionResult GetWards(string districtCode)
        {
            InitializeData();
            var filteredWards = _wards?.Values
                .Where(w => w.ParentCode == districtCode)
                .Select(w => new WardDto
                {
                    Code = w.Code,
                    Name = w.NameWithType,
                    DistrictCode = w.ParentCode
                })
                .OrderBy(w => w.Name)
                .ToList() ?? new List<WardDto>();

            return Ok(new ApiResponse<List<WardDto>>
            {
                Success = true,
                Message = "Lấy danh sách phường xã thành công",
                Data = filteredWards
            });
        }

        private class ProvinceJsonItem
        {
            public string Name { get; set; } = string.Empty;
            public string Slug { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty;

            [JsonPropertyName("name_with_type")]
            public string NameWithType { get; set; } = string.Empty;

            public string Code { get; set; } = string.Empty;
        }

        private class DistrictJsonItem
        {
            public string Name { get; set; } = string.Empty;
            public string Slug { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty;

            [JsonPropertyName("name_with_type")]
            public string NameWithType { get; set; } = string.Empty;

            public string Code { get; set; } = string.Empty;

            [JsonPropertyName("parent_code")]
            public string ParentCode { get; set; } = string.Empty;

            public string Path { get; set; } = string.Empty;

            [JsonPropertyName("path_with_type")]
            public string PathWithType { get; set; } = string.Empty;
        }

        private class WardJsonItem
        {
            public string Name { get; set; } = string.Empty;
            public string Slug { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty;

            [JsonPropertyName("name_with_type")]
            public string NameWithType { get; set; } = string.Empty;

            public string Code { get; set; } = string.Empty;

            [JsonPropertyName("parent_code")]
            public string ParentCode { get; set; } = string.Empty;

            public string Path { get; set; } = string.Empty;

            [JsonPropertyName("path_with_type")]
            public string PathWithType { get; set; } = string.Empty;
        }

        public class ProvinceDto
        {
            public string Code { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
        }

        public class DistrictDto
        {
            public string Code { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string ProvinceCode { get; set; } = string.Empty;
        }

        public class WardDto
        {
            public string Code { get; set; } = string.Empty;
            public string Name { get; set; } = string.Empty;
            public string DistrictCode { get; set; } = string.Empty;
        }
    }
}
