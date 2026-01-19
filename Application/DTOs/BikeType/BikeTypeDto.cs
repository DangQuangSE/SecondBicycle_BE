using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.BikeType
{
    public class BikeTypeDto
    {
        public int TypeId { get; set; }
        public string TypeName { get; set; } = null!;
        public int BicycleCount { get; set; } // S? lý?ng xe thu?c lo?i này
    }
}
