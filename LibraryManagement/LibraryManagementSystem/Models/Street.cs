using System;
using System.Collections.Generic;

namespace LibraryManagementSystem.Models;

public partial class Street
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? Ward { get; set; }

    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    public virtual Ward? WardNavigation { get; set; }
}
