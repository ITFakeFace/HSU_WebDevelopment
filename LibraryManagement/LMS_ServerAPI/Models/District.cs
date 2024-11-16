using System;
using System.Collections.Generic;

namespace LMS_ServerAPI.Models;

public partial class District
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? City { get; set; }

    public virtual City? CityNavigation { get; set; }

    public virtual ICollection<Ward> Wards { get; set; } = new List<Ward>();
}
