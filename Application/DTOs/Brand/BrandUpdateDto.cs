using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.Brand
{
    public class BrandUpdateDto
    {
        public string BrandName { get; set; } = null!;
        public string? Country { get; set; }
    }
}
