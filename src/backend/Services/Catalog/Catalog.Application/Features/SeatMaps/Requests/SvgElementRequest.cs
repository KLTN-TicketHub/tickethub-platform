namespace Catalog.Application.Features.SeatMaps.Requests
{
    public class SvgElementRequest
    {
        public string Type { get; set; }

        public decimal X { get; set; }
        public decimal Y { get; set; }
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public string? Fill { get; set; } // Màu tô (rgba hoặc hex)
        public string? Stroke { get; set; } // Màu viền (nếu có)
        public decimal? StrokeWidth { get; set; } // Độ dày viền (nếu có)

        // Dành riêng cho thẻ <path> (Dữ liệu nét vẽ d="M...")
        public string? Data { get; set; }

        // Dành riêng cho thẻ <text>
        public string? Text { get; set; }
        public decimal? FontSize { get; set; }
        public string? FontFamily { get; set; }
    }
}
