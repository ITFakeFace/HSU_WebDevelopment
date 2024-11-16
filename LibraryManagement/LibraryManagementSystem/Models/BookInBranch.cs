using System;
using System.Collections.Generic;

namespace LibraryManagementSystem.Models;

public partial class BookInBranch
{
    public int Library { get; set; }

    public int Book { get; set; }

    public int? Amount { get; set; }

    public string? CallNumber { get; set; }

    public virtual Book BookNavigation { get; set; } = null!;

    public virtual Library LibraryNavigation { get; set; } = null!;
}
