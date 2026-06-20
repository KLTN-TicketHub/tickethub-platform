using System;
using System.Collections.Generic;
using System.Text;

namespace Catalog.Application.Features.Events.Requests
{
    public class CreateShowTimeRequest
    {
        public DateTime StartAt { get; set; }

        public DateTime EndAt { get; set; }
    }
}
