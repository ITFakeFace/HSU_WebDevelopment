using System;
using System.Collections.Generic;

namespace LMS_ServerAPI.Models;

public partial class Ward
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? District { get; set; }

    public virtual District? DistrictNavigation { get; set; }

    public virtual ICollection<Street> Streets { get; set; } = new List<Street>();
}
