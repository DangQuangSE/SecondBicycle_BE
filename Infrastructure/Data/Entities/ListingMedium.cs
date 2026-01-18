using System;
using System.Collections.Generic;

namespace Infrastructure.Data.Entities;

public partial class ListingMedium
{
    public int MediaId { get; set; }

    public int ListingId { get; set; }

    public string MediaUrl { get; set; } = null!;

    public string? MediaType { get; set; }

    public bool? IsThumbnail { get; set; }

    public virtual BicycleListing Listing { get; set; } = null!;
}
