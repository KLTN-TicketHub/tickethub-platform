using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Application.Common.DTOs.Events
{
    public class EventLocationDto
    {
        public string VenueName { get; set; }

        public string AddressLine { get; set; }

        public string Ward { get; set; }

        public string District { get; set; }

        public string ProvinceCity { get; set; }

        public string Country { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
