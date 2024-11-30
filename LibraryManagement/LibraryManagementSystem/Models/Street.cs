using System;
using System.Collections.Generic;

namespace LibraryManagementSystem.Models;

public partial class Street : AddressType
{

    public int? Ward { get; set; }

    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    public virtual Ward? WardNavigation { get; set; }
}
