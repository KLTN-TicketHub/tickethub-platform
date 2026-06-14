using Catalog.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Application.Common.DTOs.Events
{
    public class EventDto
    {
        public Guid Id { get; set; }

        public Guid? SeatMapId { get; set; }

        public Guid CategoryId { get; set; }

        public string CategoryName { get; set; }

        public string Title { get; set; }

        public string Slug { get; set; }

        public string Description { get; set; }

        public DateTime StartAt { get; set; }

        public DateTime EndAt { get; set; }

        public DateTime SaleOpenAt { get; set; }

        public DateTime SaleCloseAt { get; set; }

        public string CoverImageUrl { get; set; }

        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public byte[] RowVersion { get; set; }
    }   
}
