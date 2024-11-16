using System;
using System.Collections.Generic;

namespace LMS_ServerAPI.Models;

public partial class Address
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? Street { get; set; }

    public virtual ICollection<Library> Libraries { get; set; } = new List<Library>();

    public virtual Street? StreetNavigation { get; set; }

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
