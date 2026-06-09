namespace Catalog.Application.Common.DTOs.SeatMaps
{
    public class RowDto
    {
        public Guid Id { get; set; }

        public string RowName { get; set; }

        public List<SeatDto> Seats { get; set; }

        public byte[] RowVersion { get; set; }
    }
}
