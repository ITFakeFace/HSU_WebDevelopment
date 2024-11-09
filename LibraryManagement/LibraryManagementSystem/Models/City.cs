using System;
using System.Collections.Generic;

namespace LibraryManagementSystem.Models;

public partial class City
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<District> Districts { get; set; } = new List<District>();
}
