using System;
using System.Collections.Generic;

namespace Infrastructure.Data.Entities;

public partial class ConfigurationSystem
{
    public int ConfigId { get; set; }

    public string ConfigKey { get; set; } = null!;

    public string? ConfigValue { get; set; }

    public string? Description { get; set; }
}
