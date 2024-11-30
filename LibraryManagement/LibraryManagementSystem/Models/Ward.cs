using System;
using System.Collections.Generic;

namespace LibraryManagementSystem.Models;

public partial class Ward : AddressType
{

    public int? District { get; set; }

    public virtual District? DistrictNavigation { get; set; }

    public virtual ICollection<Street> Streets { get; set; } = new List<Street>();
}
